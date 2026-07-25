#Requires -Version 7.0
<#
.SYNOPSIS
    Dispara o release do GitHub Actions (nextgen-release.yml) com um comando/clique,
    sem precisar abrir o navegador e preencher o formulario manualmente.

.PARAMETER Version
    Versao semantica (ex: v1.0.5). Se omitido, incrementa automaticamente o
    "patch" da ultima release publicada.

.PARAMETER Channel
    stable | beta | homolog | appliance (default: appliance)

.PARAMETER Platform
    windows | linux | appliance (default: appliance)

.EXEMPLO
    .\release.ps1
    .\release.ps1 -Version v1.1.0 -Channel stable -Platform appliance
#>

param(
    [string]$Version,
    [string]$Channel = "appliance",
    [string]$Platform = "appliance"
)

$ErrorActionPreference = "Stop"
$Repo = "rgerio01/R-L-Atelie"
$EnvFile = Join-Path $PSScriptRoot "..\..\.env"

function Step($msg) { Write-Host "`n  -> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    OK $msg" -ForegroundColor Green }
function Fail($msg) { Write-Host "ERRO: $msg" -ForegroundColor Red; exit 1 }

# ── Le o token do .env (nunca fica hardcoded no script) ───────────────────────
if (-not (Test-Path $EnvFile)) {
    Fail ".env nao encontrado em $EnvFile. Crie a variavel GITHUB_TOKEN nele (veja .env.example)."
}
$token = $null
foreach ($line in Get-Content $EnvFile) {
    if ($line -match '^\s*GITHUB_TOKEN\s*=\s*(.+)\s*$') { $token = $Matches[1].Trim('"', "'") }
}
if (-not $token) {
    Fail "GITHUB_TOKEN nao definido no .env. Crie um Personal Access Token em https://github.com/settings/tokens/new (escopos: repo + workflow) e adicione a linha GITHUB_TOKEN=ghp_xxx no .env."
}

$headers = @{
    Authorization = "token $token"
    Accept        = "application/vnd.github+json"
}

# ── Calcula a proxima versao automaticamente, se nao informada ───────────────
# Nao usamos /releases/latest: nossas releases sao todas marcadas "prerelease"
# (canais homolog/appliance/beta), e esse endpoint do GitHub ignora prerelease/draft
# e devolve 404 mesmo com releases existentes. Em vez disso listamos todas e achamos
# o maior semver manualmente.
if (-not $Version) {
    Step "Consultando releases existentes..."
    $maxMajor = -1; $maxMinor = 0; $maxPatch = 0
    try {
        $all = Invoke-RestMethod -Uri "https://api.github.com/repos/$Repo/releases?per_page=100" -Headers $headers
        foreach ($r in $all) {
            if ($r.tag_name -match '^v(\d+)\.(\d+)\.(\d+)$') {
                $maj = [int]$Matches[1]; $min = [int]$Matches[2]; $pat = [int]$Matches[3]
                if (($maj -gt $maxMajor) -or ($maj -eq $maxMajor -and $min -gt $maxMinor) -or ($maj -eq $maxMajor -and $min -eq $maxMinor -and $pat -gt $maxPatch)) {
                    $maxMajor = $maj; $maxMinor = $min; $maxPatch = $pat
                }
            }
        }
    } catch { }
    if ($maxMajor -lt 0) {
        $Version = "v1.0.0"
    } else {
        $Version = "v{0}.{1}.{2}" -f $maxMajor, $maxMinor, ($maxPatch + 1)
    }
    Ok "Proxima versao: $Version"
}

# ── Dispara o workflow ────────────────────────────────────────────────────────
Step "Disparando nextgen-release.yml (version=$Version, channel=$Channel, platform=$Platform)..."
$body = @{
    ref    = "master"
    inputs = @{
        version  = $Version
        channel  = $Channel
        platform = $Platform
    }
} | ConvertTo-Json

try {
    Invoke-RestMethod -Method Post `
        -Uri "https://api.github.com/repos/$Repo/actions/workflows/nextgen-release.yml/dispatches" `
        -Headers $headers -Body $body -ContentType "application/json"
} catch {
    Fail "Falha ao disparar o workflow: $($_.Exception.Message)"
}
Ok "Workflow disparado com sucesso!"

# ── Acha a run recem-criada para dar o link direto ────────────────────────────
Start-Sleep -Seconds 3
try {
    $runs = Invoke-RestMethod -Uri "https://api.github.com/repos/$Repo/actions/workflows/nextgen-release.yml/runs?per_page=1" -Headers $headers
    if ($runs.workflow_runs.Count -gt 0) {
        $run = $runs.workflow_runs[0]
        Write-Host ""
        Write-Host "  Acompanhe em: $($run.html_url)" -ForegroundColor Yellow
    }
} catch { }

Write-Host ""
Write-Host "  Versao $Version (channel=$Channel, platform=$Platform) em andamento." -ForegroundColor Green
Write-Host "  Assim que o release terminar, reinicie a VM/appliance para aplicar a atualizacao." -ForegroundColor Gray
Write-Host ""

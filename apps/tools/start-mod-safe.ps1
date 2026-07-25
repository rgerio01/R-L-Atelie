<#
.SYNOPSIS
    Abre o EquipeExe MOD com todos os bloqueios LavSoft ativos.

.DESCRIPTION
    Verifica que:
    1. hosts file redireciona lavsoft.com.br para 127.0.0.1
    2. Mock server esta rodando na porta 80
    3. LiveUpdate.exe e o stub bloqueador (nao o original)

    Se qualquer protecao estiver ausente, exibe aviso antes de abrir.
    NAO aplica as protecoes automaticamente (requer admin).
    Para aplicar: .\apply-lavsoft-intercept.ps1 (como admin)
#>

$ErrorActionPreference = "Stop"

$MOD = "D:\AtelieProd\MOD"
$EXE_DIR = "$MOD\apps\legacy-runtime\Equipexe\Exe"
$LAVSOFT = "$EXE_DIR\LavSoft.exe"
$LIVE_UPDATE = "$EXE_DIR\LiveUpdate.exe"
$HOSTS = "C:\Windows\System32\drivers\etc\hosts"

$warnings = @()

# Verifica hosts
$hostsOk = (Get-Content $HOSTS -Raw) -match "127\.0\.0\.1\s+www\.lavsoft\.com\.br"
if (-not $hostsOk) {
    $warnings += "AVISO: hosts file NAO redireciona lavsoft.com.br — execute apply-lavsoft-intercept.ps1 como admin"
}

# Verifica mock server (porta 80)
$port80 = Get-NetTCPConnection -LocalPort 80 -State Listen -ErrorAction SilentlyContinue
if (-not $port80) {
    $warnings += "AVISO: Nenhum servico escutando na porta 80 — /TestaAutentica pode falhar"
}

# Verifica LiveUpdate stub
$liveUpdateSize = (Get-Item $LIVE_UPDATE -ErrorAction SilentlyContinue)?.Length
if ($liveUpdateSize -gt 5MB) {
    $warnings += "AVISO: LiveUpdate.exe parece ser o original ($(($liveUpdateSize/1MB).ToString('F1'))MB) — risco de atualizacao"
}

# Exibe avisos
if ($warnings) {
    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Yellow
    Write-Host "  PROTECOES INCOMPLETAS" -ForegroundColor Yellow
    Write-Host "==========================================" -ForegroundColor Yellow
    foreach ($w in $warnings) {
        Write-Host "  $w" -ForegroundColor Yellow
    }
    Write-Host ""
    $resp = Read-Host "Continuar mesmo assim? (s/N)"
    if ($resp.ToLower() -ne "s") {
        Write-Host "Cancelado. Execute apply-lavsoft-intercept.ps1 como admin primeiro."
        exit 1
    }
}

# Abre LavSoft MOD
if (-not (Test-Path $LAVSOFT)) {
    throw "LavSoft.exe nao encontrado em $EXE_DIR"
}

$logDir = "$MOD\logs\communication"
New-Item -ItemType Directory -Path $logDir -Force | Out-Null
"[$(Get-Date -Format o)] Abrindo LavSoft MOD. Protecoes: hosts=$hostsOk porta80=$(!!$port80)" |
    Out-File -LiteralPath "$logDir\mod-launcher.log" -Encoding UTF8 -Append

Write-Host ""
Write-Host "Abrindo EquipeExe MOD..."
Write-Host "  hosts:    $(if ($hostsOk) {'OK — lavsoft.com.br → 127.0.0.1'} else {'AUSENTE'})"
Write-Host "  porta 80: $(if ($port80) {'OK — mock server ativo'} else {'AUSENTE'})"
Write-Host ""

Start-Process -FilePath $LAVSOFT -WorkingDirectory $EXE_DIR

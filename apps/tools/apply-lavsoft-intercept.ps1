<#
.SYNOPSIS
    Intercepta todo o trafego HTTP do EquipeExe para www.lavsoft.com.br.

.DESCRIPTION
    Implementa 3 camadas de bloqueio/interceptacao:
    1. hosts file → redireciona www.lavsoft.com.br e lavsoft.com.br para 127.0.0.1
    2. LavSoftMock → servidor Python local na porta 80 que responde endpoints criticos
    3. Firewall → bloqueia outbound de executaveis do MOD para internet (exceto 127.0.0.1)

    Resultado esperado:
    - /TestaAutentica     → resposta "1" (licenca valida) do mock local
    - /EnviaMovimento*    → absorvido pelo mock (dados NAO vao para LavSoft)
    - /VerificaAtualizacoes → ja bloqueado pelo LiveUpdate.Disabled + resposta mock
    - Gerenciador.exe     → endpoints administrativos bloqueados na camada de firewall

.NOTES
    REQUER execucao como Administrador.
    Rollback: rollback-lavsoft-intercept.ps1
    Log: D:\AtelieProd\MOD\logs\communication\lavsoft-intercept-YYYYMMDD.jsonl
#>

#Requires -RunAsAdministrator

$ErrorActionPreference = "Stop"

$MOD = "D:\AtelieProd\MOD"
$LOG_DIR = "$MOD\logs\communication"
$MOCK_SERVER = "$MOD\apps\services\LavSoftMock\server.py"
$HOSTS = "C:\Windows\System32\drivers\etc\hosts"
$EXE_DIR = "$MOD\apps\legacy-runtime\Equipexe\Exe"

$ts = Get-Date -Format "yyyyMMdd_HHmmss"
$logFile = "$LOG_DIR\lavsoft-intercept-$(Get-Date -Format 'yyyyMMdd').jsonl"

function Write-Log {
    param($event, $detail, $status = "ok")
    $entry = @{
        ts     = (Get-Date -Format "o")
        event  = $event
        detail = $detail
        status = $status
    } | ConvertTo-Json -Compress
    $entry | Out-File -LiteralPath $logFile -Encoding UTF8 -Append
    Write-Host "[$(Get-Date -Format HH:mm:ss)] $event — $detail"
}

New-Item -ItemType Directory -Path $LOG_DIR -Force | Out-Null

Write-Host ""
Write-Host "=========================================="
Write-Host "  LavSoft Intercept — Aplicando bloqueio"
Write-Host "=========================================="
Write-Host ""

# ──────────────────────────────────────────────────────────────────
# CAMADA 1 — hosts file
# ──────────────────────────────────────────────────────────────────
$hostsContent = Get-Content -Path $HOSTS -Raw -ErrorAction SilentlyContinue

$hostsEntries = @(
    "127.0.0.1  www.lavsoft.com.br",
    "127.0.0.1  lavsoft.com.br"
)

$hostsChanged = $false
foreach ($entry in $hostsEntries) {
    $domain = ($entry -split "\s+")[1]
    if ($hostsContent -notmatch [regex]::Escape($domain)) {
        Add-Content -Path $HOSTS -Value $entry -Encoding ASCII
        Write-Log "hosts-entry-added" $entry
        $hostsChanged = $true
    } else {
        Write-Log "hosts-entry-exists" $entry "skip"
    }
}

# Limpa cache DNS
ipconfig /flushdns | Out-Null
Write-Log "dns-cache-flushed" "ipconfig /flushdns"

# ──────────────────────────────────────────────────────────────────
# CAMADA 2 — urlacl para Python escutar porta 80 sem admin em tempo de execucao
# ──────────────────────────────────────────────────────────────────
$urlacl = netsh http show urlacl url="http://+:80/" 2>&1
if ($urlacl -notmatch "http\+:80") {
    netsh http add urlacl url="http://+:80/" user="NT AUTHORITY\NETWORK SERVICE" | Out-Null
    Write-Log "urlacl-added" "http://+:80/ NETWORK SERVICE"
} else {
    Write-Log "urlacl-exists" "http://+:80/" "skip"
}

# ──────────────────────────────────────────────────────────────────
# CAMADA 2b — inicia mock server em background
# ──────────────────────────────────────────────────────────────────
$pythonExe = (Get-Command python -ErrorAction SilentlyContinue)?.Source
if (-not $pythonExe) {
    $pythonExe = (Get-Command py -ErrorAction SilentlyContinue)?.Source
}

if ($pythonExe -and (Test-Path $MOCK_SERVER)) {
    # Verifica se ja esta rodando
    $running = Get-Process python -ErrorAction SilentlyContinue |
               Where-Object { $_.MainWindowTitle -like "*mock*" -or $true } |
               Where-Object { (Get-NetTCPConnection -OwningProcess $_.Id -ErrorAction SilentlyContinue | Where-Object LocalPort -eq 80) }

    if ($running) {
        Write-Log "mock-server-already-running" "PID=$($running.Id)" "skip"
    } else {
        $proc = Start-Process -FilePath $pythonExe `
                              -ArgumentList $MOCK_SERVER `
                              -WorkingDirectory (Split-Path $MOCK_SERVER) `
                              -WindowStyle Hidden `
                              -PassThru
        Write-Log "mock-server-started" "PID=$($proc.Id) porta=80"
        # Guarda PID para rollback
        $proc.Id | Out-File -LiteralPath "$MOD\logs\communication\lavsoft-mock.pid" -Encoding ASCII
    }
} else {
    Write-Log "mock-server-skip" "Python nao encontrado ou server.py ausente" "warn"
}

# ──────────────────────────────────────────────────────────────────
# CAMADA 3 — Firewall: bloqueia executaveis MOD que comunicam com internet
# ──────────────────────────────────────────────────────────────────
$exes = @(
    "LavSoft.exe",
    "Gerenciador.exe",
    "LavFacilLan.exe",
    "Estoque.exe",
    "Financeiro.exe",
    "SAT.exe",
    "NFE.exe",
    "LiveUpdate.exe"
)

foreach ($exe in $exes) {
    $path = Join-Path $EXE_DIR $exe
    if (-not (Test-Path -LiteralPath $path)) {
        Write-Log "firewall-skip" "$exe nao encontrado no MOD runtime" "skip"
        continue
    }

    $ruleName = "MOD LavSoft Block Outbound — $exe"
    $existing = Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue
    try {
        if ($existing) {
            Set-NetFirewallRule -DisplayName $ruleName -Enabled True -Action Block | Out-Null
            Write-Log "firewall-updated" $ruleName
        } else {
            New-NetFirewallRule `
                -DisplayName $ruleName `
                -Direction Outbound `
                -Program $path `
                -Action Block `
                -Profile Any `
                -Enabled True `
                -RemoteAddress "Internet" `
                -Description "Bloqueia $exe do MOD runtime de acessar internet (LavSoft). Rollback: rollback-lavsoft-intercept.ps1" `
                | Out-Null
            Write-Log "firewall-created" $ruleName
        }
    } catch {
        Write-Log "firewall-error" "$ruleName — $($_.Exception.Message)" "error"
    }
}

# ──────────────────────────────────────────────────────────────────
# Resultado
# ──────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "=========================================="
Write-Host "  Intercept aplicado com sucesso"
Write-Host "=========================================="
Write-Host ""
Write-Host "  hosts:    www.lavsoft.com.br → 127.0.0.1"
Write-Host "  mock:     http://127.0.0.1:80 (Python)"
Write-Host "  firewall: executaveis MOD bloqueados para internet"
Write-Host ""
Write-Host "  Log: $logFile"
Write-Host ""
Write-Host "  Para reverter: .\rollback-lavsoft-intercept.ps1"
Write-Host ""

Write-Log "intercept-complete" "hosts+urlacl+mock+firewall aplicados"

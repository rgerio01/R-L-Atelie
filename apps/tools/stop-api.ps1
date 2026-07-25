$ErrorActionPreference = 'Stop'

$pidFile = 'D:\AtelieProd\MOD\runtime\api.pid'

if (!(Test-Path -LiteralPath $pidFile)) {
    Write-Output 'PID da API nao encontrado.'
    exit 0
}

$apiPid = Get-Content -LiteralPath $pidFile -ErrorAction SilentlyContinue
if ($apiPid -and (Get-Process -Id $apiPid -ErrorAction SilentlyContinue)) {
    Stop-Process -Id $apiPid
    Write-Output "API parada. PID=$apiPid"
} else {
    Write-Output "Processo da API nao estava ativo. PID=$apiPid"
}

Remove-Item -LiteralPath $pidFile -Force -ErrorAction SilentlyContinue

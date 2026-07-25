$ErrorActionPreference = 'Stop'

$modRoot = 'D:\AtelieProd\MOD'
$project = Join-Path $modRoot 'apps\backend\EquipeExe.Mod.Api'
$stdout = Join-Path $modRoot 'logs\analysis\api-homologacao.log'
$stderr = Join-Path $modRoot 'logs\failures\api-homologacao.err.log'
$pidFile = Join-Path $modRoot 'runtime\api.pid'

if (Test-Path -LiteralPath $pidFile) {
    $existingPid = Get-Content -LiteralPath $pidFile -ErrorAction SilentlyContinue
    if ($existingPid -and (Get-Process -Id $existingPid -ErrorAction SilentlyContinue)) {
        Write-Output "API ja esta em execucao. PID=$existingPid"
        exit 0
    }
}

$process = Start-Process `
    -FilePath 'dotnet' `
    -ArgumentList @('run', '--project', $project, '--urls', 'http://127.0.0.1:5058') `
    -WorkingDirectory $project `
    -RedirectStandardOutput $stdout `
    -RedirectStandardError $stderr `
    -PassThru `
    -WindowStyle Hidden

$process.Id | Out-File -LiteralPath $pidFile -Encoding ascii
Write-Output "API iniciada em http://127.0.0.1:5058. PID=$($process.Id)"

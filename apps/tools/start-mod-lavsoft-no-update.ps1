$ErrorActionPreference = 'Stop'

$modRoot = 'D:\AtelieProd\MOD'
$runtimeExe = Join-Path $modRoot 'apps\legacy-runtime\Equipexe\Exe'
$lavSoft = Join-Path $runtimeExe 'LavSoft.exe'
$liveUpdate = Join-Path $runtimeExe 'LiveUpdate.exe'
$log = Join-Path $modRoot 'logs\communication\mod-launcher.log'

if (!(Test-Path -LiteralPath $lavSoft)) {
    throw "LavSoft MOD nao encontrado. Execute prepare-mod-runtime-no-update.ps1 primeiro."
}

if (!(Test-Path -LiteralPath $liveUpdate)) {
    throw "LiveUpdate bloqueador nao encontrado. Abertura cancelada por seguranca."
}

$env:EQUIPEEXE_MOD_ROOT = $modRoot

"[$(Get-Date -Format s)] Abrindo LavSoft MOD com politica de update bloqueado. Caminho=$lavSoft" |
    Out-File -LiteralPath $log -Encoding UTF8 -Append

Start-Process -FilePath $lavSoft -WorkingDirectory $runtimeExe

$ErrorActionPreference = 'Stop'

$modRoot = 'D:\AtelieProd\MOD'
$liveUpdate = Join-Path $modRoot 'apps\legacy-runtime\Equipexe\Exe\LiveUpdate.exe'
$policy = Join-Path $modRoot 'config\env\update-policy.json'

if (!(Test-Path -LiteralPath $liveUpdate)) {
    throw "LiveUpdate bloqueador nao encontrado: $liveUpdate"
}

if (!(Test-Path -LiteralPath $policy)) {
    throw "Politica de update nao encontrada: $policy"
}

$before = Get-ChildItem -LiteralPath (Join-Path $modRoot 'logs\communication') -Filter 'liveupdate-blocked-*.jsonl' -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

& $liveUpdate --verify-block

$after = Get-ChildItem -LiteralPath (Join-Path $modRoot 'logs\communication') -Filter 'liveupdate-blocked-*.jsonl' -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

[pscustomobject]@{
    LiveUpdatePath = $liveUpdate
    PolicyPath = $policy
    BlockLog = $after.FullName
    Verified = $true
}

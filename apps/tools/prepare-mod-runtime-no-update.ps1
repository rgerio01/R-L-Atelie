$ErrorActionPreference = 'Stop'

$modRoot = 'D:\AtelieProd\MOD'
$runtimeRoot = Join-Path $modRoot 'apps\legacy-runtime\Equipexe'
$runtimeExe = Join-Path $runtimeRoot 'Exe'
$stubPublish = Join-Path $modRoot 'apps\services\LiveUpdate.Disabled\bin\Release\net8.0\win-x64\publish'
$stubSource = Join-Path $stubPublish 'LiveUpdate.exe'
$policy = Join-Path $modRoot 'config\env\update-policy.json'
$log = Join-Path $modRoot 'logs\communication\mod-runtime-update-policy.log'

New-Item -ItemType Directory -Path $runtimeExe -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path $log -Parent) -Force | Out-Null

dotnet publish (Join-Path $modRoot 'apps\services\LiveUpdate.Disabled\LiveUpdate.Disabled.csproj') `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -o $stubPublish | Out-Null

Copy-Item -LiteralPath $stubSource -Destination (Join-Path $runtimeExe 'LiveUpdate.exe') -Force
Copy-Item -LiteralPath $policy -Destination (Join-Path $runtimeRoot 'update-policy.json') -Force

@(
    'Gerenciador.exe',
    'LavSoft.exe',
    'LavFacilLan.exe',
    'Financeiro.exe',
    'Estoque.exe',
    'NFE.exe',
    'SAT.exe',
    'Sincronizar.exe'
) | ForEach-Object {
    $source = Join-Path 'D:\AtelieProd\Equipexe\Exe' $_
    $target = Join-Path $runtimeExe $_
    if ((Test-Path -LiteralPath $source) -and !(Test-Path -LiteralPath $target)) {
        Copy-Item -LiteralPath $source -Destination $target -Force
    }
}

"[$(Get-Date -Format s)] Runtime MOD preparado com LiveUpdate bloqueado em $runtimeExe" |
    Out-File -LiteralPath $log -Encoding UTF8 -Append

Write-Output "Runtime MOD preparado: $runtimeRoot"
Write-Output "LiveUpdate bloqueado: $(Join-Path $runtimeExe 'LiveUpdate.exe')"

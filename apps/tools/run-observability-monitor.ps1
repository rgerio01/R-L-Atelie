param(
    [ValidateSet('LavSoft','LavFacilLan','Gerenciador','Financeiro','Estoque','NFE','SAT')]
    [string]$Target = 'LavSoft',

    [int]$Seconds = 60,

    [int]$IntervalMs = 1000,

    [switch]$Close
)

$ErrorActionPreference = 'Stop'

$runtimeExe = 'D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe'
$observer = 'D:\AtelieProd\MOD\apps\tools\EquipeExe.Mod.Observability\bin\Release\net8.0\EquipeExe.Mod.Observability.dll'
$outDir = 'D:\AtelieProd\MOD\logs\observability'

$targetPath = Join-Path $runtimeExe "$Target.exe"
if (!(Test-Path -LiteralPath $targetPath)) {
    throw "Executavel MOD nao encontrado: $targetPath"
}

if (!(Test-Path -LiteralPath $observer)) {
    dotnet build 'D:\AtelieProd\MOD\apps\tools\EquipeExe.Mod.Observability\EquipeExe.Mod.Observability.csproj' -c Release | Out-Host
}

$args = @(
    $observer,
    'monitor',
    '--exe', $targetPath,
    '--cwd', $runtimeExe,
    '--seconds', $Seconds,
    '--interval-ms', $IntervalMs,
    '--out', $outDir
)

if ($Close) {
    $args += '--close'
}

dotnet @args

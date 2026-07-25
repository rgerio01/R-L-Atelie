param(
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

$sourceExe = 'D:\AtelieProd\Equipexe\Exe'
$targetExe = 'D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe'
$report = 'D:\AtelieProd\MOD\docs\01-inventario\dependencias-copiadas-runtime-mod.csv'

if (!(Test-Path -LiteralPath $sourceExe)) {
    throw "Origem nao encontrada: $sourceExe"
}

if (!(Test-Path -LiteralPath $targetExe)) {
    throw "Runtime MOD nao encontrado: $targetExe"
}

$patterns = @('*.dll', '*.ocx')
$copied = New-Object System.Collections.Generic.List[object]

foreach ($pattern in $patterns) {
    Get-ChildItem -LiteralPath $sourceExe -Filter $pattern -File -Force | ForEach-Object {
        $destination = Join-Path $targetExe $_.Name
        $action = 'skipped_exists'

        if ($Force -or !(Test-Path -LiteralPath $destination)) {
            Copy-Item -LiteralPath $_.FullName -Destination $destination -Force:$Force
            $action = 'copied'
        }

        $copied.Add([pscustomobject]@{
            Name = $_.Name
            Source = $_.FullName
            Destination = $destination
            Length = $_.Length
            LastWriteTime = $_.LastWriteTime
            Action = $action
        })
    }
}

$copied |
    Sort-Object Name |
    ConvertTo-Csv -NoTypeInformation |
    Out-File -LiteralPath $report -Encoding UTF8

Write-Output "Dependencias avaliadas: $($copied.Count)"
Write-Output "Copiadas: $(($copied | Where-Object Action -eq 'copied').Count)"
Write-Output "Relatorio: $report"

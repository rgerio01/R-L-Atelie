$ErrorActionPreference = 'Stop'

$sourceRoot = 'D:\AtelieProd\Equipexe'
$targetRoot = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe'
$manifest = 'D:\AtelieProd\MOD\docs\03-banco-de-dados\copia-readonly-paradox.csv'
$log = 'D:\AtelieProd\MOD\logs\migration\copy-paradox-readonly.log'

$extensions = @('.DB', '.PX', '.MB', '.XG0', '.YG0', '.XG1', '.YG1', '.XG2', '.YG2', '.XG3', '.YG3', '.XG4', '.YG4', '.XG5', '.YG5', '.XG6', '.YG6', '.XG7', '.YG7', '.XG8', '.YG8', '.XG9', '.YG9', '.XGA', '.YGA', '.XGB', '.YGB', '.XGC', '.YGC', '.XGD', '.YGD', '.DBF')

New-Item -ItemType Directory -Path $targetRoot -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path $manifest -Parent) -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path $log -Parent) -Force | Out-Null

$copied = New-Object System.Collections.Generic.List[object]

Get-ChildItem -LiteralPath $sourceRoot -Recurse -Force -File -ErrorAction SilentlyContinue |
    Where-Object { $extensions -contains $_.Extension.ToUpperInvariant() } |
    Where-Object { $_.Name -notin @('Thumbs.db') } |
    ForEach-Object {
        $relative = $_.FullName.Substring($sourceRoot.Length).TrimStart('\')
        $target = Join-Path $targetRoot $relative
        $targetDirectory = Split-Path $target -Parent
        New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null
        Copy-Item -LiteralPath $_.FullName -Destination $target -Force
        Set-ItemProperty -LiteralPath $target -Name IsReadOnly -Value $true
        $targetItem = Get-Item -LiteralPath $target

        $copied.Add([pscustomobject]@{
            Source = $_.FullName
            Target = $target
            RelativePath = $relative
            Extension = $_.Extension
            Length = $_.Length
            LastWriteTime = $_.LastWriteTime
            CopiedAt = Get-Date
            ReadOnly = $targetItem.IsReadOnly
        })
    }

$copied | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $manifest -Encoding UTF8
"[$(Get-Date -Format s)] Copia readonly concluida. Arquivos=$($copied.Count). Destino=$targetRoot" | Out-File -LiteralPath $log -Encoding UTF8 -Append
Write-Output "Arquivos copiados: $($copied.Count)"
Write-Output "Manifesto: $manifest"

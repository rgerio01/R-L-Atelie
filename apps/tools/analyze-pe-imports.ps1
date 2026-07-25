$ErrorActionPreference = 'Stop'

$root = 'D:\AtelieProd\Equipexe'
$outDir = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\pe-imports'
$importsCsv = Join-Path $outDir 'imports-executaveis-dlls.csv'
$summaryCsv = Join-Path $outDir 'imports-resumo.csv'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

function Read-U16([byte[]]$b, [int]$o) { [BitConverter]::ToUInt16($b, $o) }
function Read-U32([byte[]]$b, [int]$o) { [BitConverter]::ToUInt32($b, $o) }

function Rva-To-Offset {
    param([uint32]$Rva, [object[]]$Sections)
    foreach ($s in $Sections) {
        $size = [Math]::Max($s.VirtualSize, $s.RawSize)
        if ($Rva -ge $s.VirtualAddress -and $Rva -lt ($s.VirtualAddress + $size)) {
            return [int]($s.RawPointer + ($Rva - $s.VirtualAddress))
        }
    }
    return $null
}

function Read-AsciiZ {
    param([byte[]]$Bytes, [int]$Offset)
    if ($Offset -lt 0 -or $Offset -ge $Bytes.Length) { return '' }
    $chars = New-Object System.Collections.Generic.List[byte]
    for ($i = $Offset; $i -lt $Bytes.Length; $i++) {
        if ($Bytes[$i] -eq 0) { break }
        $chars.Add($Bytes[$i])
    }
    return [System.Text.Encoding]::ASCII.GetString($chars.ToArray())
}

function Get-PEImports {
    param([string]$Path)

    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if ($bytes.Length -lt 0x100) { return @() }
    if ((Read-U16 $bytes 0) -ne 0x5A4D) { return @() }
    $peOffset = [int](Read-U32 $bytes 0x3C)
    if ($peOffset -le 0 -or $peOffset + 0xF8 -gt $bytes.Length) { return @() }
    if ((Read-U32 $bytes $peOffset) -ne 0x00004550) { return @() }

    $machine = Read-U16 $bytes ($peOffset + 4)
    $sectionCount = Read-U16 $bytes ($peOffset + 6)
    $optionalSize = Read-U16 $bytes ($peOffset + 20)
    $optionalOffset = $peOffset + 24
    $magic = Read-U16 $bytes $optionalOffset
    $isPe32Plus = $magic -eq 0x20B
    $dataDirectoryOffset = if ($isPe32Plus) { $optionalOffset + 112 } else { $optionalOffset + 96 }
    $importRva = Read-U32 $bytes ($dataDirectoryOffset + 8)
    if ($importRva -eq 0) { return @() }

    $sections = @()
    $sectionOffset = $optionalOffset + $optionalSize
    for ($i = 0; $i -lt $sectionCount; $i++) {
        $o = $sectionOffset + ($i * 40)
        if ($o + 40 -gt $bytes.Length) { break }
        $sections += [pscustomobject]@{
            Name = (Read-AsciiZ $bytes $o)
            VirtualSize = Read-U32 $bytes ($o + 8)
            VirtualAddress = Read-U32 $bytes ($o + 12)
            RawSize = Read-U32 $bytes ($o + 16)
            RawPointer = Read-U32 $bytes ($o + 20)
        }
    }

    $importOffset = Rva-To-Offset $importRva $sections
    if ($null -eq $importOffset) { return @() }

    $imports = New-Object System.Collections.Generic.List[object]
    for ($d = $importOffset; $d + 20 -le $bytes.Length; $d += 20) {
        $originalFirstThunk = Read-U32 $bytes $d
        $nameRva = Read-U32 $bytes ($d + 12)
        $firstThunk = Read-U32 $bytes ($d + 16)
        if ($originalFirstThunk -eq 0 -and $nameRva -eq 0 -and $firstThunk -eq 0) { break }
        $nameOffset = Rva-To-Offset $nameRva $sections
        if ($null -eq $nameOffset) { continue }
        $dllName = Read-AsciiZ $bytes $nameOffset
        $imports.Add([pscustomobject]@{
            File = $Path
            FileName = [System.IO.Path]::GetFileName($Path)
            Extension = [System.IO.Path]::GetExtension($Path)
            Machine = ('0x{0:X4}' -f $machine)
            ImportedDll = $dllName
        })
    }
    return $imports
}

$allImports = New-Object System.Collections.Generic.List[object]
Get-ChildItem -LiteralPath $root -Recurse -Force -File -Include *.exe,*.dll,*.ocx -ErrorAction SilentlyContinue |
    ForEach-Object {
        try {
            foreach ($import in (Get-PEImports -Path $_.FullName)) {
                $allImports.Add($import)
            }
        } catch {
            $allImports.Add([pscustomobject]@{
                File = $_.FullName
                FileName = $_.Name
                Extension = $_.Extension
                Machine = ''
                ImportedDll = "ERROR: $($_.Exception.Message)"
            })
        }
    }

$allImports | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $importsCsv -Encoding UTF8
$allImports |
    Where-Object { $_.ImportedDll -and $_.ImportedDll -notlike 'ERROR:*' } |
    Group-Object ImportedDll |
    Sort-Object Count -Descending |
    ForEach-Object {
        [pscustomobject]@{
            ImportedDll = $_.Name
            Count = $_.Count
            Files = (($_.Group | Select-Object -ExpandProperty FileName -Unique) -join '; ')
        }
    } | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $summaryCsv -Encoding UTF8

Write-Output "Imports: $($allImports.Count) -> $importsCsv"
Write-Output "Resumo: $summaryCsv"

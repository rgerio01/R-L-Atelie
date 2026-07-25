$ErrorActionPreference = 'Stop'

$targets = @(
    'D:\AtelieProd\Equipexe\Exe\LavSoft.exe',
    'D:\AtelieProd\Equipexe\Exe\LavFacilLan.exe',
    'D:\AtelieProd\Equipexe\Exe\Estoque.exe',
    'D:\AtelieProd\Equipexe\Exe\Financeiro.exe',
    'D:\AtelieProd\Equipexe\Exe\NFE.exe',
    'D:\AtelieProd\Equipexe\Exe\SAT.exe',
    'D:\AtelieProd\Equipexe\Exe\Senhas.exe'
)

$outDir = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\layouts-telas'
$formsCsv = Join-Path $outDir 'formularios-extraidos.csv'
$controlsCsv = Join-Path $outDir 'controles-extraidos.csv'
$bindingsCsv = Join-Path $outDir 'vinculos-banco-telas.csv'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

function Get-AsciiStrings {
    param([byte[]]$Bytes, [int]$Min = 4)

    $items = New-Object System.Collections.Generic.List[string]
    $sb = New-Object System.Text.StringBuilder
    foreach ($b in $Bytes) {
        if ($b -ge 32 -and $b -le 126) {
            [void]$sb.Append([char]$b)
        } else {
            if ($sb.Length -ge $Min) { $items.Add($sb.ToString()) }
            [void]$sb.Clear()
        }
    }
    if ($sb.Length -ge $Min) { $items.Add($sb.ToString()) }
    return $items
}

function Get-Utf16Strings {
    param([byte[]]$Bytes, [int]$Min = 4)

    $items = New-Object System.Collections.Generic.List[string]
    $sb = New-Object System.Text.StringBuilder
    for ($i = 0; $i -lt ($Bytes.Length - 1); $i += 2) {
        $value = [BitConverter]::ToUInt16($Bytes, $i)
        if ($value -ge 32 -and $value -le 126) {
            [void]$sb.Append([char]$value)
        } else {
            if ($sb.Length -ge $Min) { $items.Add($sb.ToString()) }
            [void]$sb.Clear()
        }
    }
    if ($sb.Length -ge $Min) { $items.Add($sb.ToString()) }
    return $items
}

function Get-Value {
    param([string]$Text, [string]$Key)
    $match = [regex]::Match($Text, "$Key(?:\s*)[:=](?:\s*)([^;|`r`n]+)")
    if ($match.Success) { return $match.Groups[1].Value.Trim().Trim("'").Trim('"') }
    return ''
}

function Find-PatternOffsets {
    param([byte[]]$Bytes, [byte[]]$Pattern)

    $offsets = New-Object System.Collections.Generic.List[int]
    for ($i = 0; $i -le $Bytes.Length - $Pattern.Length; $i++) {
        $match = $true
        for ($j = 0; $j -lt $Pattern.Length; $j++) {
            if ($Bytes[$i + $j] -ne $Pattern[$j]) {
                $match = $false
                break
            }
        }
        if ($match) { $offsets.Add($i) }
    }
    return $offsets
}

function Convert-BlockToText {
    param([byte[]]$Block)

    $chars = foreach ($b in $Block) {
        if ($b -ge 32 -and $b -le 126) {
            [char]$b
        } elseif ($b -eq 9 -or $b -eq 10 -or $b -eq 13) {
            ' '
        } else {
            ' '
        }
    }

    return ((-join $chars) -replace '\s+', ' ').Trim()
}

$forms = New-Object System.Collections.Generic.List[object]
$controls = New-Object System.Collections.Generic.List[object]
$bindings = New-Object System.Collections.Generic.List[object]

foreach ($target in $targets) {
    if (!(Test-Path -LiteralPath $target)) { continue }

    $exeName = [System.IO.Path]::GetFileNameWithoutExtension($target)
    $bytes = [System.IO.File]::ReadAllBytes($target)
    $pattern = [System.Text.Encoding]::ASCII.GetBytes('TPF0')
    $offsets = Find-PatternOffsets -Bytes $bytes -Pattern $pattern

    $blockIndex = 0
    foreach ($offset in $offsets) {
        $blockIndex++
        $length = [Math]::Min(100000, $bytes.Length - $offset)
        $block = New-Object byte[] $length
        [Array]::Copy($bytes, $offset, $block, 0, $length)
        $text = Convert-BlockToText -Block $block
        $blockFile = Join-Path $outDir "$exeName.tpf0.$blockIndex.txt"
        $text | Out-File -LiteralPath $blockFile -Encoding UTF8

        $formName = ''
        $formClass = ''
        if ($text -match 'TPF0\s*([A-Za-z][A-Za-z0-9_]*)\s*([A-Za-z][A-Za-z0-9_]*)?') {
            $formClass = $matches[1]
            $formName = if ($matches[2]) { $matches[2] } else { $matches[1] }
        } else {
            $formName = "TPF0_$blockIndex"
        }

        $forms.Add([pscustomobject]@{
            Executavel = $exeName
            Form = $formName
            Classe = $formClass
            Caption = Get-Value $text 'Caption'
            Left = Get-Value $text 'Left'
            Top = Get-Value $text 'Top'
            Width = Get-Value $text 'Width'
            Height = Get-Value $text 'Height'
            Origem = $target
        })

        [regex]::Matches($text, '(T(Label|Edit|DBEdit|DBLookupCombo|DBGrid|ComboBox|BitBtn|Button|MenuItem|MainMenu|Table|Query|DataSource|Panel|GroupBox|PageControl|TabSheet|CheckBox|RadioButton))\s+([A-Za-z0-9_]+).*?(?=(T(Label|Edit|DBEdit|DBLookupCombo|DBGrid|ComboBox|BitBtn|Button|MenuItem|MainMenu|Table|Query|DataSource|Panel|GroupBox|PageControl|TabSheet|CheckBox|RadioButton))\s+[A-Za-z0-9_]+|TPF0|$)') |
            ForEach-Object {
                $componentText = $_.Value
                $componentType = $_.Groups[1].Value
                $componentName = $_.Groups[3].Value
                $controls.Add([pscustomobject]@{
                    Executavel = $exeName
                    Form = $formName
                    Tipo = $componentType
                    Nome = $componentName
                    Caption = Get-Value $componentText 'Caption'
                    Left = Get-Value $componentText 'Left'
                    Top = Get-Value $componentText 'Top'
                    Width = Get-Value $componentText 'Width'
                    Height = Get-Value $componentText 'Height'
                    DataSource = Get-Value $componentText 'DataSource'
                    DataSet = Get-Value $componentText 'DataSet'
                    FieldName = Get-Value $componentText 'FieldName'
                    TableName = Get-Value $componentText 'TableName'
                    DatabaseName = Get-Value $componentText 'DatabaseName'
                    OnClick = Get-Value $componentText 'OnClick'
                    OnKeyPress = Get-Value $componentText 'OnKeyPress'
                    Raw = $componentText
                })
            }

        [regex]::Matches($text, '.{0,120}(TableName|DatabaseName|FieldName|DataSource|DataSet|SELECT|FROM).{0,240}', 'IgnoreCase') |
            ForEach-Object {
                $bindings.Add([pscustomobject]@{
                    Executavel = $exeName
                    Form = $formName
                    TableName = Get-Value $_.Value 'TableName'
                    DatabaseName = Get-Value $_.Value 'DatabaseName'
                    FieldName = Get-Value $_.Value 'FieldName'
                    DataSource = Get-Value $_.Value 'DataSource'
                    DataSet = Get-Value $_.Value 'DataSet'
                    Texto = $_.Value
                })
            }
    }

    $strings = @(Get-AsciiStrings -Bytes $bytes -Min 4) + @(Get-Utf16Strings -Bytes $bytes -Min 4)
    $strings = $strings | Sort-Object -Unique

    $rawOut = Join-Path $outDir "$exeName.layout-strings.txt"
    $strings |
        Where-Object { $_ -match '(?i)(TPF0|TForm|TF_|TFC_|Caption|Left|Top|Width|Height|TTable|TQuery|TDataSource|FieldName|DataSet|DatabaseName|TableName|OnClick|OnKeyPress|TDB|TEdit|TLabel|TButton|TBitBtn|TMenuItem|MainMenu)' } |
        Out-File -LiteralPath $rawOut -Encoding UTF8

    $currentForm = ''
    foreach ($line in $strings) {
        if ($line -match 'TPF0\s*([A-Za-z0-9_]+)\s*([A-Za-z0-9_]+)?') {
            $currentForm = if ($matches[2]) { $matches[2] } else { $matches[1] }
            $forms.Add([pscustomobject]@{
                Executavel = $exeName
                Form = $currentForm
                Classe = $matches[1]
                Caption = Get-Value $line 'Caption'
                Left = Get-Value $line 'Left'
                Top = Get-Value $line 'Top'
                Width = Get-Value $line 'Width'
                Height = Get-Value $line 'Height'
                Origem = $target
            })
        }

        if ($line -match '(T(Label|Edit|DBEdit|DBLookupCombo|DBGrid|ComboBox|BitBtn|Button|MenuItem|MainMenu|Table|Query|DataSource|Panel|GroupBox|PageControl|TabSheet|CheckBox|RadioButton))\s+([A-Za-z0-9_]+)') {
            $componentType = $matches[1]
            $componentName = $matches[3]
            $controls.Add([pscustomobject]@{
                Executavel = $exeName
                Form = $currentForm
                Tipo = $componentType
                Nome = $componentName
                Caption = Get-Value $line 'Caption'
                Left = Get-Value $line 'Left'
                Top = Get-Value $line 'Top'
                Width = Get-Value $line 'Width'
                Height = Get-Value $line 'Height'
                DataSource = Get-Value $line 'DataSource'
                DataSet = Get-Value $line 'DataSet'
                FieldName = Get-Value $line 'FieldName'
                TableName = Get-Value $line 'TableName'
                DatabaseName = Get-Value $line 'DatabaseName'
                OnClick = Get-Value $line 'OnClick'
                OnKeyPress = Get-Value $line 'OnKeyPress'
                Raw = $line
            })
        }

        if ($line -match '(?i)(TableName|DatabaseName|FieldName|DataSource|DataSet|SQL|SELECT|FROM)') {
            $bindings.Add([pscustomobject]@{
                Executavel = $exeName
                Form = $currentForm
                TableName = Get-Value $line 'TableName'
                DatabaseName = Get-Value $line 'DatabaseName'
                FieldName = Get-Value $line 'FieldName'
                DataSource = Get-Value $line 'DataSource'
                DataSet = Get-Value $line 'DataSet'
                Texto = $line
            })
        }
    }
}

$forms | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $formsCsv -Encoding UTF8
$controls | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $controlsCsv -Encoding UTF8
$bindings | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $bindingsCsv -Encoding UTF8

Write-Output "Formularios: $($forms.Count) -> $formsCsv"
Write-Output "Controles: $($controls.Count) -> $controlsCsv"
Write-Output "Vinculos: $($bindings.Count) -> $bindingsCsv"

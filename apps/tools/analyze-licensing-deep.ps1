$ErrorActionPreference = 'Continue'

$modRoot = 'D:\AtelieProd\MOD'
$legacyRoot = 'D:\AtelieProd\Equipexe'
$runtimeRoot = Join-Path $modRoot 'apps\legacy-runtime\Equipexe'
$schemaColumns = Join-Path $modRoot 'docs\03-banco-de-dados\dicionario-paradox-colunas.csv'
$schemaTables = Join-Path $modRoot 'docs\03-banco-de-dados\dicionario-paradox-tabelas.csv'
$outDir = Join-Path $modRoot 'docs\09-licensing'
$authOutDir = Join-Path $modRoot 'docs\08-auth'

New-Item -ItemType Directory -Path $outDir -Force | Out-Null
New-Item -ItemType Directory -Path $authOutDir -Force | Out-Null

Add-Type -AssemblyName System.Data

function Get-PrintableStrings {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [int]$MinimumLength = 5
    )

    $bytes = [System.IO.File]::ReadAllBytes($Path)
    $strings = New-Object System.Collections.Generic.List[string]
    $buffer = New-Object System.Text.StringBuilder

    foreach ($byte in $bytes) {
        if ($byte -ge 32 -and $byte -le 126) {
            [void]$buffer.Append([char]$byte)
        } else {
            if ($buffer.Length -ge $MinimumLength) {
                $strings.Add($buffer.ToString())
            }
            [void]$buffer.Clear()
        }
    }

    if ($buffer.Length -ge $MinimumLength) {
        $strings.Add($buffer.ToString())
    }

    return $strings | Where-Object { $_ -match '[A-Za-z0-9]' } | Sort-Object -Unique
}

function Get-MaskedValue {
    param([object]$Value)

    if ($null -eq $Value -or [System.DBNull]::Value.Equals($Value)) {
        return ''
    }

    $text = [string]$Value
    if ($text.Length -eq 0) {
        return ''
    }

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($text)
        $hash = [System.BitConverter]::ToString($sha.ComputeHash($bytes)).Replace('-', '').Substring(0, 16)
    } finally {
        $sha.Dispose()
    }

    if ($text.Length -le 4) {
        return "len=$($text.Length);sha16=$hash;sample=****"
    }

    $prefix = $text.Substring(0, [Math]::Min(2, $text.Length))
    $suffix = $text.Substring([Math]::Max(0, $text.Length - 2))
    return "len=$($text.Length);sha16=$hash;sample=$prefix***$suffix"
}

function Get-Classification {
    param([string]$Text)

    if ($Text -match '(?i)NovoReg|Licen[cç]a|License|Licenciamento|Register|Registrar|Serial|Ativa[cç][aã]o|Activation|Valida[cç][aã]o|Vencimento|Bloq') {
        return 'licenciamento'
    }
    if ($Text -match '(?i)Senha|Password|Usuario|Usu[aá]rio|Permiss|Nivel|Grupo|Sess[aã]o|Session') {
        return 'autenticacao-permissao'
    }
    if ($Text -match '(?i)MacAddress|CodMaq|Maquina|M[aá]quina|Hardware|CPU|Mother|Disco|Volume|Computador|Estacao|Esta[cç][aã]o') {
        return 'hardware-binding'
    }
    if ($Text -match '(?i)http|https|ftp|WinINet|InternetOpen|URLDownload|Socket|Connect|191\.6\.218\.152|kinghost') {
        return 'comunicacao-remota'
    }
    if ($Text -match '(?i)Update|Atualiza|LiveUpdate|Vers[aã]o|Version') {
        return 'atualizacao'
    }
    return 'sinal-relacionado'
}

$licenseRegex = '(?i)licen[cç]|license|licenciamento|licen|serial|registro|registrar|register|novoreg|ativa[cç][aã]o|activation|valid|valida[cç][aã]o|chave|senha|password|usuario|usu[aá]rio|permiss|nivel|grupo|hardware|macaddress|codmaq|maquina|m[aá]quina|computador|estacao|esta[cç][aã]o|vencimento|vencer|bloq|bloque|bloqueia|controla|vers[aã]o|version|update|atualiza|wininet|url|http|https|191\.6\.218\.152|kinghost'

$targets = @(
    'LavSoft.exe',
    'LavFacilLan.exe',
    'Gerenciador.exe',
    'Financeiro.exe',
    'Estoque.exe',
    'NFE.exe',
    'SAT.exe',
    'Senhas.exe',
    'EquConfig.exe',
    'EquEstruAtu.exe',
    'LiveUpdate.exe',
    'Parametros.exe'
)

$stringSignals = New-Object System.Collections.Generic.List[object]
foreach ($targetName in $targets) {
    $path = Join-Path (Join-Path $runtimeRoot 'Exe') $targetName
    if (!(Test-Path -LiteralPath $path)) {
        $path = Join-Path (Join-Path $legacyRoot 'Exe') $targetName
    }
    if (!(Test-Path -LiteralPath $path)) {
        continue
    }

    try {
        $fileInfo = Get-Item -LiteralPath $path
        $matches = Get-PrintableStrings -Path $path -MinimumLength 5 |
            Where-Object { $_ -match $licenseRegex -and $_.Length -le 220 } |
            Select-Object -First 250

        foreach ($match in $matches) {
            $stringSignals.Add([pscustomobject]@{
                Executavel = $targetName
                Categoria = Get-Classification -Text $match
                Texto = $match
                Origem = $path
                TamanhoArquivo = $fileInfo.Length
                UltimaAlteracao = $fileInfo.LastWriteTime
            })
        }
    } catch {
        $stringSignals.Add([pscustomobject]@{
            Executavel = $targetName
            Categoria = 'erro'
            Texto = $_.Exception.Message
            Origem = $path
            TamanhoArquivo = ''
            UltimaAlteracao = ''
        })
    }
}

$stringOut = Join-Path $outDir 'sinais-licenciamento-executaveis.csv'
$stringSignals | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $stringOut -Encoding UTF8

$schemaSignals = New-Object System.Collections.Generic.List[object]
if (Test-Path -LiteralPath $schemaColumns) {
    Import-Csv -LiteralPath $schemaColumns |
        Where-Object {
            ($_.TableName -match $licenseRegex) -or
            ($_.ColumnName -match $licenseRegex)
        } |
        ForEach-Object {
            $text = "$($_.TableName).$($_.ColumnName)"
            $schemaSignals.Add([pscustomobject]@{
                Categoria = Get-Classification -Text $text
                TableName = $_.TableName
                ColumnName = $_.ColumnName
                RelativePath = $_.RelativePath
                DataType = $_.DataType
                ColumnSize = $_.ColumnSize
                Observacao = if ($_.TableName -match '(?i)^NovoReg|NovoReg') { 'forte candidato a registro/licenciamento' } elseif ($_.ColumnName -match '(?i)Licen[cç]a|NovoReg|Serial|Ativa|Venc|Bloq') { 'coluna candidata a regra de licença/ativação' } else { 'requer validação funcional' }
            })
        }
}

$schemaOut = Join-Path $outDir 'mapa-tabelas-licenciamento.csv'
$schemaSignals | Sort-Object Categoria, TableName, ColumnName | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $schemaOut -Encoding UTF8

$configSignals = New-Object System.Collections.Generic.List[object]
$configRoots = @($legacyRoot, $runtimeRoot) | Where-Object { Test-Path -LiteralPath $_ }
$configExtensions = @('.ini', '.xml', '.json', '.cfg', '.txt', '.bat', '.reg')
foreach ($root in $configRoots) {
    Get-ChildItem -LiteralPath $root -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $configExtensions -contains $_.Extension.ToLowerInvariant() -and $_.Length -le 2MB } |
        ForEach-Object {
            try {
                $lines = Get-Content -LiteralPath $_.FullName -ErrorAction Stop
                for ($i = 0; $i -lt $lines.Count; $i++) {
                    if ($lines[$i] -match $licenseRegex) {
                        $configSignals.Add([pscustomobject]@{
                            Arquivo = $_.FullName
                            Linha = $i + 1
                            Categoria = Get-Classification -Text $lines[$i]
                            Texto = $lines[$i]
                            TamanhoArquivo = $_.Length
                            UltimaAlteracao = $_.LastWriteTime
                        })
                    }
                }
            } catch {
                $configSignals.Add([pscustomobject]@{
                    Arquivo = $_.FullName
                    Linha = ''
                    Categoria = 'erro'
                    Texto = $_.Exception.Message
                    TamanhoArquivo = $_.Length
                    UltimaAlteracao = $_.LastWriteTime
                })
            }
        }
}

$configOut = Join-Path $outDir 'sinais-licenciamento-configs.csv'
$configSignals | Sort-Object Arquivo, Linha | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $configOut -Encoding UTF8

$candidateTables = @(
    @{ Directory = Join-Path $modRoot 'data\original-readonly\Equipexe\Ger\Dados'; Tables = @('NovoReg', 'NovoReg.BD', 'NovoReg.db', 'NovoRegBD', 'Usuarios', 'Senhas', 'Nivel') },
    @{ Directory = Join-Path $modRoot 'data\original-readonly\Equipexe\Ger\Filial'; Tables = @('NovoReg') },
    @{ Directory = Join-Path $modRoot 'data\original-readonly\Equipexe\Lav\FILIAL'; Tables = @('NovoRegLavFilial') }
)

$samples = New-Object System.Collections.Generic.List[object]
foreach ($group in $candidateTables) {
    $directory = $group.Directory
    if (!(Test-Path -LiteralPath $directory)) {
        continue
    }

    $connectionString = "Driver={Microsoft Paradox Driver (*.db )};Dbq=$directory;DefaultDir=$directory;DriverID=538;FIL=Paradox 5.X;"
    foreach ($table in $group.Tables) {
        try {
            $connection = New-Object System.Data.Odbc.OdbcConnection($connectionString)
            $connection.Open()
            $command = $connection.CreateCommand()
            $command.CommandText = "SELECT * FROM [$table]"
            $adapter = New-Object System.Data.Odbc.OdbcDataAdapter($command)
            $data = New-Object System.Data.DataTable
            [void]$adapter.Fill($data)

            $rowIndex = 0
            foreach ($row in $data.Rows) {
                $rowIndex++
                if ($rowIndex -gt 10) {
                    break
                }
                foreach ($column in $data.Columns) {
                    $samples.Add([pscustomobject]@{
                        Diretorio = $directory
                        Tabela = $table
                        Linha = $rowIndex
                        Coluna = $column.ColumnName
                        Categoria = Get-Classification -Text "$table.$($column.ColumnName)"
                        ValorMascarado = Get-MaskedValue -Value $row[$column.ColumnName]
                    })
                }
            }

            if ($data.Rows.Count -eq 0) {
                $samples.Add([pscustomobject]@{
                    Diretorio = $directory
                    Tabela = $table
                    Linha = 0
                    Coluna = ''
                    Categoria = 'sem-registros'
                    ValorMascarado = 'tabela acessivel sem registros'
                })
            }
        } catch {
            $samples.Add([pscustomobject]@{
                Diretorio = $directory
                Tabela = $table
                Linha = ''
                Coluna = ''
                Categoria = 'erro-leitura'
                ValorMascarado = $_.Exception.Message
            })
        } finally {
            if ($adapter) { $adapter.Dispose() }
            if ($command) { $command.Dispose() }
            if ($connection) { $connection.Dispose() }
        }
    }
}

$samplesOut = Join-Path $outDir 'amostras-mascaradas-tabelas-licenciamento-auth.csv'
$samples | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $samplesOut -Encoding UTF8

$summary = [pscustomobject]@{
    GeradoEm = Get-Date
    SinaisExecutaveis = $stringSignals.Count
    SinaisSchema = $schemaSignals.Count
    SinaisConfigs = $configSignals.Count
    AmostrasMascaradas = $samples.Count
    ExecutaveisComSinais = ($stringSignals | Select-Object -ExpandProperty Executavel -Unique | Sort-Object) -join '; '
    TabelasFortes = ($schemaSignals | Where-Object { $_.Observacao -match 'forte candidato' } | Select-Object -ExpandProperty TableName -Unique | Sort-Object) -join '; '
}

$summaryOut = Join-Path $outDir 'resumo-licenciamento-profundo.csv'
$summary | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $summaryOut -Encoding UTF8

Write-Output "Sinais executaveis: $($stringSignals.Count)"
Write-Output "Sinais schema: $($schemaSignals.Count)"
Write-Output "Sinais configs: $($configSignals.Count)"
Write-Output "Amostras mascaradas: $($samples.Count)"
Write-Output "Saidas:"
Write-Output " - $stringOut"
Write-Output " - $schemaOut"
Write-Output " - $configOut"
Write-Output " - $samplesOut"
Write-Output " - $summaryOut"

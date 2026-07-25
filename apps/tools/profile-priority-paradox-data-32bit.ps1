$ErrorActionPreference = 'Continue'

$root = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe'
$outDir = 'D:\AtelieProd\MOD\docs\10-database'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

Add-Type -AssemblyName System.Data

$priority = @(
    @{ Rel = 'Ger\Dados'; Tables = @('Clientes', 'CliContato', 'ClientesObs', 'FunCli', 'FunCliRou', 'GruClientes', 'Usuarios', 'Senhas', 'Nivel') },
    @{ Rel = 'Lav\FILIAL'; Tables = @('MovCab', 'MovLocRol', 'ControleEti', 'IndenRol', 'CliCredito', 'FecCaixa', 'MovIniCaixa', 'Notas', 'NotaFisPag', 'MovEstLan') },
    @{ Rel = 'Lav\Dados'; Tables = @('CadLocRol') },
    @{ Rel = 'REC\FILIAL'; Tables = @('Duplicat', 'Boletos', 'DupBoleto') },
    @{ Rel = 'EST\DADOS'; Tables = @('ProdEst', 'TabProdEst', 'ProdEstKit', 'ProdEstPac') },
    @{ Rel = 'EST\FILIAL'; Tables = @('MovEst', 'MovEstCan', 'MovEstEnc') },
    @{ Rel = 'Ger\Filial'; Tables = @('Produt', 'MovControle') },
    @{ Rel = 'SAT\FILIAL'; Tables = @('NotaSat', 'NotaSatCanc', 'MovSatCli', 'MovSatCliOcor', 'MovSatFor', 'MovSatForOcor', 'MovSatInt', 'MovSatIntOcor', 'Anotacoes') },
    @{ Rel = 'PAG\FILIAL'; Tables = @('Titulos', 'TitGru', 'TitGruLan') }
)

function Classify-Column {
    param([string]$Name)
    if ($Name -match '(?i)status|situ|sit|posicao|cancel|ativo|bloq|entreg|pago|fech|baix|liber|aprov|baixa') { return 'status' }
    if ($Name -match '(?i)valor|val|vlr|preco|pre[cç]o|total|tot|subtotal|desc|acresc|juros|multa|custo|credito|debito|saldo|pago|pagto|iss|base') { return 'valor' }
    if ($Name -match '(?i)data|dat|dt|venc|ven|emi|emiss|cad|alt|lan|pag|can|fecha|abert|ent') { return 'data' }
    if ($Name -match '(?i)qtd|qde|qtde|quant|peso|volume|pecas|pe[cç]as') { return 'quantidade' }
    if ($Name -match '(?i)codcli|cliente|nomcli|grucli') { return 'cliente' }
    if ($Name -match '(?i)codpro|produto|proest|marca|modelo|unid|fornec|codest') { return 'produto' }
    if ($Name -match '(?i)usuario|usu|operador|atendente|tecnico|respons|vendedor|codven') { return 'usuario' }
    return 'outro'
}

function Convert-DecimalSafe {
    param([object]$Value)
    if ($null -eq $Value -or [DBNull]::Value.Equals($Value)) { return $null }
    $text = ([string]$Value).Trim()
    if ($text.Length -eq 0) { return $null }
    $number = 0.0
    if ([double]::TryParse($text, [Globalization.NumberStyles]::Any, [Globalization.CultureInfo]::InvariantCulture, [ref]$number)) { return $number }
    if ([double]::TryParse($text, [Globalization.NumberStyles]::Any, [Globalization.CultureInfo]::GetCultureInfo('pt-BR'), [ref]$number)) { return $number }
    return $null
}

$rowCounts = New-Object System.Collections.Generic.List[object]
$statusValues = New-Object System.Collections.Generic.List[object]
$valueStats = New-Object System.Collections.Generic.List[object]
$dateStats = New-Object System.Collections.Generic.List[object]
$fieldPresence = New-Object System.Collections.Generic.List[object]
$failures = New-Object System.Collections.Generic.List[object]

foreach ($group in $priority) {
    $dir = Join-Path $root $group.Rel
    if (!(Test-Path -LiteralPath $dir)) { continue }
    $connectionString = "Driver={Microsoft Paradox Driver (*.db )};Dbq=$dir;DefaultDir=$dir;DriverID=538;FIL=Paradox 5.X;"

    foreach ($table in $group.Tables) {
        $dbPath = Join-Path $dir "$table.DB"
        if (!(Test-Path -LiteralPath $dbPath)) {
            $failures.Add([pscustomobject]@{ Directory=$dir; Table=$table; Error='arquivo DB nao encontrado' })
            continue
        }

        try {
            $connection = New-Object System.Data.Odbc.OdbcConnection($connectionString)
            $connection.Open()
            $command = $connection.CreateCommand()
            $command.CommandText = "SELECT * FROM [$table]"
            $adapter = New-Object System.Data.Odbc.OdbcDataAdapter($command)
            $data = New-Object System.Data.DataTable
            [void]$adapter.Fill($data)

            $rowCounts.Add([pscustomobject]@{
                Directory = $dir
                RelativeDirectory = $group.Rel
                Table = $table
                Rows = $data.Rows.Count
                Columns = $data.Columns.Count
                FileLength = (Get-Item -LiteralPath $dbPath).Length
            })

            foreach ($column in $data.Columns) {
                $class = Classify-Column $column.ColumnName
                $nonEmpty = 0
                $distinct = New-Object 'System.Collections.Generic.Dictionary[string,int]'
                $numericValues = New-Object System.Collections.Generic.List[double]
                $dateValues = New-Object System.Collections.Generic.List[datetime]

                foreach ($row in $data.Rows) {
                    $value = $row[$column.ColumnName]
                    if ($null -eq $value -or [DBNull]::Value.Equals($value)) { continue }
                    $text = ([string]$value).Trim()
                    if ($text.Length -eq 0) { continue }
                    $nonEmpty++
                    if ($distinct.Count -lt 200 -or $distinct.ContainsKey($text)) {
                        if ($distinct.ContainsKey($text)) { $distinct[$text]++ } else { $distinct[$text] = 1 }
                    }

                    if ($class -eq 'valor' -or $class -eq 'quantidade') {
                        $num = Convert-DecimalSafe $value
                        if ($null -ne $num) { $numericValues.Add([double]$num) }
                    }
                    if ($class -eq 'data') {
                        $dt = [datetime]::MinValue
                        if ([datetime]::TryParse($text, [ref]$dt)) { $dateValues.Add($dt) }
                    }
                }

                $fieldPresence.Add([pscustomobject]@{
                    Directory = $dir
                    Table = $table
                    Field = $column.ColumnName
                    Class = $class
                    DataType = $column.DataType.FullName
                    NonEmpty = $nonEmpty
                    DistinctTracked = $distinct.Count
                    Evidence = 'confirmado por schema + ocorrencia direta readonly'
                })

                if ($class -eq 'status') {
                    $top = $distinct.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 25
                    foreach ($item in $top) {
                        $statusValues.Add([pscustomobject]@{
                            Directory = $dir
                            Table = $table
                            Field = $column.ColumnName
                            Value = $item.Key
                            Count = $item.Value
                            Meaning = 'nao confirmado'
                            Evidence = 'ocorrencia direta readonly; significado exige UI/runtime'
                        })
                    }
                }

                if (($class -eq 'valor' -or $class -eq 'quantidade') -and $numericValues.Count -gt 0) {
                    $sum = 0.0
                    foreach ($n in $numericValues) { $sum += $n }
                    $sorted = $numericValues | Sort-Object
                    $valueStats.Add([pscustomobject]@{
                        Directory = $dir
                        Table = $table
                        Field = $column.ColumnName
                        Class = $class
                        Count = $numericValues.Count
                        Min = $sorted[0]
                        Max = $sorted[$sorted.Count - 1]
                        Sum = $sum
                        Evidence = 'ocorrencia direta readonly; formula/calculo exige runtime'
                    })
                }

                if ($class -eq 'data' -and $dateValues.Count -gt 0) {
                    $sortedDates = $dateValues | Sort-Object
                    $dateStats.Add([pscustomobject]@{
                        Directory = $dir
                        Table = $table
                        Field = $column.ColumnName
                        Count = $dateValues.Count
                        MinDate = $sortedDates[0]
                        MaxDate = $sortedDates[$sortedDates.Count - 1]
                        Evidence = 'ocorrencia direta readonly'
                    })
                }
            }
        }
        catch {
            $failures.Add([pscustomobject]@{ Directory=$dir; Table=$table; Error=$_.Exception.Message })
        }
        finally {
            if ($adapter) { $adapter.Dispose() }
            if ($command) { $command.Dispose() }
            if ($connection) { $connection.Dispose() }
        }
    }
}

$rowCounts | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath (Join-Path $outDir 'perfil-tabelas-prioritarias-linhas.csv') -Encoding UTF8
$statusValues | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath (Join-Path $outDir 'perfil-status-valores-distintos.csv') -Encoding UTF8
$valueStats | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath (Join-Path $outDir 'perfil-valores-monetarios-estatisticas.csv') -Encoding UTF8
$dateStats | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath (Join-Path $outDir 'perfil-datas-faixas.csv') -Encoding UTF8
$fieldPresence | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath (Join-Path $outDir 'perfil-campos-presenca.csv') -Encoding UTF8
$failures | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath (Join-Path $outDir 'perfil-tabelas-prioritarias-falhas.csv') -Encoding UTF8

Write-Output "Tabelas perfiladas: $($rowCounts.Count)"
Write-Output "Status distintos: $($statusValues.Count)"
Write-Output "Estatisticas valor/qtd: $($valueStats.Count)"
Write-Output "Faixas de datas: $($dateStats.Count)"
Write-Output "Campos perfilados: $($fieldPresence.Count)"
Write-Output "Falhas: $($failures.Count)"

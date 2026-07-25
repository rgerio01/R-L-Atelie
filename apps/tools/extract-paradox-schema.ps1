$ErrorActionPreference = 'Continue'

$root = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe'
$outDir = 'D:\AtelieProd\MOD\docs\03-banco-de-dados'
$schemaOut = Join-Path $outDir 'dicionario-paradox-colunas.csv'
$tableOut = Join-Path $outDir 'dicionario-paradox-tabelas.csv'
$failOut = Join-Path $outDir 'dicionario-paradox-falhas.csv'

New-Item -ItemType Directory -Path $outDir -Force | Out-Null

Add-Type -AssemblyName System.Data

$tables = New-Object System.Collections.Generic.List[object]
$columns = New-Object System.Collections.Generic.List[object]
$failures = New-Object System.Collections.Generic.List[object]

Get-ChildItem -LiteralPath $root -Recurse -Force -File -Filter '*.DB' -ErrorAction SilentlyContinue |
    Sort-Object FullName |
    ForEach-Object {
        $file = $_
        $directory = $file.DirectoryName
        $tableName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
        $relativePath = $file.FullName.Substring($root.Length).TrimStart('\')
        $connectionString = "Driver={Microsoft Paradox Driver (*.db )};Dbq=$directory;DefaultDir=$directory;DriverID=538;FIL=Paradox 5.X;"

        try {
            $connection = New-Object System.Data.Odbc.OdbcConnection($connectionString)
            $connection.Open()
            $command = $connection.CreateCommand()
            $command.CommandText = "SELECT * FROM [$tableName] WHERE 1 = 0"
            $reader = $command.ExecuteReader()
            $schema = $reader.GetSchemaTable()
            $reader.Close()
            $connection.Close()

            $tables.Add([pscustomobject]@{
                TableName = $tableName
                RelativePath = $relativePath
                Directory = $directory
                Length = $file.Length
                ColumnCount = if ($schema) { $schema.Rows.Count } else { 0 }
                Extracted = $true
                ExtractedAt = Get-Date
            })

            foreach ($row in $schema.Rows) {
                $columns.Add([pscustomobject]@{
                    TableName = $tableName
                    RelativePath = $relativePath
                    ColumnName = $row['ColumnName']
                    Ordinal = $row['ColumnOrdinal']
                    TypeName = $row['DataTypeName']
                    DataType = $row['DataType']
                    ColumnSize = $row['ColumnSize']
                    DecimalDigits = $row['NumericScale']
                    Nullable = $row['AllowDBNull']
                    DefaultValue = ''
                })
            }
        }
        catch {
            $failures.Add([pscustomobject]@{
                TableName = $tableName
                RelativePath = $relativePath
                Directory = $directory
                Length = $file.Length
                Error = $_.Exception.Message
                ExtractedAt = Get-Date
            })
        }
        finally {
            if ($reader) {
                $reader.Dispose()
            }
            if ($command) {
                $command.Dispose()
            }
            if ($connection) {
                $connection.Dispose()
            }
        }
    }

$tables | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $tableOut -Encoding UTF8
$columns | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $schemaOut -Encoding UTF8
$failures | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $failOut -Encoding UTF8

Write-Output "Tabelas extraidas: $($tables.Count)"
Write-Output "Colunas extraidas: $($columns.Count)"
Write-Output "Falhas: $($failures.Count)"
Write-Output "Saida: $schemaOut"

$ErrorActionPreference = 'Continue'

$directory = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe\Ger\Dados'
$outDir = 'D:\AtelieProd\MOD\docs\04-autenticacao-permissoes'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

Add-Type -AssemblyName System.Data

$connectionString = "Driver={Microsoft Paradox Driver (*.db )};Dbq=$directory;DefaultDir=$directory;DriverID=538;FIL=Paradox 5.X;"
$tables = @('Usuarios', 'Senhas', 'UsuaSis', 'UsuaFil', 'UsuaGru', 'UsuaGruInt', 'GruUsuarios', 'Nivel')

foreach ($table in $tables) {
    try {
        $connection = New-Object System.Data.Odbc.OdbcConnection($connectionString)
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT * FROM [$table]"
        $adapter = New-Object System.Data.Odbc.OdbcDataAdapter($command)
        $data = New-Object System.Data.DataTable
        [void]$adapter.Fill($data)

        foreach ($row in $data.Rows) {
            foreach ($column in $data.Columns) {
                if ($column.ColumnName -match '(?i)senha|password') {
                    $value = [string]$row[$column.ColumnName]
                    if ($value.Length -gt 0) {
                        $row[$column.ColumnName] = ('*' * [Math]::Min($value.Length, 10))
                    }
                }
            }
        }

        $data | ConvertTo-Csv -NoTypeInformation |
            Out-File -LiteralPath (Join-Path $outDir "amostra-legado-$table.csv") -Encoding UTF8

        [pscustomobject]@{
            Table = $table
            Rows = $data.Rows.Count
            Columns = $data.Columns.Count
            Output = Join-Path $outDir "amostra-legado-$table.csv"
        }
    }
    catch {
        [pscustomobject]@{
            Table = $table
            Rows = $null
            Columns = $null
            Output = $_.Exception.Message
        }
    }
    finally {
        if ($adapter) { $adapter.Dispose() }
        if ($command) { $command.Dispose() }
        if ($connection) { $connection.Dispose() }
    }
}

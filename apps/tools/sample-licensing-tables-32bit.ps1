$ErrorActionPreference = 'Continue'

$out = 'D:\AtelieProd\MOD\docs\09-licensing\amostras-mascaradas-tabelas-licenciamento-auth.csv'
$rows = New-Object System.Collections.Generic.List[object]

Add-Type -AssemblyName System.Data

function Mask-Value {
    param([object]$Value)

    if ($null -eq $Value -or [DBNull]::Value.Equals($Value)) {
        return ''
    }

    $text = [string]$Value
    if ($text.Length -eq 0) {
        return ''
    }

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $hash = [BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($text))).Replace('-', '').Substring(0, 16)
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

$groups = @(
    @{
        Dir = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe\Ger\Dados'
        Tabs = @('NovoReg', 'NovoReg.BD', 'NovoReg.db', 'NovoRegBD', 'Usuarios', 'Senhas', 'Nivel', 'CadCart')
    },
    @{
        Dir = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe\Ger\Filial'
        Tabs = @('NovoReg')
    },
    @{
        Dir = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe\Lav\FILIAL'
        Tabs = @('NovoRegLavFilial')
    }
)

foreach ($group in $groups) {
    if (!(Test-Path -LiteralPath $group.Dir)) {
        continue
    }

    $connectionString = "Driver={Microsoft Paradox Driver (*.db )};Dbq=$($group.Dir);DefaultDir=$($group.Dir);DriverID=538;FIL=Paradox 5.X;"
    foreach ($table in $group.Tabs) {
        try {
            $connection = New-Object System.Data.Odbc.OdbcConnection($connectionString)
            $connection.Open()
            $command = $connection.CreateCommand()
            $command.CommandText = "SELECT * FROM [$table]"
            $adapter = New-Object System.Data.Odbc.OdbcDataAdapter($command)
            $data = New-Object System.Data.DataTable
            [void]$adapter.Fill($data)

            $index = 0
            foreach ($row in $data.Rows) {
                $index++
                if ($index -gt 5) {
                    break
                }

                foreach ($column in $data.Columns) {
                    $rows.Add([pscustomobject]@{
                        Diretorio = $group.Dir
                        Tabela = $table
                        Linha = $index
                        Coluna = $column.ColumnName
                        ValorMascarado = Mask-Value $row[$column.ColumnName]
                    })
                }
            }

            if ($data.Rows.Count -eq 0) {
                $rows.Add([pscustomobject]@{
                    Diretorio = $group.Dir
                    Tabela = $table
                    Linha = 0
                    Coluna = ''
                    ValorMascarado = 'tabela acessivel sem registros'
                })
            }
        } catch {
            $rows.Add([pscustomobject]@{
                Diretorio = $group.Dir
                Tabela = $table
                Linha = ''
                Coluna = ''
                ValorMascarado = $_.Exception.Message
            })
        } finally {
            if ($adapter) { $adapter.Dispose() }
            if ($command) { $command.Dispose() }
            if ($connection) { $connection.Dispose() }
        }
    }
}

$rows | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $out -Encoding UTF8
Write-Output "Amostras: $($rows.Count) -> $out"

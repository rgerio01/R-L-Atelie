$ErrorActionPreference = 'Stop'

$directory = 'D:\AtelieProd\MOD\data\original-readonly\Equipexe\Ger\Dados'
$outDir = 'D:\AtelieProd\MOD\logs\analysis\restricted'
$summaryOut = 'D:\AtelieProd\MOD\docs\04-autenticacao-permissoes\analise-codificacao-senha-legado.md'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path $summaryOut -Parent) -Force | Out-Null

Add-Type -AssemblyName System.Data

function Shift-Text {
    param(
        [string]$Text,
        [int]$Delta
    )

    $chars = foreach ($ch in $Text.ToCharArray()) {
        [char](([int][char]$ch) + $Delta)
    }
    return -join $chars
}

function Shift-Digits {
    param(
        [string]$Text,
        [int]$Delta
    )

    $chars = foreach ($ch in $Text.ToCharArray()) {
        if ($ch -ge '0' -and $ch -le '9') {
            $n = ([int][char]$ch - [int][char]'0' + $Delta) % 10
            if ($n -lt 0) { $n += 10 }
            [char]([int][char]'0' + $n)
        } else {
            $ch
        }
    }
    return -join $chars
}

$connectionString = "Driver={Microsoft Paradox Driver (*.db )};Dbq=$directory;DefaultDir=$directory;DriverID=538;FIL=Paradox 5.X;"
$connection = New-Object System.Data.Odbc.OdbcConnection($connectionString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = 'SELECT CodUsuario, NomUsuario, GruUsuario, TipUsuario, Senha, Cancelado FROM [Usuarios]'
$adapter = New-Object System.Data.Odbc.OdbcDataAdapter($command)
$data = New-Object System.Data.DataTable
[void]$adapter.Fill($data)
$connection.Close()

$analysis = foreach ($row in $data.Rows) {
    $encoded = [string]$row['Senha']
    [pscustomobject]@{
        CodUsuario = [string]$row['CodUsuario']
        GruUsuario = [string]$row['GruUsuario']
        TipUsuario = [string]$row['TipUsuario']
        Cancelado = [string]$row['Cancelado']
        EncodedLength = $encoded.Length
        EncodedCharCodes = (($encoded.ToCharArray() | ForEach-Object { [int][char]$_ }) -join ' ')
        ShiftMinus1Ascii = Shift-Text $encoded -1
        ShiftMinus2Ascii = Shift-Text $encoded -2
        ShiftMinus3Ascii = Shift-Text $encoded -3
        ShiftMinus1Digits = Shift-Digits $encoded -1
        ShiftPlus1Digits = Shift-Digits $encoded 1
        Reversed = -join ($encoded.ToCharArray()[($encoded.Length - 1)..0])
    }
}

$restrictedOut = Join-Path $outDir 'analise-codificacao-senha-legado-restrito.csv'
$analysis | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $restrictedOut -Encoding UTF8

$lengths = $analysis | Group-Object EncodedLength | Sort-Object Name | ForEach-Object { "- tamanho $($_.Name): $($_.Count) usuario(s)" }
$hasNumericOnly = ($analysis | Where-Object { $_.EncodedCharCodes -match '^(?:\\d+\\s*)+$' }).Count

@"
# Analise de Codificacao de Senha Legado

## Escopo

Analise realizada somente sobre a copia readonly:

`D:\AtelieProd\MOD\data\original-readonly\Equipexe\Ger\Dados\Usuarios.DB`

Nenhuma alteracao foi feita no legado.

## Resultado preliminar

Foram analisados $($analysis.Count) usuarios da tabela `Usuarios.DB`.

Distribuicao de tamanho do campo `Senha`:

$($lengths -join "`n")

O campo `Senha` tem tamanho fisico 10 no schema, mas os registros ativos observados usam 5 caracteres preenchidos.

## Hipotese em investigacao

Foi gerado arquivo restrito com testes de deslocamento:

`logs\analysis\restricted\analise-codificacao-senha-legado-restrito.csv`

O arquivo inclui variacoes como:

- caractere ASCII -1;
- caractere ASCII -2;
- caractere ASCII -3;
- digito -1 circular;
- digito +1 circular;
- reversao da string.

Esta abordagem foi criada porque ha lembranca operacional de regra do tipo `1 vira 2`, `2 vira 3`, indicando possivel cifra simples por deslocamento.

## Cuidado operacional

Mesmo que a codificacao seja confirmada, a alteracao de senha no legado nao deve ser feita direto no original. O caminho seguro e testar primeiro em copia/homologacao, porque o acesso tambem depende de `UsuaSis.DB`, `Nivel.DB` e possivelmente `Senhas.exe`.
"@ | Out-File -LiteralPath $summaryOut -Encoding UTF8

Write-Output "Resumo: $summaryOut"
Write-Output "Restrito: $restrictedOut"

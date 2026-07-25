$ErrorActionPreference = 'Stop'

$targets = @(
    'D:\AtelieProd\Equipexe\Exe\LavSoft.exe',
    'D:\AtelieProd\Equipexe\Exe\LavFacilLan.exe',
    'D:\AtelieProd\Equipexe\Exe\Gerenciador.exe',
    'D:\AtelieProd\Equipexe\Exe\Financeiro.exe',
    'D:\AtelieProd\Equipexe\Exe\Estoque.exe',
    'D:\AtelieProd\Equipexe\Exe\NFE.exe',
    'D:\AtelieProd\Equipexe\Exe\SAT.exe'
)

$outDir = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\strings-executaveis'
$mapOut = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-telas-menus-relatorios-inicial.csv'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

function Get-PrintableStrings {
    param(
        [byte[]]$Bytes,
        [int]$MinimumLength = 4
    )

    $results = New-Object System.Collections.Generic.List[string]
    $buffer = New-Object System.Text.StringBuilder

    foreach ($byte in $Bytes) {
        if ($byte -ge 32 -and $byte -le 126) {
            [void]$buffer.Append([char]$byte)
        } else {
            if ($buffer.Length -ge $MinimumLength) {
                $results.Add($buffer.ToString())
            }
            [void]$buffer.Clear()
        }
    }

    if ($buffer.Length -ge $MinimumLength) {
        $results.Add($buffer.ToString())
    }

    return $results
}

$keywords = 'menu|relat|impress|cadastro|consulta|financeiro|estoque|cliente|usuario|usu[aá]rio|senha|permiss|nfe|nota|sat|caixa|recep|lavagem|rol|peca|pe[cç]a|produto|servi[cç]o|backup|sincron|param|config|cancel|desconto|vendedor|entrega|romaneio|comanda'
$map = New-Object System.Collections.Generic.List[object]

foreach ($target in $targets) {
    if (!(Test-Path -LiteralPath $target)) {
        continue
    }

    $name = [System.IO.Path]::GetFileNameWithoutExtension($target)
    $bytes = [System.IO.File]::ReadAllBytes($target)
    $strings = Get-PrintableStrings -Bytes $bytes -MinimumLength 5 |
        Where-Object { $_ -match '[A-Za-z]' } |
        Sort-Object -Unique

    $stringFile = Join-Path $outDir "$name.strings.txt"
    $strings | Out-File -LiteralPath $stringFile -Encoding UTF8

    $strings |
        Where-Object { $_ -match $keywords } |
        Select-Object -First 500 |
        ForEach-Object {
            $category = if ($_ -match '(?i)relat|impress') { 'relatorio/impressao' }
                elseif ($_ -match '(?i)menu') { 'menu' }
                elseif ($_ -match '(?i)senha|usuario|usu[aá]rio|permiss') { 'autenticacao/permissao' }
                elseif ($_ -match '(?i)nfe|nota|sat') { 'fiscal' }
                elseif ($_ -match '(?i)cadastro|cliente|produto|servi[cç]o') { 'cadastro' }
                elseif ($_ -match '(?i)financeiro|caixa|desconto') { 'financeiro' }
                elseif ($_ -match '(?i)estoque|peca|pe[cç]a|lavagem|rol') { 'operacional' }
                else { 'outro' }

            $map.Add([pscustomobject]@{
                Executavel = $name
                Categoria = $category
                Texto = $_
                Origem = $target
            })
        }
}

$map | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $mapOut -Encoding UTF8
Write-Output "Mapa inicial: $mapOut"

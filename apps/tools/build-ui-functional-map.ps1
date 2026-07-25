$ErrorActionPreference = 'Stop'

$root = 'D:\AtelieProd\Equipexe'
$outDir = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-funcional-telas'
$summaryCsv = Join-Path $outDir 'mapa-funcional-executaveis.csv'
$correlationCsv = Join-Path $outDir 'correlacao-menu-permissao-executavel.csv'
$reportMd = Join-Path $outDir 'relatorio-mapa-funcional-telas.md'
$permissionsCsv = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\menus-permissoes-niveldb.csv'

New-Item -ItemType Directory -Path $outDir -Force | Out-Null

$targets = @(
    [pscustomobject]@{ Name = 'LavSoft'; Path = 'D:\AtelieProd\Equipexe\Exe\LavSoft.exe' },
    [pscustomobject]@{ Name = 'LavFacilLan'; Path = 'D:\AtelieProd\Equipexe\Exe\LavFacilLan.exe' },
    [pscustomobject]@{ Name = 'Gerenciador'; Path = 'D:\AtelieProd\Equipexe\Exe\Gerenciador.exe' },
    [pscustomobject]@{ Name = 'Financeiro'; Path = 'D:\AtelieProd\Equipexe\Exe\Financeiro.exe' },
    [pscustomobject]@{ Name = 'Estoque'; Path = 'D:\AtelieProd\Equipexe\Exe\Estoque.exe' },
    [pscustomobject]@{ Name = 'NFE'; Path = 'D:\AtelieProd\Equipexe\Exe\NFE.exe' },
    [pscustomobject]@{ Name = 'SAT'; Path = 'D:\AtelieProd\Equipexe\Exe\SAT.exe' }
)

function Get-AsciiStrings {
    param([byte[]]$Bytes, [int]$Min = 4)
    $text = [System.Text.Encoding]::Latin1.GetString($Bytes)
    return [regex]::Matches($text, "[ -~]{$Min,}") | ForEach-Object { $_.Value }
}

function Get-Utf16Strings {
    param([byte[]]$Bytes, [int]$Min = 4)
    if ($Bytes.Length -gt 12000000) { return @() }
    $text = [System.Text.Encoding]::Unicode.GetString($Bytes)
    return [regex]::Matches($text, "[ -~]{$Min,}") | ForEach-Object { $_.Value }
}

function Normalize-Text {
    param([string]$Text)
    if ([string]::IsNullOrWhiteSpace($Text)) { return '' }
    $value = $Text -replace '&', ''
    $value = $value -replace '\s+', ' '
    return $value.Trim()
}

function Get-Category {
    param([string]$Text)
    $t = $Text.ToLowerInvariant()
    if ($t -match 'relat|movimento|analit|sintet|conferencia|previs|comiss|frequ|extrato|recibo|carta|impress') { return 'relatorio/impressao' }
    if ($t -match 'cliente|usuario|filial|matriz|parametro|tabela|cadastro|forma|condicao|servico|tecido|cor|marca|defeito|feriado|grupo|produto') { return 'cadastro' }
    if ($t -match 'caixa|finance|pagamento|credito|cobranca|fatura|desconto|devolu|sangria|fechamento') { return 'financeiro/caixa' }
    if ($t -match 'nfe|nfse|nota fiscal|sat|cupom|fiscal|leitura x|reducao|memoria fiscal') { return 'fiscal' }
    if ($t -match 'rol|entrada|entrega|lavagem|passadoria|terceir|localizacao|peca|expedicao|estoque') { return 'operacional' }
    if ($t -match 'senha|login|permiss|nivel|bloq|usuario') { return 'autenticacao/permissao' }
    if ($t -match 'update|atualiza|sincron|nuvem|download|upload|email|conexao|servidor|host|http|socket') { return 'comunicacao/update' }
    return 'outro'
}

function Looks-Useful {
    param([string]$Text)
    if ([string]::IsNullOrWhiteSpace($Text)) { return $false }
    if ($Text.Length -lt 4 -or $Text.Length -gt 140) { return $false }
    if ($Text -match '^[A-Za-z0-9+/=]{40,}$') { return $false }
    if ($Text -match '^[0-9A-Fa-f]{16,}$') { return $false }
    if ($Text -match '[\x00-\x08\x0B\x0C\x0E-\x1F]') { return $false }
    return ($Text -match '(?i)(cadastro|cliente|usuario|senha|nivel|permiss|entrada|rol|entrega|pagamento|caixa|finance|estoque|produto|servico|relat|movimento|conferencia|fiscal|cupom|nfe|nfse|sat|nota|parametro|filial|matriz|preco|desconto|devolu|cobranca|fatur|lavagem|passadoria|terceir|localizacao|update|atualiza|sincron|nuvem|email|servidor|conexao|impress)')
}

$rows = New-Object System.Collections.Generic.List[object]

foreach ($target in $targets) {
    if (!(Test-Path -LiteralPath $target.Path)) { continue }
    $bytes = [System.IO.File]::ReadAllBytes($target.Path)
    $strings = @(Get-AsciiStrings -Bytes $bytes -Min 4) + @(Get-Utf16Strings -Bytes $bytes -Min 4)
    $strings |
        ForEach-Object { Normalize-Text $_ } |
        Where-Object { Looks-Useful $_ } |
        Sort-Object -Unique |
        ForEach-Object {
            $rows.Add([pscustomobject]@{
                Executavel = $target.Name
                Categoria = Get-Category $_
                Texto = $_
                StatusLayout = 'texto_estatico_sem_posicao'
                ProximaValidacao = 'captura_dinamica_MOD'
            })
        }
}

$rows | Sort-Object Executavel,Categoria,Texto | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $summaryCsv -Encoding UTF8

$correlations = New-Object System.Collections.Generic.List[object]
if (Test-Path -LiteralPath $permissionsCsv) {
    $permissions = Import-Csv -LiteralPath $permissionsCsv | Where-Object { $_.Op }
    $indexedRows = $rows | ForEach-Object {
        [pscustomobject]@{
            Executavel = $_.Executavel
            Categoria = $_.Categoria
            Texto = $_.Texto
            Normalizado = (($_.Texto -replace '[^A-Za-z0-9]','').ToLowerInvariant())
        }
    }
    foreach ($perm in $permissions) {
        $op = [string]$perm.Op
        $token = (($op -replace '\d+$','' -replace '[^A-Za-z0-9]','').ToLowerInvariant())
        if ($token.Length -lt 4) { continue }
        $matches = $indexedRows | Where-Object { $_.Normalizado.Contains($token) } | Select-Object -First 5
        if ($matches) {
            foreach ($match in $matches) {
                $correlations.Add([pscustomobject]@{
                    Sistema = $perm.CodSistema
                    OperacaoPermissao = $op
                    Executavel = $match.Executavel
                    Categoria = $match.Categoria
                    TextoEncontrado = $match.Texto
                    Confianca = 'media'
                    Observacao = 'correlacao por nome/texto; exige validacao visual'
                })
            }
        }
    }
}

$correlations | Sort-Object Sistema,OperacaoPermissao,Executavel,TextoEncontrado | ConvertTo-Csv -NoTypeInformation | Out-File -LiteralPath $correlationCsv -Encoding UTF8

$byExe = $rows | Group-Object Executavel | Sort-Object Name
$byCat = $rows | Group-Object Categoria | Sort-Object Count -Descending

$md = New-Object System.Collections.Generic.List[string]
$md.Add('# Mapa Funcional de Telas, Menus e Relatorios')
$md.Add('')
$md.Add('Data: 2026-05-23')
$md.Add('')
$md.Add('Escopo: executaveis principais do legado analisados por strings legiveis, sem alterar o original.')
$md.Add('')
$md.Add('## Arquivos gerados')
$md.Add('')
$md.Add('- `' + $summaryCsv + '`')
$md.Add('- `' + $correlationCsv + '`')
$md.Add('')
$md.Add('## Resultado por executavel')
$md.Add('')
foreach ($g in $byExe) {
    $md.Add("- $($g.Name): $($g.Count) textos funcionais candidatos")
}
$md.Add('')
$md.Add('## Resultado por categoria')
$md.Add('')
foreach ($g in $byCat) {
    $md.Add("- $($g.Name): $($g.Count)")
}
$md.Add('')
$md.Add('## Leitura tecnica')
$md.Add('')
$md.Add('- Este mapa indica o que cada executavel aparenta expor em menus, telas, mensagens, relatorios e acoes.')
$md.Add('- A coluna `StatusLayout` informa que a posicao exata ainda nao foi extraida de forma confiavel.')
$md.Add('- A posicao de campos, botoes e grades deve ser confirmada por captura dinamica no runtime MOD.')
$md.Add('- A correlacao com permissao usa nome/texto e deve ser tratada como indicio, nao como prova final.')
$md.Add('')
$md.Add('## Proxima validacao')
$md.Add('')
$md.Add('1. Abrir o runtime MOD com atualizacao bloqueada.')
$md.Add('2. Acessar cada menu principal com usuario administrador MOD.')
$md.Add('3. Capturar janela, titulo, controles visiveis, posicoes, atalhos, botoes e relatorios gerados.')
$md.Add('4. Cruzar captura visual com `Nivel.DB`, strings estaticas e tabelas Paradox.')

$md | Out-File -LiteralPath $reportMd -Encoding UTF8

Write-Output "Mapa funcional: $($rows.Count) linhas -> $summaryCsv"
Write-Output "Correlacoes: $($correlations.Count) linhas -> $correlationCsv"
Write-Output "Relatorio: $reportMd"

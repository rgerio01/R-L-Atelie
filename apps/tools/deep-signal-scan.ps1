$ErrorActionPreference = 'Stop'

$root = 'D:\AtelieProd\Equipexe'
$outDir = 'D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

$patterns = [ordered]@{
    'rede-http-socket' = '(https?://|ftp://|smtp|pop3|imap|socket|winsock|wininet|urlmon|download|upload|webservice|soap|http|porta|host|server|servidor)'
    'hardware-binding' = '(MacAddress|MachineName|ComputerName|GetVolumeInformation|GetComputerName|serial|hardware|hd|disco|maquina|máquina|estacao|estação|registrar|ativacao|ativação)'
    'licenca-autenticacao' = '(licen|license|senha|password|usuario|usu[aá]rio|login|autentica|validacao|validação|serial|registro|bloq|bloqueio|franquia)'
    'update-sync-cloud' = '(LiveUpdate|VerificaAtualizacoes|TesteVerificaAtualizacoes|atualiza|update|sincroniza|nuvem|download|subdirAtualiza|RegistraEstacao)'
    'memoria-processos' = '(CreateProcess|CreateThread|VirtualAlloc|VirtualFree|LoadLibrary|FreeLibrary|WinExec|ShellExecute|TerminateProcess|OpenProcess|Sleep|mutex|thread)'
    'temporarios-locks' = '(PDOXUSRS|\\.LCK|_QS|TEMP|temporario|temporary|cache|lock|net dir|local share)'
}

$files = Get-ChildItem -LiteralPath $root -Recurse -Force -File -Include *.exe,*.dll,*.ocx,*.ini,*.xml,*.bat,*.cmd,*.reg,*.txt -ErrorAction SilentlyContinue

foreach ($key in $patterns.Keys) {
    $out = Join-Path $outDir "$key.txt"
    if (Get-Command rg -ErrorAction SilentlyContinue) {
        rg -n -a -i $patterns[$key] $root --glob '*.exe' --glob '*.dll' --glob '*.ocx' --glob '*.ini' --glob '*.xml' --glob '*.bat' --glob '*.cmd' --glob '*.reg' --glob '*.txt' |
            Out-File -LiteralPath $out -Encoding UTF8
    } else {
        $files | Select-String -Pattern $patterns[$key] -CaseSensitive:$false | Out-File -LiteralPath $out -Encoding UTF8
    }
}

Get-ChildItem -LiteralPath $outDir -File | Select-Object Name,Length,LastWriteTime | ConvertTo-Csv -NoTypeInformation |
    Out-File -LiteralPath (Join-Path $outDir 'indice-sinais-profundos.csv') -Encoding UTF8

Write-Output "Sinais profundos gerados em $outDir"

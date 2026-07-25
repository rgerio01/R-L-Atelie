# Relatorio de Execucao Dinamica - Modulos Principais MOD

Data: 2026-05-23

## Escopo

Baselines dinamicos curtos no runtime MOD:

`D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe`

Modulos monitorados:

- `LavSoft.exe`
- `LavFacilLan.exe`
- `Gerenciador.exe`
- `Financeiro.exe`
- `Estoque.exe`

Nao foram monitorados nesta rodada:

- `NFE.exe`
- `SAT.exe`

Motivo: modulos fiscais devem ser avaliados em etapa controlada propria, com cuidado por possiveis dependencias de certificado, webservice fiscal, SAT/impressora e drivers.

## Resumo de performance

| Modulo | Duracao | Working Set pico | Memoria privada pico | Threads pico | Handles pico | Rede | Processos filhos | DLLs/modulos |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| LavSoft | 20,99s | 19,44 MB | 6,73 MB | 6 | 257 | 0 | 18 | 53 |
| LavFacilLan | 12,15s | 51,16 MB | 16,14 MB | 10 | 475 | 9 | 0 | 88 |
| Gerenciador | 12,02s | 20,41 MB | 22,95 MB | 6 | 232 | 0 | 0 | 40 |
| Financeiro | 12,03s | 21,06 MB | 4,77 MB | 6 | 245 | 0 | 0 | 46 |
| Estoque | 12,20s | 37,75 MB | 10,02 MB | 12 | 486 | 9 | 0 | 87 |

## Comunicacao externa observada

Foram observadas conexoes HTTP para:

`191.6.218.152:80`

Evidencias:

- `LavFacilLan`: `192.168.0.101:52033 -> 191.6.218.152:80`, estado `Established`.
- `Estoque`: `192.168.0.101:52049 -> 191.6.218.152:80`, estado `Established`.

Arquivos:

- `D:\AtelieProd\MOD\logs\observability\LavFacilLan-network-20260523-124446.csv`
- `D:\AtelieProd\MOD\logs\observability\Estoque-network-20260523-124524.csv`

Classificacao inicial:

- comunicacao externa confirmada;
- porta 80, HTTP sem TLS;
- finalidade ainda desconhecida;
- risco alto ate classificacao final.

Busca estatica:

- nao foi encontrada ocorrencia textual direta de `191.6.218.152` nos arquivos pesquisados do original e do runtime MOD;
- a origem pode depender de DNS, tabela de banco, dado compactado/ofuscado ou resposta remota.

## DLLs e dependencias observadas

### LavFacilLan

Dependencias relevantes carregadas:

- `wininet.dll`
- `WSOCK32.DLL`
- `urlmon.dll`
- `BEMAFI32.DLL`
- `Mp20fi32.dll`
- `general32.dll`
- `qtintf70.dll`
- `IDAPI32.DLL`

Interpretação:

- modulo com superficie de rede, fiscal/impressao e BDE.

### Estoque

Dependencias relevantes carregadas:

- `wininet.dll`
- `urlmon.dll`
- `IDAPI32.DLL`

Interpretação:

- modulo com superficie de rede e BDE.

## Isolamento de rede MOD

Scripts criados:

- `D:\AtelieProd\MOD\apps\tools\apply-mod-network-isolation.ps1`
- `D:\AtelieProd\MOD\apps\tools\rollback-mod-network-isolation.ps1`

Escopo preparado:

- `LavFacilLan.exe`
- `Estoque.exe`
- `LiveUpdate.exe`

Status:

- tentativa de aplicacao retornou `Acesso negado`;
- nenhuma regra ativa foi confirmada;
- aplicacao exige sessao elevada/administrativa;
- o script de rollback esta preparado para remover regras criadas no futuro.

## Conclusoes

1. O comportamento dinamico confirma que `LavFacilLan` e `Estoque` fazem comunicacao externa na inicializacao.
2. O bloqueio de `LiveUpdate` nao cobre todos os pontos de rede do legado.
3. A nova arquitetura deve centralizar comunicacoes externas em configuracao administravel.
4. A estrategia offline-first deve tratar essas rotas antigas como dependencias a substituir ou bloquear.
5. Os modulos mantem consumo de memoria relativamente baixo, coerente com objetivo de preservar leveza.

## Proximas acoes

1. Executar isolamento de rede MOD em sessao administrativa e repetir baselines.
2. Procurar `191.6.218.152` em strings, arquivos de configuracao e tabelas.
3. Classificar a finalidade da comunicacao.
4. Criar mapa de endpoints/hosts.
5. Executar etapa fiscal separada para `NFE` e `SAT`.

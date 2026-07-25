# Achado de Comunicacao HTTP no Runtime MOD

Data: 2026-05-23

## Resumo

Durante baselines dinamicos curtos no runtime MOD, foram observadas conexoes HTTP diretas para:

`191.6.218.152:80`

Modulos observados:

- `LavFacilLan.exe`
- `Estoque.exe`

## Evidencias

Arquivos:

- `D:\AtelieProd\MOD\logs\observability\LavFacilLan-network-20260523-124446.csv`
- `D:\AtelieProd\MOD\logs\observability\Estoque-network-20260523-124524.csv`

Trecho consolidado:

- `LavFacilLan`: `192.168.0.101:52033 -> 191.6.218.152:80`, estado `Established`.
- `Estoque`: `192.168.0.101:52049 -> 191.6.218.152:80`, estado `Established`.

## Classificacao inicial

Status: comunicacao externa confirmada em runtime MOD.

Classificacao provisoria:

- tipo: HTTP sem TLS observado pela porta 80;
- finalidade: desconhecida;
- risco: alto ate classificacao final;
- possiveis hipoteses: update, sincronizacao, validacao, telemetria ou servico auxiliar.

## Acao de contencao preparada

Foi criada politica reversivel de isolamento por firewall somente para executaveis do runtime MOD.

Scripts:

- aplicar: `D:\AtelieProd\MOD\apps\tools\apply-mod-network-isolation.ps1`
- rollback: `D:\AtelieProd\MOD\apps\tools\rollback-mod-network-isolation.ps1`

Escopo inicial:

- `LavFacilLan.exe`
- `Estoque.exe`
- `LiveUpdate.exe`

O original `D:\AtelieProd\Equipexe` nao e alterado por esta politica.

Status em 2026-05-23:

- Tentativa de aplicacao sem elevacao retornou `Acesso negado`.
- Nenhuma regra ativa foi confirmada pelo `Get-NetFirewallRule`.
- A politica esta preparada, mas exige execucao elevada/administrativa para aplicar no Windows Firewall.

## Busca estatica pelo IP

Foi feita busca estatica por `191.6.218.152` no original e no runtime MOD, excluindo bancos Paradox e indices.

Resultado:

- nenhuma ocorrencia textual direta encontrada;
- o endereco pode vir de DNS, configuracao codificada de outra forma, banco, DLL compactada/ofuscada ou resposta de servico.

## Proximas acoes

1. Repetir baseline apos isolamento e confirmar ausencia de conexao.
2. Identificar se o IP aparece em strings, INIs, tabelas ou DLLs.
3. Classificar finalidade da comunicacao.
4. Decidir se a nova versao deve substituir por configuracao administravel, sincronizacao propria ou bloqueio definitivo.

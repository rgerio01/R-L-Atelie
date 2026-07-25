# Plano de Observabilidade Futura

Data: 2026-05-23

## Objetivo

Dar visibilidade completa ao sistema novo e ao processo de migracao.

## Logs estruturados

Categorias:

- `app.lifecycle`
- `auth.login`
- `auth.permission`
- `admin.action`
- `sync.outbox`
- `sync.inbound`
- `sync.conflict`
- `db.query`
- `fiscal.nfe`
- `fiscal.sat`
- `financeiro.caixa`
- `report.run`
- `update.apply`
- `device.activation`
- `crash`

Campos minimos:

- timestamp;
- level;
- tenant_id;
- company_id;
- branch_id;
- user_id;
- device_id;
- module;
- action;
- correlation_id;
- duration_ms;
- result;
- error_code;
- message.

## Tracing

Cada fluxo deve ter `correlation_id`:

- login;
- entrada de ROL;
- pagamento;
- fechamento de caixa;
- emissao fiscal;
- relatorio;
- sincronizacao;
- atualizacao.

## Metricas

Metricas locais:

- tempo de inicializacao;
- memoria atual;
- pico de memoria;
- threads;
- handles;
- tempo por tela;
- tempo por relatorio;
- fila de sync pendente;
- erros por modulo;
- ultimo sync bem-sucedido.

## Crash dumps

Politica:

- dump leve por padrao;
- dump completo apenas sob modo diagnostico;
- redacao de dados sensiveis;
- retencao limitada;
- envio cloud somente com consentimento/politica administrativa.

## Health checks

Checks locais:

- banco local;
- lock de banco;
- fila de sync;
- permissao de escrita em logs;
- espaco em disco;
- validade da licenca;
- integridade do modulo;
- conectividade cloud opcional.

## Painel administrativo futuro

Visoes:

- lojas online/offline;
- dispositivos ativos;
- falhas recentes;
- sync pendente;
- versoes instaladas;
- tentativas de login;
- acoes administrativas;
- consumo de memoria;
- relatorios lentos.

## Continuidade da ferramenta atual

O coletor `EquipeExe.Mod.Observability` deve evoluir para:

- monitorar todos os modulos;
- associar amostras a telas;
- gravar eventos de navegacao;
- comparar consumo entre versoes;
- gerar baseline por maquina;
- alertar regressao de performance.

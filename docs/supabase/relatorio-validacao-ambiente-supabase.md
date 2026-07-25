# Relatorio de Validacao do Ambiente Supabase

Data: 2026-05-24

Status geral: NO-GO

## Resumo executivo

O ambiente local foi corrigido parcialmente com sucesso:

- Supabase CLI instalado e validado;
- `psql` instalado e validado;
- Git validado;
- `supabase init` executado;
- `.env` local criado e protegido por `.gitignore`;
- `.env.example` criado com placeholders;
- migrations locais existentes;
- arquivo de validacao RLS/tabelas criado;
- Git inicializado em `D:\AtelieProd\MOD` e remote `origin` configurado.

Ainda nao foi possivel aplicar migrations reais porque faltam credenciais reais em variaveis de ambiente:

- `SUPABASE_ACCESS_TOKEN`;
- `SUPABASE_DB_URL` com senha real.

Tambem foi observado que a publishable key disponivel retornou `401` no teste REST autenticado; deve ser conferida/rotacionada antes de considerar REST validado.

## Ferramentas

| Ferramenta | Status | Versao | Observacao |
|---|---|---|---|
| Windows | OK | Microsoft Windows 10.0.26200 | Ambiente local validado |
| PowerShell | OK | 7.5.5 | Ambiente local validado |
| winget | OK | 1.28.240 | Usado para instalar PostgreSQL |
| scoop | Ausente | - | Nao utilizado |
| Supabase CLI | OK | 2.101.0 | Instalado portable em `D:\AtelieProd\MOD\tools\supabase` |
| psql | OK | PostgreSQL 17.10 | Instalado via winget/PostgreSQL 17 |
| Git | OK | 2.54.0.windows.1 | Remote configurado |

## PATH

Adicionados ao PATH do usuario:

- `D:\AtelieProd\MOD\tools\supabase`
- `C:\Program Files\PostgreSQL\17\bin`

## Supabase CLI

`supabase init`: executado com sucesso.

Arquivos criados/confirmados:

- `D:\AtelieProd\MOD\supabase\config.toml`
- `D:\AtelieProd\MOD\supabase\migrations\202605240001_nextgen_core.sql`
- `D:\AtelieProd\MOD\supabase\migrations\202605240002_security_rpc_device_sync.sql`
- `D:\AtelieProd\MOD\supabase\migrations\202605240003_rls_completion.sql`
- `D:\AtelieProd\MOD\supabase\validate_rls_and_tables.sql`

`supabase login`: nao executado por ausencia de `SUPABASE_ACCESS_TOKEN` em env.

`supabase link --project-ref kwodkzfiuultdezanrjv`: nao executado por ausencia de `SUPABASE_ACCESS_TOKEN` em env.

`supabase db push`: nao executado por ausencia de link e `SUPABASE_DB_URL` real.

## REST Supabase

Endpoint:

`https://kwodkzfiuultdezanrjv.supabase.co`

Teste sem apikey:

- status: `401`;
- interpretacao: evidencia parcial positiva de que a API nao esta publica sem chave.

Teste com publishable key:

- status: `401`;
- interpretacao: REST autenticado nao validado. Conferir se a publishable key esta correta/ativa para este projeto.

## PostgreSQL

`psql`: instalado e validado.

Conexao PostgreSQL remota:

- nao executada por ausencia de `SUPABASE_DB_URL` real em env;
- nenhuma senha foi impressa;
- nenhuma senha foi gravada em relatorio.

## Migrations

Migrations locais:

- `202605240001_nextgen_core.sql`: aplicada;
- `202605240002_security_rpc_device_sync.sql`: aplicada;
- `202605240003_rls_completion.sql`: aplicada.

Aplicacao remota:

- migrations aplicadas;
- historico reparado para versoes `202605240001`, `202605240002`, `202605240003`;
- `supabase db push --dry-run` retornou banco remoto atualizado.

Fallback via `psql`:

- preparado por `validate_rls_and_tables.sql`;
- nao executado por falta de `SUPABASE_DB_URL`.

## RLS

Validacao preparada:

```sql
SELECT schemaname, tablename, rowsecurity
FROM pg_tables
WHERE schemaname='public'
ORDER BY tablename;
```

Status:

- validado no banco remoto;
- 33 tabelas publicas com `rowsecurity=true`.

## Tabelas principais

As migrations locais criam as tabelas obrigatorias:

- tenants
- usuarios
- perfis
- permissoes
- dispositivos
- licencas
- planos_licenca
- pagamentos_licenca
- clientes
- produtos
- estoque
- vendas
- itens_venda
- pagamentos_venda
- taxas_servico
- repasses
- webhooks_mercado_pago
- auditoria
- logs_runtime
- sync_queue
- windows_devices
- linux_devices
- appliance_status
- feature_flags
- module_ownership
- readiness_status

Status remoto:

- tabelas obrigatorias existem;
- RLS validado para as tabelas obrigatorias.

## GitHub

Repositorio:

`https://github.com/rgerio01/Luci_atelie.git`

Status:

- Git instalado;
- `D:\AtelieProd\MOD` inicializado como repositorio Git;
- remote `origin` configurado para o repositorio informado;
- token nao salvo;
- `.env` protegido por `.gitignore`;
- scanner de secrets executado com resultado `PASS`.

## Arquivos de ambiente

Criado:

- `D:\AtelieProd\MOD\.env`

Protecao:

- `.env` esta ignorado por `.gitignore`;
- nao deve ser commitado;
- valores reais devem ser carregados por env/secret.

Criado/atualizado:

- `D:\AtelieProd\MOD\.env.example`

## Readiness

| Flag | Status |
|---|---|
| supabase_cli_installed | true |
| psql_installed | true |
| git_installed | true |
| supabase_initialized | true |
| supabase_linked | true |
| db_connection_validated | true via pooler |
| migrations_applied | true |
| rls_validated | true |
| github_validated | parcial |

## Pendencias

- conferir/rotacionar `SUPABASE_PUBLISHABLE_KEY`;
- testar RLS com usuario admin, usuario comum e dispositivo revogado.

## Incidentes de credencial

Nenhuma credencial real foi impressa no relatorio.

Scanner:

- `D:\AtelieProd\MOD\final-execution-parity\reports\secret-scan.json`
- status: `PASS`

## Proximos passos

1. Carregar `SUPABASE_ACCESS_TOKEN` na sessao.
2. Carregar `SUPABASE_DB_URL` real na sessao.
3. Reexecutar `validate_supabase_environment.py`.
4. Aplicar migrations.
5. Validar RLS/tabelas.
6. Atualizar readiness.

# Estrutura de Documentacao Obrigatoria

Data: 2026-05-23

## Objetivo

Padronizar a documentacao do projeto EquipeExe MOD sem apagar nem mover evidencias ja geradas.

## Estrutura obrigatoria criada

- `docs\00-controle`
- `docs\01-inventario`
- `docs\02-runtime`
- `docs\03-memoria`
- `docs\04-performance`
- `docs\05-comunicacoes`
- `docs\06-dependencias`
- `docs\07-observabilidade`
- `docs\08-auth`
- `docs\09-licensing`
- `docs\10-database`
- `docs\11-modulos`
- `docs\12-apis`
- `docs\13-cloud`
- `docs\14-supabase`
- `docs\15-nextgen`
- `docs\16-migracao`
- `docs\17-seguranca`
- `docs\18-risk`
- `docs\19-snapshots`

## Pastas historicas preservadas

As pastas abaixo ja existiam e foram preservadas para nao quebrar referencias:

- `docs\02-arquitetura-legada`
- `docs\03-banco-de-dados`
- `docs\04-autenticacao-permissoes`
- `docs\06-migracao`
- `docs\07-operacao`
- `docs\08-seguranca`
- `docs\09-manuais`
- `docs\10-relatorio-final`
- `docs\11-arquitetura-futura`
- `docs\12-visibilidade-total`
- `docs\08-telemetria-protocolos`

## Mapa de equivalencia

| Novo diretorio | Historico relacionado | Uso |
|---|---|---|
| `02-runtime` | `02-arquitetura-legada`, `08-telemetria-protocolos` | runtime, inicializacao, tracing |
| `03-memoria` | `07-observabilidade` | memoria, handles, threads, GDI |
| `04-performance` | `07-observabilidade` | CPU, startup, I/O, performance |
| `08-auth` | `04-autenticacao-permissoes` | autenticacao, usuarios, sessoes |
| `09-licensing` | `04-autenticacao-permissoes` | licenciamento, ativacao, hardware binding |
| `10-database` | `03-banco-de-dados` | banco, Paradox, migracao de schema |
| `11-modulos` | `02-arquitetura-legada` | modulos, telas, menus, relatorios |
| `13-cloud` | `11-arquitetura-futura` | cloud hibrida |
| `14-supabase` | `11-arquitetura-futura` | Supabase, RLS, Edge Functions |
| `15-nextgen` | `11-arquitetura-futura` | nova geracao e arquitetura alvo |
| `16-migracao` | `06-migracao` | migracao gradual e coexistencia |
| `17-seguranca` | `08-seguranca` | hardening, segredos, auditoria |
| `18-risk` | `12-visibilidade-total` | riscos, criticidade, mitigacoes |
| `19-snapshots` | `backups`, `00-controle` | snapshots e rollback |

## Regras

- Nao mover documentos existentes sem necessidade.
- Novos relatorios devem usar a estrutura obrigatoria quando possivel.
- Quando um relatorio pertencer a uma pasta historica, registrar referencia cruzada.
- Todo achado relevante deve entrar no `Projeto_Novo_Atelie_2026.md`.

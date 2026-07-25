# Plano Supabase - Auth, Licensing, Devices e Auditoria

Data: 2026-05-23

## Objetivo

Planejar a futura utilizacao do Supabase como camada cloud da nova geracao do EquipeExe, mantendo operacao local offline-first.

## Responsabilidades do Supabase

- autenticacao;
- usuarios;
- perfis;
- permissoes;
- sessoes;
- dispositivos autorizados;
- licencas;
- feature flags;
- auditoria;
- telemetria administrativa;
- fila de sincronizacao cloud;
- painel administrativo.

## Modelo multi-tenant

Campos obrigatorios:

- `tenant_id`
- `company_id`
- `branch_id`

Entidades principais:

- `profiles`
- `companies`
- `branches`
- `roles`
- `permissions`
- `role_permissions`
- `user_roles`
- `devices`
- `device_fingerprints`
- `licenses`
- `license_assignments`
- `sessions`
- `audit_events`
- `feature_flags`
- `sync_events`

## RLS

Politicas:

- usuario so acessa dados do proprio `tenant_id`;
- filial limitada por `branch_id`, exceto administradores;
- auditoria visivel apenas para perfis autorizados;
- service role usada somente em Edge Functions controladas;
- dispositivos e licencas gerenciados por admins.

## Edge Functions

Funcoes futuras:

- `activate-device`
- `refresh-license`
- `revoke-device`
- `sync-permissions`
- `publish-feature-flags`
- `ingest-audit-events`
- `issue-offline-grant`

## Offline-first

O app local deve manter:

- cache de usuario/permissao;
- licenca assinada local;
- device binding local;
- fila de eventos;
- data limite offline (`offline_until`);
- ultima sincronizacao (`last_sync_at`).

Sem internet:

- operacao comum continua;
- admin remoto e sync ficam pendentes;
- auditoria acumula localmente;
- licenca local vale ate `offline_until`.

## Migracao a partir do legado

1. Extrair usuarios/permissoes Paradox.
2. Criar matriz de permissao moderna.
3. Criar tenants/empresas/filiais.
4. Registrar dispositivos atuais.
5. Emitir licencas locais iniciais.
6. Rodar modo sombra comparando decisoes.
7. Ativar cloud por filial.

## Seguranca

- TLS obrigatorio;
- secrets apenas no backend/Edge Functions;
- licencas assinadas;
- logs sem senha ou chave;
- auditoria append-only;
- revogacao de dispositivo;
- MFA opcional para administradores.

## Conclusao

Supabase deve ser a camada administrativa central, mas nao o ponto unico de falha. A decisao operacional essencial precisa estar cacheada e assinada localmente para manter continuidade.

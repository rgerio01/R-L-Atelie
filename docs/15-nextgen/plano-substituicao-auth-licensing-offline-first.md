# Plano de Substituicao Gradual - Auth, Licensing e Offline-First

Data: 2026-05-23

## Objetivo

Substituir gradualmente autenticacao, licenciamento, gerenciamento de dispositivos, sessoes, permissoes, sincronizacao e telemetria do legado por uma arquitetura propria, administravel e offline-first.

## Principios

- O legado continua como referencia operacional ate validacao completa.
- A nova camada nao deve depender de endpoints LavSoft antigos.
- Toda decisao critica deve funcionar offline.
- O cloud deve complementar a operacao local, nao impedir continuidade.
- Toda mudanca deve ter rollback e log.
- A migracao deve preservar baixo consumo de memoria.

## Arquitetura alvo

### Local

- SQLite ou banco local equivalente.
- Cache de usuarios/permissoes.
- Licenca local assinada.
- Device binding tolerante.
- Fila de sincronizacao.
- Logs estruturados.
- Auditoria local.
- Health checks.

### Cloud/Supabase futuro

- Auth central.
- Usuarios e perfis.
- Permissoes granulares.
- Dispositivos autorizados.
- Sessoes.
- Licencas.
- Feature flags.
- Auditoria.
- Logs centralizados.
- Edge Functions para ativacao/sync.
- RLS por `tenant_id`, `company_id`, `branch_id`.

## Modelo novo de licenciamento

Entidades locais:

- `tenants`
- `companies`
- `branches`
- `users`
- `roles`
- `permissions`
- `devices`
- `device_fingerprints`
- `licenses`
- `license_grants`
- `sessions`
- `audit_events`
- `sync_queue`

Campos base:

- `tenant_id`
- `company_id`
- `branch_id`
- `device_id`
- `license_id`
- `issued_at`
- `expires_at`
- `offline_until`
- `signature`
- `revoked_at`
- `last_sync_at`

## Device binding novo

Identificadores recomendados:

- machine GUID;
- motherboard/BIOS quando disponivel;
- volume serial;
- MAC normalizado;
- usuario/hostname como sinal fraco;
- TPM quando existir.

Politica:

- usar score de similaridade;
- tolerar troca parcial de hardware;
- exigir reativacao apenas se score cair abaixo do limite;
- registrar toda troca;
- permitir revogacao administrativa.

## Autenticacao nova

Local:

- hash forte de senha;
- login offline com cache valido;
- lockout progressivo;
- auditoria local;
- obrigatoriedade de troca configuravel, nao fixa.

Cloud:

- Supabase Auth;
- MFA opcional;
- sessoes revogaveis;
- sincronizacao de permissoes;
- refresh controlado quando houver internet.

## Permissoes novas

Padrao granular:

- `financeiro.visualizar`
- `financeiro.editar`
- `pdv.cancelar`
- `usuarios.criar`
- `admin.logs`
- `licensing.gerenciar`
- `devices.revogar`
- `sync.executar`

Migracao:

1. Mapear `Usuarios`, `Senhas`, `Nivel`, `GruUsuarios`.
2. Criar matriz equivalencia legado -> nova permissao.
3. Rodar em modo sombra, sem bloquear operacao.
4. Comparar decisoes do legado e da nova camada.
5. Ativar bloqueio novo modulo por modulo.

## Sincronizacao

Fila local:

- operacao;
- entidade;
- payload;
- prioridade;
- tentativas;
- ultimo erro;
- status;
- criado/em_processamento/concluido/falhou.

Regras:

- idempotencia por chave externa;
- backoff exponencial;
- conciliacao por versao/updated_at;
- auditoria de conflito;
- nunca bloquear operacao local comum por falha de cloud.

## Substituicao por fases

### Fase 1 - Observabilidade sombra

- capturar login, permissao, device e endpoints;
- nao mudar comportamento;
- criar logs MOD.

### Fase 2 - Auth local paralelo

- usuarios novos no MOD;
- Gabriela admin principal;
- permissoes equivalentes;
- senha `12345` apenas como estado administrativo atual de homologacao.

### Fase 3 - Licensing local controlado

- licenca local assinada;
- device binding novo;
- auditoria local;
- sem dependencia do legado.

### Fase 4 - Sync cloud opcional

- Supabase para usuarios, dispositivos, licencas e auditoria;
- cache local obrigatorio;
- fallback offline.

### Fase 5 - Corte gradual do legado

- modulo por modulo;
- comparar decisoes legado x MOD;
- rollback por feature flag;
- desativar endpoints antigos depois de validacao.

## Rollback

- manter banco legado readonly durante migracao;
- preservar MOD separado;
- feature flag para voltar a decisao legada;
- exportar snapshot antes de migracao;
- logs de toda conversao de permissao/licenca.

## Requisitos de seguranca

- TLS obrigatorio em cloud;
- assinatura digital de licenca;
- segredos fora de INI;
- senha com hash moderno;
- logs sem senha/chave em texto puro;
- auditoria imutavel;
- principio do menor privilegio.

## Conclusao

A substituicao deve ocorrer como uma camada nova e observavel, primeiro em modo sombra, depois em operacao local, e por fim com cloud hibrida. O objetivo e eliminar dependencia dos endpoints e segredos legados sem quebrar a continuidade operacional.

# Plano Offline-First, Sync e Supabase

Data: 2026-05-23

## Objetivo

Projetar uma arquitetura local/cloud que preserve continuidade operacional mesmo sem internet e permita administracao centralizada futura.

## Modelo local

Banco local proposto:

- SQLite para nova aplicacao;
- Paradox/BDE apenas como origem legada durante migracao;
- tabelas locais com `tenant_id`, `company_id`, `branch_id`;
- fila local de eventos;
- logs estruturados;
- cache de autenticacao;
- cache de feature flags;
- cache de permissao.

## Modelo cloud

Supabase como plano futuro para:

- autenticacao;
- usuarios;
- permissoes;
- tenants;
- empresas;
- filiais;
- dispositivos;
- sessoes;
- licencas;
- auditoria;
- telemetria administrativa;
- feature flags;
- sincronizacao;
- dashboard administrativo.

## Entidades base

- `tenants`
- `companies`
- `branches`
- `users`
- `roles`
- `permissions`
- `role_permissions`
- `user_roles`
- `devices`
- `device_activations`
- `licenses`
- `sessions`
- `feature_flags`
- `audit_events`
- `sync_events`
- `sync_checkpoints`
- `app_versions`
- `update_channels`

## Padrao de identificadores

Toda entidade sincronizavel deve ter:

- `id` global;
- `tenant_id`;
- `company_id`;
- `branch_id` quando aplicavel;
- `created_at`;
- `updated_at`;
- `deleted_at` para exclusao logica;
- `version`;
- `source_device_id`;
- `sync_status`.

## Sync local/cloud

### Outbox local

Cada alteracao local gera evento:

- entidade;
- operacao;
- payload;
- versao;
- usuario;
- dispositivo;
- data;
- hash;
- tentativas;
- status.

### Inbound sync

O cliente baixa alteracoes:

- por tenant;
- por filial;
- por checkpoint;
- por permissao;
- com paginacao;
- com validacao de versao.

### Conflitos

Politicas possiveis:

- last-write-wins para dados simples;
- merge controlado para cadastros;
- bloqueio por fluxo para fiscal/caixa;
- resolucao manual para divergencias financeiras;
- auditoria obrigatoria em conflito.

## RLS e seguranca

Diretrizes futuras:

- toda tabela cloud sensivel deve usar isolamento por `tenant_id`;
- usuarios comuns acessam apenas sua empresa/filial autorizada;
- dispositivos precisam estar ativos e nao revogados;
- auditoria nao deve ser editavel pelo cliente;
- operacoes administrativas criticas devem passar por funcao controlada.

## Edge Functions futuras

Usos planejados:

- ativacao de dispositivo;
- revogacao;
- validacao de licenca;
- emissao de token offline;
- consolidacao de auditoria;
- processamento de sync;
- webhooks administrativos;
- feature flags por tenant.

## Fallback offline

Quando sem internet:

- login permitido com cache valido;
- permissoes lidas do cache local;
- acoes geram eventos locais;
- licenca usa janela de tolerancia;
- sincronizacao fica pendente;
- logs permanecem locais;
- alertas administrativos sao gerados ao reconectar.

## Device binding

Identificadores candidatos:

- CPU;
- motherboard;
- volume serial;
- MAC;
- TPM quando disponivel.

Modelo:

- gerar fingerprint local;
- calcular score de similaridade;
- tolerar troca parcial de hardware;
- exigir aprovacao quando score cair abaixo do limite;
- registrar historico de fingerprints;
- permitir revogacao remota.

## Feature flags

Modelo:

- flag global;
- flag por tenant;
- flag por empresa;
- flag por filial;
- flag por usuario/perfil;
- cache local;
- valor padrao seguro;
- rollback rapido.

## Riscos

- sincronizacao financeira/fiscal exige regra forte de conflito;
- operacao offline nao pode depender de validacao cloud em tempo real;
- telemetria nao pode expor dados sensiveis;
- multi-tenant deve ser modelado desde o inicio;
- atualizacao automatica deve ser assinada e reversivel.

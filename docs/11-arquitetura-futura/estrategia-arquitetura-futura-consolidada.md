# Estrategia de Arquitetura Futura Consolidada

Data: 2026-05-23

## Norte arquitetural

A nova geracao do EquipeExe deve ser:

- offline-first;
- hibrida local/cloud;
- modular;
- leve;
- observavel;
- administravel internamente;
- preparada para multi-tenant;
- segura por padrao;
- capaz de rollback operacional.

## Premissas

- O legado atual e predominantemente Windows desktop 32-bit, Delphi/BDE/Paradox.
- A substituicao total imediata e arriscada.
- A reconstrucao deve ser gradual e guiada por evidencias.
- O runtime MOD deve funcionar como laboratorio controlado.
- A nova arquitetura deve preservar fluxos e regras de negocio antes de redesenhar experiencia.

## Camadas propostas

### Local Runtime

Responsavel por operacao diaria offline.

Componentes:

- app desktop;
- SQLite/local DB;
- cache operacional;
- fila de sincronizacao;
- logs locais;
- auditoria local;
- autenticacao cacheada;
- feature flags cacheadas;
- motor de relatorios local.

### Local API

Responsavel por separar regra de negocio da interface.

Componentes:

- autenticacao local;
- permissoes;
- usuarios;
- modulos;
- relatorios;
- sync;
- auditoria;
- integracoes fiscais;
- health checks.

### Cloud Control Plane

Responsavel por administracao centralizada.

Componentes:

- usuarios;
- permissoes;
- licenciamento;
- dispositivos autorizados;
- sessoes;
- auditoria;
- telemetria administrativa;
- feature flags;
- atualizacoes;
- dashboard.

### Sync Engine

Responsavel por reconciliar local e nuvem.

Componentes:

- outbound queue;
- inbound queue;
- controle de versao;
- idempotencia;
- retries;
- backoff;
- conflito;
- checkpoints;
- dead-letter local;
- logs de sincronizacao.

## Modulos futuros

Estrutura logica desejada:

- `/modules/auth`
- `/modules/licensing`
- `/modules/sync`
- `/modules/core`
- `/modules/financeiro`
- `/modules/pdv`
- `/modules/usuarios`
- `/modules/dashboard`
- `/modules/relatorios`
- `/modules/admin`
- `/modules/telemetria`
- `/modules/fiscal`
- `/modules/estoque`
- `/modules/clientes`

## Politica de atualizacao

O legado teve atualizacao automatica bloqueada no MOD. A futura atualizacao deve ser reconstruida como componente separado.

Requisitos:

- updater isolado;
- assinatura digital;
- verificacao de integridade;
- rollback automatico;
- atualizacao modular;
- delta updates;
- trilha de auditoria;
- janela de manutencao;
- bloqueio administrativo.

## Permissoes granulares

Modelo futuro:

- permissao por modulo;
- permissao por tela;
- permissao por acao;
- permissao por botao;
- permissao por API;
- auditoria por decisao de permissao.

Exemplos:

- `financeiro.visualizar`
- `financeiro.editar`
- `financeiro.exportar`
- `pdv.cancelar`
- `pdv.reimprimir`
- `usuarios.criar`
- `usuarios.bloquear`
- `admin.logs.visualizar`
- `fiscal.nfe.emitir`
- `fiscal.sat.cancelar`

## Performance

Metas:

- inicializacao rapida;
- baixo consumo de RAM;
- carregamento sob demanda;
- modulos fiscais carregados apenas quando necessarios;
- relatorios paginados;
- cache local controlado;
- consultas indexadas;
- rotinas longas em background;
- UI responsiva mesmo offline.

## Estrategia de migracao

1. Mapear comportamento legado.
2. Criar API local equivalente para novas funcoes.
3. Criar autenticacao/permissao propria.
4. Criar banco local moderno em paralelo.
5. Migrar modulo por modulo.
6. Validar equivalencia operacional.
7. Ativar sincronizacao cloud opcional.
8. Desativar dependencias antigas somente apos homologacao.

## Criterio de sucesso

A nova arquitetura sera considerada pronta para substituicao gradual quando:

- autenticar e autorizar usuarios internamente;
- reproduzir fluxos operacionais criticos;
- operar offline;
- sincronizar com controle;
- auditar acoes administrativas;
- recuperar falhas;
- fazer rollback;
- manter consumo baixo de memoria;
- preservar dados e experiencia operacional.

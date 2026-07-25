# Plano de Modularizacao

Data: 2026-05-23

## Objetivo

Reduzir acoplamento, permitir migracao gradual e criar uma base facil de manter.

## Modulos

### Auth

- login local;
- cache de sessao;
- MFA futuro;
- politica de senha;
- auditoria de login;
- integracao Supabase futura.

### Licensing

- licenca local;
- janela offline;
- device binding;
- ativacao;
- revogacao;
- auditoria.

### Sync

- outbox;
- inbound sync;
- conflitos;
- retry;
- checkpoints;
- dead-letter.

### Core

- cadastros base;
- parametros;
- empresas;
- filiais;
- regras compartilhadas.

### Financeiro

- contas;
- pagamentos;
- cobranca;
- caixa;
- faturamento;
- recibos.

### PDV / Operacional

- ROL;
- entrada;
- entrega;
- cancelamento;
- reemissao;
- lavagem;
- passadoria;
- localizacao de pecas.

### Fiscal

- NFE;
- NFSe;
- SAT;
- impressoras fiscais;
- contingencia;
- logs fiscais.

### Estoque

- produtos;
- grupos;
- movimentacao;
- inventario;
- relatorios.

### Usuarios

- usuarios;
- grupos;
- perfis;
- permissoes;
- auditoria administrativa.

### Relatorios

- catalogo de relatorios;
- parametros;
- fontes de dados;
- exportacao;
- impressao.

### Admin

- painel administrativo;
- logs;
- feature flags;
- dispositivos;
- backups;
- atualizacoes.

### Telemetria

- health checks;
- metricas;
- tracing;
- crash reports;
- performance.

## Contratos entre modulos

Cada modulo deve declarar:

- nome;
- versao;
- dependencias;
- permissoes;
- tabelas;
- endpoints locais;
- eventos emitidos;
- eventos consumidos;
- logs;
- rollback.

## Politica de rollback modular

- cada modulo possui migracoes independentes;
- atualizacao deve validar integridade antes de ativar;
- se falhar, reativar versao anterior;
- logs de rollback devem ser preservados;
- dados devem usar migracao reversivel quando possivel.

## Ordem recomendada

1. Auth.
2. Usuarios/permissoes.
3. Observabilidade.
4. Core/cadastros.
5. Financeiro/caixa.
6. PDV/operacional.
7. Estoque.
8. Relatorios.
9. Fiscal.
10. Sync/cloud.
11. Admin/dashboard.

# Escopo Tecnico do Novo Sistema

Data: 2026-05-23

## Objetivo

Criar um sistema novo, moderno, offline-first, auditavel e preparado para Supabase, preservando os dominios operacionais aprendidos no EquipeExe.

## Camadas

- `frontend-desktop`: interface operacional leve.
- `backend-local`: API local, sincronizacao, filas e regras.
- `backend-cloud`: autenticacao, licencas, pagamentos, dispositivos e auditoria central.
- `database-local`: SQLite ou equivalente embarcado.
- `database-cloud`: Supabase/PostgreSQL futuro.
- `sync-engine`: reconciliacao offline/online.
- `payment-gateway`: integracao Mercado Pago/Mercado Livre.
- `audit-engine`: trilha de eventos.
- `license-engine`: planos, vencimentos, entitlement e device binding.

## Modulos

- Clientes
- Produtos
- Estoque
- Ordens/ROL/Servicos
- Vendas
- Caixa
- Financeiro
- Fiscal
- Relatorios
- Usuarios e Permissoes
- Auditoria
- Licenciamento
- Pagamentos
- Sincronizacao
- Configuracoes

## Banco novo

Entidades obrigatorias:

- `usuarios`
- `perfis`
- `permissoes`
- `usuarios_permissoes`
- `clientes`
- `produtos`
- `estoque`
- `ordens_servico`
- `vendas`
- `itens_venda`
- `pagamentos`
- `caixa`
- `licencas`
- `dispositivos`
- `maquinas_cartao`
- `transacoes_pix`
- `transacoes_cartao`
- `taxas_servico`
- `repasses`
- `auditoria`
- `logs`
- `configuracoes`

## Nao funcionais

- baixo consumo de memoria;
- inicializacao rapida;
- operacao local mesmo sem internet para atividades nao digitais;
- sincronizacao posterior;
- logs estruturados;
- trilha de auditoria;
- rollback de migracao;
- exportacao de dados;
- controle granular de permissoes.

## Politica offline-first

Permitido offline:

- consultar dados locais;
- cadastrar/editar cliente;
- abrir OS/ROL;
- registrar dinheiro;
- gerar auditoria local;
- preparar venda pendente.

Exige online/confirmacao:

- PIX;
- cartao;
- renovacao de licenca;
- revogacao/autorizacao de dispositivo;
- conciliacao de pagamento.

## Integracao Supabase futura

- Auth para usuarios/cloud.
- PostgreSQL para sincronizacao.
- RLS por `tenant_id`, `company_id`, `branch_id`.
- Edge Functions para licencas, webhooks e pagamentos.
- Storage para anexos/documentos.
- Realtime apenas quando necessario.

## Rollback

- Migracao inicia em leitura do legado.
- Escrita NextGen separada.
- Exportacao e importacao versionadas.
- Nenhuma etapa depende de alterar o EquipeExe original.

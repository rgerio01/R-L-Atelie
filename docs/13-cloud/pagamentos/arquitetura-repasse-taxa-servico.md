# Arquitetura da Taxa de Servico por Venda

Data: 2026-05-23

## Regra inicial

A cada venda realizada por Luci, registrar taxa de servico de R$ 0,05 para Rogerio.

Esta taxa deve ser:

- transparente;
- auditavel;
- registrada em banco;
- visivel em relatorios administrativos/financeiros;
- conciliavel;
- nunca ocultada em valores de cliente ou operador.

## Modelos possiveis de liquidacao

### Modelo A - Split/marketplace homologado

Se o provedor e contrato permitirem split ou marketplace, o backend cria a transacao com recebedor principal Luci e componente de taxa Rogerio.

Status: a validar com Mercado Pago, contrato e tipo de integracao.

### Modelo B - Passivo interno e repasse posterior

Venda entra integralmente para Luci. O sistema registra uma taxa de R$ 0,05 como passivo a repassar para Rogerio. Periodicamente, o financeiro gera repasse consolidado.

Status: recomendacao inicial por ser mais controlavel e simples de auditar.

## Tabelas

### `config_taxas`

- `id`
- `tipo_taxa`
- `valor_fixo`
- `percentual`
- `ativo`
- `recebedor_taxa`
- `vigencia_inicio`
- `vigencia_fim`
- `created_by`
- `created_at`

### `taxas_servico`

- `id`
- `venda_id`
- `valor_venda`
- `valor_taxa`
- `recebedor_principal`
- `recebedor_taxa`
- `status_repasse`: `pendente`, `agrupado`, `pago`, `cancelado`, `estornado`
- `data_criacao`
- `data_pagamento`
- `origem`
- `observacao`

### `repasse_taxas`

- `id`
- `recebedor_taxa`
- `periodo_inicio`
- `periodo_fim`
- `valor_total`
- `quantidade_vendas`
- `status`
- `payment_provider_id`
- `data_criacao`
- `data_pagamento`
- `created_by`

### `historico_taxas`

- `id`
- `taxa_servico_id`
- `acao`
- `valor_anterior`
- `valor_novo`
- `usuario_id`
- `data_hora`
- `motivo`

## Fluxo

1. Venda aprovada para Luci.
2. Sistema cria registro em `taxas_servico` com R$ 0,05.
3. Auditoria grava usuario, caixa, venda, conta recebedora e regra aplicada.
4. Relatorio financeiro lista taxas pendentes/pagas.
5. Financeiro gera repasse consolidado.
6. Pagamento/baixa do repasse exige permissao `taxas.repassar`.

## Regras de cancelamento

- Venda cancelada antes de conclusao: taxa nao nasce.
- Venda estornada: taxa deve mudar para `estornado` ou gerar ajuste negativo.
- Repasse ja pago com venda estornada: gerar ajuste em proximo ciclo.

## Auditoria obrigatoria

- regra vigente aplicada;
- usuario da venda;
- usuario do cancelamento/estorno;
- caixa;
- dispositivo;
- conta recebedora de venda;
- conta recebedora da taxa;
- status de conciliacao.

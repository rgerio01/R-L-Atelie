# Modelo Banco NextGen

Data: 2026-05-23

## Tabelas núcleo

- `customers`
- `customer_contacts`
- `service_orders`
- `service_order_items`
- `service_order_status_history`
- `products`
- `stock_items`
- `stock_movements`
- `receivables`
- `payments`
- `invoices`
- `fiscal_events`
- `users`
- `roles`
- `permissions`
- `audit_events`

## Campos de rastreabilidade obrigatorios

- `legacy_table`
- `legacy_path`
- `legacy_key`
- `legacy_hash`
- `evidence_status`
- `migration_batch_id`

## Regra

Nenhum relacionamento importado deve virar FK obrigatoria sem estar classificado ao menos como confirmado por schema. Relacionamentos por hipótese devem entrar em tabela de staging para validação.

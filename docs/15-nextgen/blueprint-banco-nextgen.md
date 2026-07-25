# Blueprint Banco NextGen

Data: 2026-05-23

## Objetivo

Propor estrutura de banco moderna para substituir gradualmente o Paradox/BDE, preservando dados, relacoes e comportamento operacional.

## Padroes obrigatorios

Campos base:

- `id`
- `tenant_id`
- `company_id`
- `branch_id`
- `legacy_table`
- `legacy_key`
- `created_at`
- `updated_at`
- `deleted_at`
- `created_by`
- `updated_by`

Auditoria:

- `audit_events`
- `entity_type`
- `entity_id`
- `action`
- `old_value`
- `new_value`
- `user_id`
- `occurred_at`
- `source`

## Tabelas propostas

### Clientes

- `customers`
- `customer_contacts`
- `customer_addresses`
- `customer_notes`
- `customer_groups`
- `customer_employees`
- `customer_garments`

Mapeamento legado:

- `Clientes`
- `CliContato`
- `ClientesObs`
- `GruClientes`
- `FunCli`
- `FunCliRou`

### OS / ROL

- `service_orders`
- `service_order_items`
- `service_order_status_history`
- `service_order_locations`
- `service_order_prints`
- `service_order_cancellations`

Campos principais em `service_orders`:

- `legacy_rol`
- `legacy_num_os`
- `customer_id`
- `opened_at`
- `expected_at`
- `delivered_at`
- `status`
- `position`
- `total_amount`
- `total_items`
- `user_id`

Mapeamento legado:

- `MovCab`
- `MovLocRol`
- `CadLocRol`
- `ControleEti`
- `IndenRol`

### Produtos e servicos

- `products`
- `services`
- `price_tables`
- `price_table_items`
- `product_groups`
- `product_kits`
- `product_packages`

Mapeamento legado:

- `Produt`
- `ProdEst`
- `TabProdEst`
- `ProdEstKit`
- `ProdEstPac`

### Estoque

- `stock_items`
- `stock_movements`
- `stock_cancellations`
- `stock_closings`

Campos principais em `stock_movements`:

- `stock_item_id`
- `movement_type`
- `quantity`
- `unit_value`
- `total_value`
- `reason`
- `source_type`
- `source_id`
- `user_id`

Mapeamento legado:

- `MovEst`
- `MovEstCan`
- `MovEstEnc`
- `ProdEst`

### Financeiro

- `cash_sessions`
- `cash_movements`
- `receivables`
- `payables`
- `payments`
- `customer_credits`
- `bank_slips`
- `invoice_payments`

Mapeamento legado:

- `Duplicat`
- `Boletos`
- `DupBoleto`
- `CliCredito`
- `FecCaixa`
- `MovIniCaixa`
- `Titulos`
- `TitGru`

### Fiscal

- `invoices`
- `fiscal_documents`
- `fiscal_cancellations`
- `sat_events`
- `nfe_events`

Mapeamento legado:

- `Notas`
- `NotaFisPag`
- `NotaSat`
- `NotaSatCanc`

### Permissoes

- `users`
- `roles`
- `permissions`
- `role_permissions`
- `user_roles`
- `branch_permissions`

Mapeamento legado:

- `Usuarios`
- `Senhas`
- `Nivel`
- `GruUsuarios`

### Sincronizacao

- `sync_queue`
- `sync_conflicts`
- `sync_checkpoints`

## Integridade

Regras:

- `service_orders.customer_id` obrigatorio.
- `service_order_items.service_order_id` obrigatorio.
- `receivables.customer_id` obrigatorio quando origem for cliente.
- `stock_movements.stock_item_id` obrigatorio.
- Cancelamentos devem registrar motivo e usuario.
- Valores monetarios devem ter precisao decimal.

## Migracao

Etapas:

1. Congelar snapshot readonly.
2. Importar tabelas de referencia.
3. Importar clientes.
4. Importar produtos/servicos.
5. Importar OS/ROL.
6. Importar financeiro.
7. Importar estoque.
8. Importar fiscal.
9. Importar permissoes.
10. Validar totais por relatorio legado.

## Conclusao

O banco nextgen deve ser relacional, auditavel e offline-first. A chave da migracao e preservar `legacy_key` para rastreabilidade campo a campo.

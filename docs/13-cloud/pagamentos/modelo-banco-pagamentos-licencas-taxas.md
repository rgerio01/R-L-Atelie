# Modelo de Banco - Pagamentos, Licencas e Taxas

Data: 2026-05-23

## Entidades principais

### `pagamentos`

- `id`
- `tenant_id`
- `origem`: `venda`, `licenca`, `repasse`, `ajuste`
- `origem_id`
- `forma`: `pix`, `cartao_credito`, `cartao_debito`, `dinheiro`
- `valor`
- `status`
- `recebedor_conta_id`
- `provider`
- `provider_payment_id`
- `provider_order_id`
- `external_reference`
- `idempotency_key`
- `created_by`
- `created_at`
- `approved_at`
- `cancelled_at`

### `transacoes_pix`

- `id`
- `pagamento_id`
- `qr_code_payload`
- `qr_code_base64`
- `expira_em`
- `status`
- `provider_payload_hash`

### `transacoes_cartao`

- `id`
- `pagamento_id`
- `terminal_id`
- `maquina_cartao_id`
- `tipo_cartao`
- `parcelas`
- `nsu`
- `authorization_code`
- `brand`
- `status`

### `maquinas_cartao`

- `id`
- `tenant_id`
- `provider`
- `terminal_id`
- `apelido`
- `serial`
- `pos_id`
- `store_id`
- `status`
- `ultimo_checkin`

### `licencas`

- `id`
- `tenant_id`
- `plano`
- `valor`
- `status`
- `data_inicio`
- `data_vencimento`
- `limite_dispositivos`
- `dispositivo_principal_id`
- `entitlement_assinado`
- `grace_period_until`

### `dispositivos`

- `id`
- `tenant_id`
- `nome`
- `hardware_fingerprint_hash`
- `score_confianca`
- `status`
- `ativado_em`
- `ultimo_checkin`

### `taxas_servico`

- `id`
- `venda_id`
- `valor_venda`
- `valor_taxa`
- `recebedor_principal`
- `recebedor_taxa`
- `status_repasse`
- `data_criacao`
- `data_pagamento`
- `origem`
- `observacao`

### `auditoria`

- `id`
- `tenant_id`
- `usuario_id`
- `acao`
- `entidade`
- `entidade_id`
- `antes_json`
- `depois_json`
- `ip_origem`
- `dispositivo_id`
- `created_at`

## Indices recomendados

- `pagamentos(origem, origem_id)`
- `pagamentos(status, created_at)`
- `pagamentos(provider_payment_id)`
- `licencas(tenant_id, status)`
- `dispositivos(tenant_id, status)`
- `taxas_servico(status_repasse, data_criacao)`
- `auditoria(entidade, entidade_id, created_at)`

## Regras de integridade

- Todo pagamento deve ter `external_reference` unica por tenant.
- Toda venda aprovada deve ter pagamento aprovado ou dinheiro registrado.
- Toda taxa de servico deve apontar para venda existente.
- Toda baixa financeira deve gerar auditoria.
- Credenciais nao pertencem a este banco operacional em texto puro.

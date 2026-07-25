# Arquitetura de Licenciamento e Pagamento de Licenca

Data: 2026-05-23

## Objetivo

Substituir o licenciamento legado por um modulo proprio, administravel, auditavel e preparado para operacao local/cloud.

## Planos

| Plano | Periodicidade | Vencimento | Recebedor |
|---|---|---|---|
| Mensal | 1 mes | data de ativacao + 1 mes | Rogerio |
| Trimestral | 3 meses | data de ativacao + 3 meses | Rogerio |
| Semestral | 6 meses | data de ativacao + 6 meses | Rogerio |
| Anual | 12 meses | data de ativacao + 12 meses | Rogerio |

Valores devem ficar em `config_planos_licenca`, nunca fixos no executavel.

## Campos da licenca

- `id`
- `tenant_id`
- `plano`
- `valor`
- `status`: `trial`, `ativa`, `vencida`, `bloqueada`, `cancelada`, `pendente_pagamento`
- `data_inicio`
- `data_vencimento`
- `dispositivo_principal_id`
- `limite_dispositivos`
- `ultimo_pagamento_id`
- `entitlement_assinado`
- `grace_period_until`
- `created_at`
- `updated_at`

## Fluxo de compra/renovacao

1. Usuario com permissao `licencas.gerenciar` escolhe plano.
2. Sistema calcula valor e vencimento.
3. Desktop solicita criacao de pagamento ao backend.
4. Backend cria pagamento para conta de Rogerio.
5. PIX exibe QR Code; cartao usa checkout/Point conforme canal configurado.
6. Confirmacao atualiza `licencas`, `historico_licencas` e `pagamentos_licenca`.
7. Backend gera entitlement local assinado para funcionamento offline.
8. Desktop atualiza cache local.

## Operacao offline

- O sistema deve validar um entitlement local assinado.
- Deve existir periodo de tolerancia configuravel.
- Durante tolerancia, exibir alerta administrativo.
- Fora da tolerancia, bloquear apenas operacoes nao essenciais conforme politica.
- Auditoria e tentativa de renovacao entram em fila ate reconectar.

## Device binding

Dados sugeridos:

- hash composto de identificadores autorizados;
- tolerancia a troca parcial de hardware;
- score de similaridade;
- data da primeira ativacao;
- ultimo check-in;
- status do dispositivo: `autorizado`, `pendente`, `revogado`, `bloqueado`.

## Permissoes

- `licencas.visualizar`
- `licencas.gerenciar`
- `licencas.renovar`
- `licencas.cancelar`
- `licencas.bloquear_dispositivo`
- `licencas.autorizar_dispositivo`

## Rollback

- Manter licenciamento legado intacto durante coexistencia.
- NextGen deve iniciar em modo paralelo sem bloquear EquipeExe.
- A ativacao NextGen deve poder ser revertida removendo apenas registros NextGen e cache local.

## Pendencias

- Definir valores oficiais dos planos.
- Definir politica de tolerancia offline.
- Validar tratamento fiscal/contabil da venda de licenca.

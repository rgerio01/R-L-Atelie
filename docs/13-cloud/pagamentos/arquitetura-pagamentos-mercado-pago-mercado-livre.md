# Arquitetura de Pagamentos Mercado Pago / Mercado Livre

Data: 2026-05-23

## Objetivo

Definir a arquitetura segura para pagamentos do novo Atelie/EquipeExe NextGen, separando:

- licencas do sistema, recebidas por Rogerio;
- vendas de produtos e servicos, recebidas por Luci;
- taxa de servico transparente e auditavel de R$ 0,05 por venda de Luci para Rogerio;
- PIX, cartao, parcelamento e dinheiro;
- conciliacao, auditoria, cancelamento, estorno e fallback operacional.

## Principios obrigatorios

- Nenhum `Access Token`, `Client Secret` ou credencial sensivel deve ficar embutido no executavel desktop.
- O desktop chama uma API propria; a API propria chama Mercado Pago/Mercado Livre.
- Toda operacao de pagamento deve usar chave de idempotencia.
- Toda confirmacao deve ser reconciliada por status consultado na API e/ou webhook.
- Venda digital so deve ser finalizada apos confirmacao real do provedor.
- Dinheiro pode operar offline, mas deve gerar auditoria local e entrar em fila de sincronizacao.
- Licenca pode operar offline por periodo de tolerancia com entitlement local assinado, mas renovacao/ativacao exige conciliacao.

## Contas recebedoras

| Fluxo | Recebedor | Conta | Observacao |
|---|---|---|---|
| Licenca mensal/trimestral/semestral/anual | Rogerio | Mercado Pago/Mercado Livre de Rogerio | Receita de software/licenca |
| Venda de produto/servico | Luci | Mercado Pago/Mercado Livre de Luci | Receita operacional da loja |
| Taxa de servico por venda | Rogerio | Mercado Pago/Mercado Livre de Rogerio | R$ 0,05 por venda, registrada e auditavel |

## Componentes

- `Desktop App`: interface local, caixa, OS, venda, QR Code, status visual.
- `Payment API`: backend proprio, guarda credenciais e conversa com Mercado Pago.
- `Webhook Receiver`: recebe notificacoes de pagamento, cancelamento, estorno e expiracao.
- `Reconciliation Worker`: consulta transacoes pendentes, corrige divergencias e fecha conciliacao.
- `Local Queue`: fila offline para eventos locais, especialmente dinheiro e auditoria.
- `Audit Ledger`: trilha imutavel de usuario, caixa, terminal, conta recebedora e status.

## PIX

Fluxo:

1. Operador inicia venda/licenca.
2. Desktop envia valor, tipo, recebedor e referencia externa para `Payment API`.
3. Backend cria pagamento/ordem PIX no Mercado Pago usando credencial da conta correta.
4. Backend retorna QR Code/payload e identificador da transacao.
5. Desktop exibe QR Code.
6. Webhook/status confirma aprovacao, expiracao ou falha.
7. Desktop libera venda/licenca somente com status aprovado.

Dados obrigatorios:

- `external_reference` sem dados pessoais.
- `payment_provider_id`.
- `qr_code_payload`.
- `status`.
- `status_detail`.
- `receiver_account`.
- `created_by_user_id`.
- `cash_session_id` quando aplicavel.

## Cartao e maquininha Point

Arquitetura recomendada:

1. Backend lista/valida terminais autorizados vinculados a conta de Luci.
2. Operador escolhe terminal autorizado no Desktop.
3. Backend cria ordem Point com valor, parcelas e `terminal_id`.
4. A ordem e carregada no terminal em modo PDV.
5. Cliente paga presencialmente.
6. Sistema recebe status por webhook/consulta.
7. Venda fecha apenas apos confirmacao.

Observacao tecnica importante: o fluxo oficial de Point integrado e por ordem associada a terminal. A camada Bluetooth/local pode existir apenas para descoberta, status local ou UX, mas a autorizacao financeira deve passar por API/status/webhook do provedor ou SDK homologado.

## Dinheiro

Fluxo:

1. Operador seleciona dinheiro.
2. Sistema registra valor devido, valor recebido e troco.
3. Sistema exige caixa aberto e usuario autenticado.
4. Sistema grava pagamento local imediatamente.
5. Evento entra em auditoria/sincronizacao.
6. Fechamento de caixa consolida dinheiro fisico.

## Cancelamentos e estornos

- Cancelamento antes de pagamento: cancelar ordem pendente.
- Cancelamento apos aprovacao: estorno/reembolso conforme regra do provedor e permissao `vendas.estornar`.
- Cancelamento fiscal deve ser fluxo separado e exigir permissao fiscal.
- Toda divergencia vai para fila de conciliacao.

## Evidencias oficiais utilizadas

- Mercado Pago documenta que credenciais identificam a integracao e que o `Access Token` e chave privada usada no backend.
- Mercado Pago Point integrado processa pagamentos criando ordens associadas a terminal em modo PDV.
- A ordem retorna identificadores e status que devem ser salvos para consultas/notificacoes.

## Pendencias

- Confirmar tipo de conta e credenciais de Rogerio e Luci.
- Confirmar se sera integracao propria ou OAuth multi-conta.
- Confirmar modelo juridico/contabil da taxa de servico.
- Confirmar terminais Point disponiveis e modo PDV.
- Definir ambiente sandbox e usuarios de teste.

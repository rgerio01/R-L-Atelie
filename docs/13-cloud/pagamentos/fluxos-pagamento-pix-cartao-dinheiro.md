# Fluxos de Pagamento PIX, Cartao e Dinheiro

Data: 2026-05-23

## Fluxo PIX de venda

1. Caixa seleciona PIX.
2. Sistema cria pagamento para conta de Luci.
3. QR Code aparece na tela.
4. Sistema aguarda confirmacao por webhook/status.
5. Se aprovado, registra pagamento e finaliza venda.
6. Se expirado, permite gerar novo QR Code.
7. Se recusado, venda permanece pendente.

## Fluxo PIX de licenca

1. Administrador escolhe plano.
2. Sistema cria pagamento para conta de Rogerio.
3. QR Code aparece na tela.
4. Confirmacao ativa/renova licenca.
5. Entitlement local assinado e atualizado.

## Fluxo cartao por terminal Point

1. Caixa seleciona credito/debito e parcelas.
2. Sistema valida terminal autorizado.
3. Backend cria ordem Point para terminal.
4. Cliente paga no terminal.
5. Webhook/status confirma.
6. Sistema registra NSU/autorizacao/identificador da transacao.
7. Venda fecha somente com status aprovado.

## Fluxo dinheiro

1. Caixa seleciona dinheiro.
2. Sistema exige caixa aberto.
3. Operador informa valor recebido.
4. Sistema calcula troco.
5. Sistema grava pagamento, operador, caixa, data/hora e observacao.
6. Venda e finalizada localmente.
7. Evento entra em auditoria e sincronizacao.

## Falhas e contingencia

- Sem internet: PIX/cartao digital ficam indisponiveis ou pendentes; dinheiro pode continuar.
- Webhook indisponivel: worker consulta status periodicamente.
- Terminal offline: troca de terminal ou pagamento alternativo.
- Pagamento duplicado: idempotencia e reconciliacao.
- Divergencia de status: bloquear fechamento definitivo ate conciliacao.

## Status padrao

- `criado`
- `aguardando_cliente`
- `pendente`
- `aprovado`
- `recusado`
- `expirado`
- `cancelado`
- `estornado`
- `conciliado`
- `divergente`

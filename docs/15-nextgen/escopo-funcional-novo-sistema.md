# Escopo Funcional do Novo Sistema Atelie / EquipeExe NextGen

Data: 2026-05-23

## Objetivo

Criar um novo sistema do zero, preservando os fluxos essenciais identificados no EquipeExe e substituindo arquitetura, autenticação, licenciamento, pagamentos, auditoria e banco por estrutura moderna.

## Modulos funcionais

### Clientes

- cadastro completo;
- documentos;
- contatos;
- endereco;
- historico de OS/ROL;
- historico financeiro;
- observacoes;
- status ativo/inativo.

### Produtos e servicos

- cadastro de produtos;
- cadastro de servicos;
- tabela de precos;
- categorias;
- unidades;
- estoque vinculado;
- status;
- historico de alteracao.

### OS / ROL / Ordens de servico

- abertura;
- itens/pecas/servicos;
- valores;
- descontos;
- status;
- entrega;
- cancelamento;
- impressao;
- historico.

### Vendas

- venda de produto;
- venda de servico;
- pagamento PIX;
- pagamento cartao;
- dinheiro;
- parcelamento;
- cancelamento/estorno;
- taxa de servico configuravel.

### Financeiro e caixa

- abertura de caixa;
- fechamento;
- sangria;
- recebimentos;
- contas a receber;
- contas a pagar;
- duplicatas;
- creditos de cliente;
- relatorios.

### Estoque

- entrada;
- saida;
- ajuste;
- baixa por venda/OS;
- devolucao;
- estoque minimo;
- historico de movimentacao.

### Licenciamento

- planos mensal, trimestral, semestral e anual;
- PIX;
- cartao;
- dinheiro manual;
- vencimento;
- status;
- dispositivos autorizados;
- historico de pagamentos.

### Usuarios e permissoes

- perfis;
- permissoes por modulo/tela/botao/relatorio;
- auditoria;
- bloqueio de acoes criticas.

### Pagamentos Mercado Pago/Mercado Livre

- conta Rogerio: licencas e taxas de servico;
- conta Luci: vendas de produtos e servicos;
- backend intermediario;
- webhooks;
- reconciliacao.

### Auditoria

- toda venda;
- todo cancelamento;
- todo desconto;
- toda alteracao de permissao;
- todo pagamento;
- toda taxa;
- todo repasse;
- todo erro de integracao.

## Principio operacional

O sistema deve ser offline-first para cadastros e operacao local, mas pagamentos digitais dependem de confirmacao remota antes de liberar venda/licenca.

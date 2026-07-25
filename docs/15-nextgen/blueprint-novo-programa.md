# Blueprint do Novo Programa Atelie / EquipeExe NextGen

Data: 2026-05-23

## Visao

Novo sistema desktop corporativo para operacao de loja/atelie/lavanderia/servicos, com cadastro, OS/ROL, vendas, pagamentos, caixa, estoque, financeiro, auditoria, licenciamento proprio e integracao Mercado Pago.

## Experiencia principal

Primeira tela apos login:

- dashboard operacional compacto;
- caixa atual;
- OS/ROL em aberto;
- vendas pendentes;
- alertas de licenca;
- pagamentos pendentes;
- sincronizacao;
- atalhos para cliente, venda, OS, caixa e relatorios.

## Navegacao

- Clientes
- OS/ROL
- Produtos
- Estoque
- Vendas
- Caixa
- Financeiro
- Fiscal
- Relatorios
- Administracao
- Licenca
- Auditoria
- Configuracoes

## Fluxo de venda

1. Selecionar cliente opcional/obrigatorio conforme regra.
2. Adicionar produto/servico.
3. Aplicar desconto se permitido.
4. Escolher pagamento.
5. Confirmar PIX/cartao/dinheiro.
6. Registrar taxa de servico quando venda for de Luci.
7. Atualizar financeiro/estoque.
8. Emitir comprovante/relatorio.

## Fluxo de OS/ROL

1. Criar atendimento para cliente.
2. Registrar itens/servicos.
3. Definir status.
4. Vincular valores.
5. Finalizar com pagamento ou gerar financeiro.
6. Imprimir/consultar historico.

## Auditoria

Registrar:

- quem vendeu;
- quem cancelou;
- quem recebeu;
- quem alterou valor;
- quem deu desconto;
- quem alterou permissao;
- quem gerou licenca;
- quem recebeu pagamento;
- forma de pagamento;
- conta recebedora;
- taxa aplicada;
- dispositivo usado.

## Licenciamento

- plano escolhido dentro do sistema;
- vencimento visivel;
- QR Code PIX/cartao quando configurado;
- historico de pagamentos;
- dispositivos autorizados;
- alerta de vencimento;
- cache offline assinado.

## Regras de seguranca

- perfil minimo necessario;
- separacao entre administracao do sistema e operacao de loja;
- credenciais no backend;
- logs mascarados;
- auditoria imutavel;
- exportacao controlada.

## Primeira versao recomendada

MVP controlado:

- usuarios/perfis;
- clientes;
- produtos;
- OS/ROL basico;
- venda;
- caixa;
- PIX;
- dinheiro;
- auditoria;
- licenca local com pagamento PIX;
- importacao readonly do Paradox.

Cartao/Point entra depois de validar credenciais, terminais e ambiente de testes.

# Blueprint de Dominio NextGen

Data: 2026-05-23

## Objetivo

Definir o dominio de negocio da futura geracao do EquipeExe com base no legado.

## Agregados principais

### Cliente

Responsabilidade:

- cadastro;
- contatos;
- enderecos;
- documentos;
- observacoes;
- grupo/tabela de preco;
- historico;
- vinculo com funcionarios/pecas quando aplicavel.

Entidades:

- `Customer`
- `CustomerContact`
- `CustomerAddress`
- `CustomerNote`
- `CustomerGroup`
- `CustomerEmployee`
- `CustomerGarment`

Origem legada:

- `Clientes`
- `CliContato`
- `ClientesObs`
- `GruClientes`
- `FunCli`
- `FunCliRou`

### Ordem de Servico / ROL

Responsabilidade:

- abertura;
- itens/pecas;
- servicos;
- status/localizacao;
- entrega;
- cancelamento;
- impressao;
- totais.

Entidades:

- `ServiceOrder`
- `ServiceOrderItem`
- `ServiceOrderStatusHistory`
- `ServiceOrderLocation`
- `ServiceOrderPaymentLink`
- `ServiceOrderPrintEvent`

Origem legada:

- `MovCab`
- `MovLocRol`
- `CadLocRol`
- `ControleEti`
- `IndenRol`
- `MovRoupa*`
- `MovProc*`

### Produto / Servico

Responsabilidade:

- cadastro de produto/servico;
- preco;
- grupo;
- unidade;
- pacote/kit;
- relacao com estoque;
- relacao com ROL/OS.

Entidades:

- `Product`
- `Service`
- `PriceTable`
- `ProductGroup`
- `ProductKit`
- `ProductPackage`

Origem legada:

- `Produt`
- `ProdEst`
- `TabProdEst`
- `ProdEstKit`
- `ProdEstPac`

### Estoque

Responsabilidade:

- saldo;
- entrada;
- saida;
- ajuste;
- cancelamento;
- encerramento;
- historico.

Entidades:

- `StockItem`
- `StockMovement`
- `StockAdjustment`
- `StockClosing`
- `StockCancellation`

Origem legada:

- `ProdEst`
- `MovEst`
- `MovEstCan`
- `MovEstEnc`

### Financeiro

Responsabilidade:

- caixa;
- pagamento;
- recebimento;
- duplicatas;
- boletos;
- credito de cliente;
- contas a pagar;
- fechamento.

Entidades:

- `CashSession`
- `CashMovement`
- `Receivable`
- `Payable`
- `Payment`
- `CustomerCredit`
- `InvoicePayment`

Origem legada:

- `Duplicat`
- `Boletos`
- `DupBoleto`
- `CliCredito`
- `FecCaixa`
- `MovIniCaixa`
- `Titulos`
- `TitGru`

### Fiscal

Responsabilidade:

- nota fiscal;
- SAT/NFE;
- cancelamento fiscal;
- logs fiscais;
- integracao com componentes de terceiros.

Entidades:

- `Invoice`
- `FiscalDocument`
- `FiscalCancellation`
- `FiscalDeviceEvent`

Origem legada:

- `Notas`
- `NotaFisPag`
- `NotaSat`
- `NotaSatCanc`
- tabelas NFE/SAT.

### Permissao e Auditoria

Responsabilidade:

- usuarios;
- grupos;
- perfis;
- permissoes granulares;
- trilha de auditoria.

Entidades:

- `User`
- `Role`
- `Permission`
- `UserRole`
- `AuditEvent`

Origem legada:

- `Usuarios`
- `Senhas`
- `Nivel`
- `GruUsuarios`
- tabelas de cancelamento/ocorrencia/log.

## Regras de modelagem

- Toda entidade deve possuir `id` moderno e manter `legacy_key`.
- Toda tabela migrada deve preservar origem: `legacy_table`, `legacy_path`, `legacy_hash`.
- Alteracoes criticas devem gerar `audit_events`.
- Cancelamento nao deve apagar registros; deve alterar status e registrar motivo.
- Valores monetarios devem usar decimal fixo.
- Datas do legado devem ser normalizadas com timezone local.

## Conclusao

O dominio nextgen deve preservar o eixo:

`Cliente -> OS/ROL -> Itens/Servicos -> Pagamento/Nota -> Estoque/Relatorio/Auditoria`.

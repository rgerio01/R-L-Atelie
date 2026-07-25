# Mapa Clientes, OS/ROL, Produtos, Financeiro e Estoque

Data: 2026-05-23

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\mapa-clientes-os-produtos.csv`

## Cliente

Tabela principal:

- `Ger\Dados\Clientes.DB`

Chave candidata:

- `CodCli`

Campos principais:

- `NomCli`
- `EndCli`
- `CidCli`
- `EstCli`
- `CepCli`
- `TelCli`
- `TelCli2`
- `Contato`
- `CgcCli`
- `DigCli`
- `GruCli`
- `CodTab`
- `InsEstCli`
- `InsMunCli`

Tabelas complementares:

- `CliContato`: contatos, cargo, setor, telefone, celular, email, observacao.
- `ClientesObs`: observacoes por cliente.
- `FunCli`: funcionarios do cliente.
- `FunCliRou`: roupas/pecas vinculadas ao funcionario/cliente.
- `GruClientes*`: grupos de clientes e descontos.

Impactos:

- OS/ROL por `CodCli`;
- financeiro por `CodCli`;
- notas/fiscal por `CodCli`;
- relatorios de movimento/frequencia/faturamento.

## OS / ROL

Tabela principal candidata:

- `Lav\FILIAL\MovCab.DB`

Chaves candidatas:

- `ROL`
- `NumOS`

Campos principais:

- `ROL`
- `CodCli`
- `DatEntLoja`
- `CodTab`
- `CodTipSer`
- `CodTipEnt`
- `CodPra`
- `NumGR`
- `NumOS`
- `DatLan`
- `CodVen`
- `DatEnt`
- `ValTot`
- `TotPecas`
- `Posicao`
- `NumNot`
- `CodUsuario`

Tabelas complementares:

- `MovLocRol`: localizacao do ROL.
- `ControleEti`: etiquetas e controle de produto/pecas.
- `IndenRol`: indenizacoes por ROL.
- `MovControle`: solicitacoes/controle/ocorrencias.
- `CadLocRol`: locais/status possiveis.
- `MovRoupa*`, `MovProc*`, `MovTri*`: itens/processos/triagem a validar.

Fluxo provavel:

1. Entrada/lancamento cria `MovCab`.
2. Itens/pecas/servicos sao gravados em tabelas de movimento/roupa/processo.
3. Localizacao/status muda em `MovLocRol`, `Posicao` ou tabelas de processo.
4. Pagamento/nota cruza `NumNot`, `ValTot`, `CodCli` e financeiro.
5. Entrega atualiza status/data/localizacao.

## Produto / Servico

Tabelas candidatas:

- `Ger\Filial\Produt.DB`
- `EST\DADOS\ProdEst.DB`
- `EST\DADOS\TabProdEst.DB`
- `EST\DADOS\ProdEstKit.DB`
- `EST\DADOS\ProdEstPac.DB`

Chaves candidatas:

- `CodPro`
- `CodProEst`

Campos importantes:

- descricao;
- grupo;
- unidade;
- tabela de preco;
- cancelado/status;
- classe/subclasse;
- imagem;
- centro de custo.

Impactos:

- itens do ROL/OS;
- tabela de preco;
- estoque;
- fiscal;
- relatorios por produto/servico.

## Estoque

Tabelas:

- `EST\FILIAL\MovEst.DB`
- `EST\FILIAL\MovEstCan.DB`
- `EST\FILIAL\MovEstEnc.DB`
- `Lav\FILIAL\MovEstLan.DB`
- `EST\DADOS\ProdEst.DB`

Movimento:

- `SeqLan`
- `CodEst`
- `CodProEst`
- `DatLan`
- `Qde`
- `TipoES`
- `ValTot`
- `ValUnit`
- `Cancelado`
- `CodUsuario`

Regras pendentes:

- se baixa e automatica por venda/ROL;
- como devolucao retorna ao estoque;
- como encerramento calcula saldo;
- como custo medio e formado.

## Financeiro

Tabelas:

- `REC\FILIAL\Duplicat.DB`
- `Lav\FILIAL\Notas.DB`
- `Lav\FILIAL\NotaFisPag.DB`
- `Lav\FILIAL\CliCredito.DB`
- `Lav\FILIAL\FecCaixa.DB`
- `Lav\FILIAL\MovIniCaixa.DB`
- `PAG\FILIAL\Titulos.DB`

Vinculos:

- cliente por `CodCli`;
- ROL por `Rol` em `CliCredito` e possivelmente `NumNot`/`NumFat`;
- caixa por `SeqCai`;
- nota por `NumNot`/`NumNotFis`.

Regras pendentes:

- parcelamento;
- desconto;
- credito;
- sangria;
- fechamento;
- estorno/cancelamento.

## Conclusao

O eixo mais importante da reconstrucao e:

`Clientes.CodCli -> MovCab.ROL/CodCli -> itens/produtos/servicos -> financeiro/notas/duplicatas -> estoque/relatorios`.

Esse eixo deve ser validado dinamicamente antes de qualquer migracao definitiva.

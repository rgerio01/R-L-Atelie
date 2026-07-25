# Mapa de Valores e Calculos

Data: 2026-05-23

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\mapa-valores-calculos.csv`

## Resultado

Campos classificados como valor financeiro: 147.

Tambem foi executado perfilamento readonly das tabelas prioritarias. Foram geradas 33 estatisticas de valor/quantidade, sem alterar a base.

Distribuicao por dominio:

- Configuracao/Admin: 73
- Financeiro: 25
- Nao classificado: 20
- Clientes: 9
- Notas/Fiscal: 9
- Produtos/Servicos: 5
- Movimentos/OS/ROL: 4
- Estoque: 2

## Campos monetarios criticos

### OS/ROL

Campos candidatos:

- `MovCab.ValTot`
- campos de desconto/acrescimo/valor em tabelas de itens ou movimentos.

Ocorrencias readonly:

- `MovCab.ValTot`: 31.947 ocorrencias, minimo 0, maximo 3000.
- `MovCab.TotPecas`: 31.971 ocorrencias, minimo 0, maximo 48.
- `MovCab.DescontoROL`: 31.971 ocorrencias, minimo 0, maximo 50.
- `MovCab.DescontoValor`: 31.971 ocorrencias, minimo 0, maximo 130.

Status:

- confirmado por schema como campo de valor;
- formula de calculo nao confirmada.

Validacao:

- criar ROL teste com itens e desconto no MOD;
- comparar antes/depois em `MovCab` e tabelas de itens;
- verificar relatorio e impressao.

### Financeiro

Campos candidatos:

- `Duplicat.ValFat`
- `Duplicat.ValDup`
- `Duplicat.ValDupPag`
- `Boletos.ValBol`
- `CliCredito.ValCre`
- `Titulos.ValTot`
- `Titulos.ValTit`
- `Titulos.ValTitPag`

Ocorrencias readonly:

- `Duplicat.ValFat`: 19.717 ocorrencias, minimo -15, maximo 1500.
- `Duplicat.ValDup`: 19.717 ocorrencias, minimo -15, maximo 1500.
- `Duplicat.ValDupPag`: 19.251 ocorrencias, minimo -15, maximo 1170.
- `CliCredito.ValCre`: 106 ocorrencias, minimo 0, maximo 200.
- `Titulos.ValTit`: 103 ocorrencias, minimo 3, maximo 2279,73.

Status:

- confirmado por schema.
- relacao entre valor bruto, duplicata, pago, baixa e saldo precisa validação por fluxo.

### Fiscal

Campos candidatos:

- `Notas.ValNot`
- `Notas.ValBasNot`
- `NotaFisPag.ValNot`
- `NotaFisPag.ValBasNot`
- `NotaSat.ValNot`

Ocorrencias readonly:

- `Notas.ValNot`: 21.679 ocorrencias, minimo -15, maximo 1500.
- `Notas.ValBasNot`: 21.679 ocorrencias, minimo -15, maximo 1500.

Pendencia:

- confirmar se valores fiscais espelham financeiro ou sao recalculados por emissao.

### Estoque

Campos candidatos:

- `MovEst.ValTot`
- `MovEst.ValUnit`
- `TabProdEst.PreUni`

Ocorrencias readonly:

- `MovEst.Qde`: 1 ocorrencia, valor 0.
- `MovEst.ValTot`: 1 ocorrencia, valor 0.
- `MovEst.ValUnit`: 1 ocorrencia, valor 0.

Pendencia:

- confirmar custo medio, preco de venda e impacto financeiro.

## Regras de calculo pendentes

Ainda precisam de validação dinâmica:

- total de ROL;
- total por item/servico;
- desconto por valor;
- desconto percentual;
- acrescimos;
- juros/multa;
- valor pago;
- valor aberto;
- valor cancelado;
- estorno;
- baixa de estoque por valor.

## Regra de documentacao

Cada calculo confirmado futuramente deve registrar:

- tela;
- botao/acao;
- tabelas lidas;
- tabelas alteradas;
- campos origem;
- campos destino;
- formula;
- arredondamento;
- usuario;
- log gerado;
- exemplo antes/depois.

# Mapa de Fluxos Operacionais

Data: 2026-05-23

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\mapa-fluxos-operacionais.csv`
- `D:\AtelieProd\MOD\docs\11-modulos\matriz-regras-negocio.csv`
- `D:\AtelieProd\MOD\docs\11-modulos\matriz-relatorios.csv`

## Fluxo 1 - Cadastro de Cliente

Inicio:

- menu/tela de Clientes no LavSoft ou modulo administrativo.

Dados:

- `Clientes.DB`
- `CliContato.DB`
- `ClientesObs.DB`
- `GruClientes*.DB`

Regras a validar:

- obrigatoriedade de `NomCli`;
- validacao de CPF/CNPJ em `CgcCli`/`DigCli`;
- duplicidade por documento/nome;
- status ativo/cancelado;
- impacto em financeiro e ROL.

Resultado:

- cliente fica disponivel para ROL/OS, financeiro, notas e relatorios.

## Fluxo 2 - Criacao de OS/ROL

Inicio:

- Entrada/Lancamento de ROL.

Dados:

- `MovCab.DB`
- tabelas de itens/roupas/processos;
- `Clientes.DB`;
- `Produt.DB`;
- `Notas.DB`/financeiro quando houver pagamento/nota.

Regras a validar:

- geracao do numero `ROL`;
- uso de `NumOS`;
- preenchimento de cliente;
- data de entrada/previsao/entrega;
- total de pecas;
- calculo de `ValTot`;
- status inicial em `Posicao`.

Resultado:

- ROL aberto para producao, entrega, pagamento e relatorios.

## Fluxo 3 - Alteracao de Status / Localizacao

Inicio:

- telas de localizacao, passadoria, terceirizacao, controle de lavagem, marca entrega.

Dados:

- `MovCab.Posicao`;
- `MovLocRol`;
- `CadLocRol`;
- tabelas de processo/triagem.

Regras a validar:

- quais status existem;
- quem pode alterar;
- se existe historico por data/hora;
- impacto em entrega e relatorios.

## Fluxo 4 - Pagamento e Caixa

Inicio:

- Pagamento de ROL, Caixa Dia a Dia, Controle de Caixa.

Dados:

- `Duplicat`;
- `CliCredito`;
- `FecCaixa`;
- `MovIniCaixa`;
- `Notas`;
- `NotaFisPag`.

Regras a validar:

- formas de pagamento;
- descontos;
- creditos;
- baixa;
- fechamento;
- cancelamento/estorno;
- emissao fiscal.

## Fluxo 5 - Estoque

Inicio:

- Entrada no estoque, baixa, atualiza estoque, encerramento.

Dados:

- `ProdEst`;
- `MovEst`;
- `MovEstCan`;
- `MovEstEnc`;
- `TabProdEst`.

Regras a validar:

- tipo entrada/saida em `TipoES`;
- saldo anterior/novo;
- estoque minimo/maximo;
- baixa por venda/OS;
- cancelamento de movimento.

## Fluxo 6 - Relatorios

Inicio:

- menus de relatorios por modulo.

Dados candidatos:

- `MovCab`;
- `Clientes`;
- `Produt`;
- `MovEst`;
- `Duplicat`;
- `Notas`;
- `NotaFisPag`.

Regras a validar:

- filtros;
- agrupamentos;
- totais;
- permissao por relatorio;
- formato de impressao/exportacao.

## Fluxo 7 - Permissoes e Auditoria

Inicio:

- `Senhas.exe` e telas de usuarios/permissoes.

Dados:

- `Usuarios`;
- `Senhas`;
- `Nivel`;
- `GruUsuarios`.

Regras a validar:

- significado de `NivelI`, `NivelA`, `NivelE`, `NivelT`;
- permissoes por filial;
- bloqueios por sistema;
- logs de alteracao.

## Conclusao

Os fluxos devem ser migrados por eixo, nao por tela isolada. A ordem recomendada e:

1. Clientes.
2. ROL/OS.
3. Produtos/servicos.
4. Pagamento/financeiro.
5. Estoque.
6. Relatorios.
7. Permissao/auditoria.

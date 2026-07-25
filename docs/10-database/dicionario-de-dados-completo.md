# Dicionario de Dados Completo - EquipeExe

Data: 2026-05-23

## Escopo

Este documento consolida a leitura do banco Paradox/BDE extraido em copia somente leitura.

Fonte principal:

- `D:\AtelieProd\MOD\docs\03-banco-de-dados\dicionario-paradox-tabelas.csv`
- `D:\AtelieProd\MOD\docs\03-banco-de-dados\dicionario-paradox-colunas.csv`

Arquivo detalhado gerado:

- `D:\AtelieProd\MOD\docs\10-database\dicionario-de-dados-completo.csv`
- `D:\AtelieProd\MOD\docs\10-database\mapa-entidades-dominio.csv`

## Resultado geral

- Tabelas extraidas: 478
- Colunas extraidas: 4.996

Classificacao por dominio:

| Dominio | Tabelas | Campos |
|---|---:|---:|
| config/admin | 150 | 1.371 |
| outro/a validar | 161 | 1.284 |
| cliente | 25 | 631 |
| ordem_servico_rol | 28 | 447 |
| fiscal | 36 | 385 |
| produto_servico | 36 | 384 |
| financeiro | 19 | 290 |
| estoque | 10 | 102 |
| permissao/auth | 8 | 52 |
| auditoria/log | 5 | 50 |

## Clientes

Tabelas fortes:

- `Ger\Dados\Clientes.DB`
- `Ger\Dados\Clientes-.DB`
- `Ger\Dados\Backup\Clientes.DB`
- `Ger\Dados\CliContato.DB`
- `Ger\Dados\CliCont.DB`
- `Ger\Dados\ClientesObs.DB`
- `Lav\FILIAL\EstCli.DB`
- `Ger\Dados\FunCli.DB`
- `PEC\DADOS\FunCli.DB`
- `Ger\Dados\FunCliRou.DB`
- `PEC\DADOS\FunCliRou.DB`
- `Ger\Dados\GruClientes*.DB`

Campos principais em `Clientes.DB`:

- `CodCli`
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
- `EndCobCli`
- `EndEntCli`
- `CidCobCli`

Leitura funcional:

- `CodCli` e o identificador interno forte do cliente.
- `NomCli` e o nome visual/operacional.
- `CgcCli`/`DigCli` aparentam compor documento fiscal/CPF/CNPJ.
- Existem campos para endereco principal, cobranca e entrega.
- `CliContato` expande contatos, telefone, celular, email e observacoes.
- `ClientesObs` guarda observacoes extensas por cliente.
- `FunCli` e `FunCliRou` indicam vinculo cliente -> funcionario/roupa/peca, importante para lavanderia corporativa/uniformes.

## Ordens de servico / ROL

Tabelas fortes:

- `Lav\FILIAL\MovCab.DB`
- `Lav\FILIAL\MovLocRol.DB`
- `Lav\FILIAL\ControleEti.DB`
- `Lav\FILIAL\IndenRol.DB`
- `Ger\Filial\MovControle.DB`
- `Lav\Dados\CadLocRol.DB`
- tabelas `MovRoupa*`, `MovProc*`, `MovTri*`, `OrcCab`

Campos principais em `MovCab.DB`:

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
- `Unidade`
- `CodUsuario`

Leitura funcional:

- `ROL` e o numero operacional central do fluxo de lavanderia/OS.
- `CodCli` vincula o ROL/OS ao cliente.
- `DatEntLoja`, `DatLan`, `DatEnt` indicam abertura/entrada/previsao/entrega.
- `ValTot` e `TotPecas` indicam total financeiro e quantidade.
- `Posicao` e candidato forte a status/localizacao do ROL.
- `NumOS` sugere compatibilidade com ordem de servico formal.
- `CodUsuario` registra usuario responsavel por lancamento/acao.

## Produtos e servicos

Tabelas fortes:

- `Ger\Filial\Produt.DB`
- `EST\DADOS\ProdEst.DB`
- `Importa\EST\DADOS\ProdEst.DB`
- `EST\DADOS\ProdEstKit.DB`
- `EST\DADOS\ProdEstPac.DB`
- `EST\DADOS\TabProdEst.DB`
- tabelas de cores, marcas, tecidos, servicos e grupos identificadas por dominio `produto_servico`.

Leitura funcional:

- `Produt` e candidato a produto/servico operacional usado no LavSoft.
- `ProdEst` representa produto de estoque, com campos de grupo, classe, imagem, centro de custo e status.
- `TabProdEst` guarda preco por tabela/produto.
- Kits e pacotes de produto existem em `ProdEstKit` e `ProdEstPac`.

## Estoque

Tabelas fortes:

- `EST\DADOS\ProdEst.DB`
- `EST\FILIAL\MovEst.DB`
- `Importa\EST\FILIAL\MovEst.DB`
- `EST\FILIAL\MovEstCan.DB`
- `EST\FILIAL\MovEstEnc.DB`
- `Lav\FILIAL\MovEstLan.DB`

Campos principais em `MovEst.DB`:

- `SeqLan`
- `CodEst`
- `CodProEst`
- `DatLan`
- `Qde`
- `TipoES`
- `ValTot`
- `ValUnit`
- `TipInteg`
- `CodInteg`
- `Obs`
- `Cancelado`
- `CodUsuario`
- `CodClaEst`

Leitura funcional:

- `CodProEst` vincula movimento ao produto de estoque.
- `Qde`, `TipoES`, `ValTot`, `ValUnit` sustentam entrada/saida/valor.
- `Cancelado`, `MotivoCan`, `DatCan`, `CodUsuario` aparecem em tabelas de cancelamento.
- `MovEstEnc` registra encerramento/saldo por data.

## Financeiro

Tabelas fortes:

- `REC\FILIAL\Duplicat.DB`
- `REC\FILIAL\Boletos.DB`
- `REC\FILIAL\DupBoleto.DB`
- `Lav\FILIAL\CliCredito.DB`
- `Lav\FILIAL\FecCaixa.DB`
- `Lav\FILIAL\MovIniCaixa.DB`
- `Lav\FILIAL\Notas.DB`
- `Lav\FILIAL\NotaFisPag.DB`
- `PAG\FILIAL\Titulos.DB`
- `PAG\FILIAL\TitGru.DB`

Campos principais em `Duplicat.DB`:

- `NumFat`
- `NumDup`
- `DatEmi`
- `DatVen`
- `ValFat`
- `ValDup`
- `EmiBol`
- `Baixa`
- `DatPag`
- `ValDupPag`
- `CodCli`
- `CodFpg`
- `CodCpg`
- `CodBan`
- `SeqCai`

Leitura funcional:

- `Duplicat` representa contas a receber/faturas/duplicatas.
- `CodCli` vincula financeiro ao cliente.
- `DatVen`, `Baixa`, `DatPag`, `ValDupPag` sustentam vencimento, baixa e pagamento.
- `CliCredito` guarda credito do cliente, inclusive referencia a `Rol`.
- `FecCaixa` e `MovIniCaixa` sustentam abertura/fechamento/valor de caixa.
- `Titulos`/`TitGru` sustentam contas a pagar.

## Fiscal

Tabelas fortes:

- `Lav\FILIAL\Notas.DB`
- `Lav\FILIAL\NotaFisPag.DB`
- `SAT\FILIAL\NotaSat.DB`
- `SAT\FILIAL\NotaSatCanc.DB`
- tabelas NFE/SAT e documento fiscal correlatas.

Leitura funcional:

- Fiscal cruza cliente, valor, usuario, cancelamento e nota.
- Deve ser separado cuidadosamente de licencas de SDKs fiscais externos.

## Autenticacao e permissao

Tabelas fortes:

- `Ger\Dados\Usuarios.DB`
- `Ger\Dados\Senhas.DB`
- `Ger\Dados\Nivel.DB`
- `Ger\Dados\GruUsuarios.DB`
- `UsuaSis`, `UsuaFil`, `UsuaGru`, `UsuaGruInt`

Leitura funcional:

- Permissao opera por sistema/rotina/opcao, usuario e filial.
- A matriz moderna deve migrar operacoes como `AlteraCliente1`, `CFAbertura1`, `CFFechamento1`, etc.

## Auditoria, historico e logs

Tabelas fortes/candidatas:

- `Anotacoes`
- tabelas `*Can` de cancelamento;
- tabelas `*Ocor` de ocorrencia;
- tabelas `Log` e `EstatLavSoft`.

Leitura funcional:

- O legado guarda historico de forma distribuida, muitas vezes em tabela paralela de cancelamento ou ocorrencia.
- A nova arquitetura deve centralizar auditoria sem perder o historico especifico por modulo.

## Limites desta fase

Este dicionario e profundo no nivel de schema/campo, mas ainda nao substitui:

- amostragem estatistica de registros;
- validacao de indices/chaves Paradox;
- captura dinamica tela -> query;
- validacao de regras de calculo por execucao assistida.

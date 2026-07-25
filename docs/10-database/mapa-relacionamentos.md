# Mapa de Relacionamentos - EquipeExe

Data: 2026-05-23

## Fonte

Relacionamentos inferidos a partir de:

- nomes de tabelas;
- nomes de campos;
- caminhos fisicos dos bancos;
- mapas UI -> banco;
- strings funcionais dos executaveis.

CSV detalhado:

- `D:\AtelieProd\MOD\docs\10-database\mapa-relacionamentos.csv`

## Relacionamentos centrais

### Cliente -> OS/ROL

Chave candidata:

- `CodCli`

Tabelas de cliente:

- `Clientes`
- `CliContato`
- `ClientesObs`
- `FunCli`
- `FunCliRou`

Tabelas de OS/ROL:

- `MovCab`
- `MovControle`
- `MovLocRol`
- `ControleEti`
- `IndenRol`
- `MovRoupa*`
- `MovProc*`

Leitura:

`MovCab.CodCli` e a evidencia mais forte de relacionamento cliente -> ROL. O ROL parece ser a ordem operacional principal da lavanderia.

### Cliente -> Financeiro

Chave candidata:

- `CodCli`

Tabelas:

- `Duplicat`
- `Notas`
- `NotaFisPag`
- `CliCredito`
- `NotaSat`

Leitura:

Financeiro e fiscal carregam `CodCli`, permitindo rastrear faturamento, duplicatas, creditos e notas por cliente.

### OS/ROL -> Produto/Servico

Chaves candidatas:

- `ROL`
- `CodPro`
- `SeqPro`
- `RolSeqPro`
- `CodTipSer`
- `CodTipEnt`

Tabelas:

- `MovCab`
- `MovRoupa*`
- `ControleEti`
- `Produt`
- tabelas de servico/tipo/preco.

Leitura:

`MovCab` guarda cabecalho e totais. Tabelas de roupa/itens/processamento devem guardar pecas, produtos ou servicos vinculados ao ROL.

### Produto -> Estoque

Chaves candidatas:

- `CodPro`
- `CodProEst`

Tabelas:

- `Produt`
- `ProdEst`
- `MovEst`
- `MovEstCan`
- `MovEstEnc`
- `TabProdEst`

Leitura:

`Produt` parece produto/servico operacional. `ProdEst` parece produto de estoque. `MovEst` registra movimento por produto de estoque.

### OS/ROL -> Financeiro

Chaves candidatas:

- `ROL`
- `NumNot`
- `NumFat`
- `SeqCai`
- `CodCli`

Tabelas:

- `MovCab`
- `CliCredito`
- `Notas`
- `Duplicat`
- `FecCaixa`

Leitura:

`MovCab.ValTot` e `MovCab.NumNot` conectam a ordem operacional a nota/faturamento. `CliCredito` contem `Rol`, conectando credito a ROL.

### Financeiro -> Relatorios

Tabelas candidatas:

- `Duplicat`
- `Notas`
- `NotaFisPag`
- `Boletos`
- `Titulos`
- `FecCaixa`

Leitura:

Relatorios financeiros devem puxar duplicatas, notas, pagamentos, titulos e caixa.

## Relacionamentos de auditoria

Padroes encontrados:

- tabelas `*Can`: cancelamento;
- tabelas `*Ocor`: ocorrencia;
- campos `Cancelado`, `DatCan`, `MotivoCan`, `CodUsuario`;
- campos `DatLan`, `HorLan`, `CodUsuario`.

Leitura:

Auditoria no legado e espalhada por tabelas de evento/cancelamento. Na nova geracao, manter tabelas especificas por dominio e tambem uma `audit_events` central.

## Validacoes pendentes

- Confirmar indices Paradox (`.PX`, `.XG*`, `.YG*`).
- Executar amostragem de registros por tabelas criticas.
- Abrir telas e capturar arquivos acessados por ProcMon.
- Confirmar se `NumOS` e usado como OS formal ou campo auxiliar ao ROL.
- Confirmar relacao entre `Produt.CodPro` e `ProdEst.CodProEst`.

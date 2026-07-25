# Dicionario de Dados - Fase 2

## Copia readonly

Foi criada copia isolada dos arquivos Paradox/BDE em:

`D:\AtelieProd\MOD\data\original-readonly\Equipexe`

Manifesto:

`D:\AtelieProd\MOD\docs\03-banco-de-dados\copia-readonly-paradox.csv`

Arquivos copiados: 4.562.

## Extracao via ODBC Paradox

Driver usado: `Microsoft Paradox Driver (*.db )` 32-bit.

Resultado:

- Tabelas extraidas: 478.
- Colunas extraidas: 4.996.
- Tabelas com falha: 485.

Arquivos gerados:

- `dicionario-paradox-tabelas.csv`
- `dicionario-paradox-colunas.csv`
- `dicionario-paradox-falhas.csv`

## Observacao tecnica

O metodo `GetSchema('Columns')` do driver Paradox retornou recurso nao implementado. A extracao foi refeita usando `SELECT * FROM [Tabela] WHERE 1 = 0` e leitura de `GetSchemaTable()`.

As falhas restantes devem ser analisadas por categoria. A principal suspeita e limitacao do driver, versao/formato da tabela, arquivo auxiliar ausente, ou tabela que exige acesso BDE nativo.

## Modulos com maior volume de tabelas extraidas

- `Ger`: 196 tabelas.
- `Lav`: 101 tabelas.
- `SAT`: 45 tabelas.
- `EST`: 37 tabelas.
- `Estruturas`: 20 tabelas.
- `REC`: 17 tabelas.
- `ESC`: 15 tabelas.
- `PEC`: 15 tabelas.

## Tabelas operacionais relevantes

- `Ger\Dados\Clientes.DB`: 124 colunas.
- `Lav\FILIAL\MovCab.DB`: 58 colunas.
- `REC\FILIAL\Duplicat.DB`: 60 colunas.
- `Lav\FILIAL\Notas.DB`: 37 colunas.
- `Ger\Dados\Usuarios.DB`: usuarios e senha legada.
- `Ger\Dados\Nivel.DB`: permissoes por usuario/sistema/rotina/operacao.
- `Ger\Dados\UsuaSis.DB`: usuarios por sistema.
- `Ger\Dados\Senhas.DB`: estrutura de senha/opcao, sem registros na copia analisada.

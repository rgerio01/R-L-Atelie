# Validacao de Registros - ISO Linux - 2026-05-26

## Resultado

A ISO `D:\AtelieProd\atelie-linux.iso` foi gerada com a base SQLite ja migrada em
`/opt/atelie/data/pdv.db`.

## Contagens validadas no pacote `dist-linux`

- Usuarios legados: 9
- Clientes: 5.069
- ROLs / vendas / ordens: 31.972
- Itens de ROL: 174.558
- Financeiro / duplicatas: 19.717
- Servicos / tabela de precos: 3.693
- Catalogo de arquivos legados preservados: 8

## Arquivos brutos preservados na ISO

Os CSVs exportados dos `.DB` Paradox ficam dentro da ISO em
`/atelie/runtime/api/import/legacy/` e, apos a instalacao, em
`/opt/atelie/runtime/api/import/legacy/`.

- Usuarios.csv
- Clientes.csv
- MovCab.csv
- MovItem.csv
- MovItemSer.csv
- Notas.csv
- Duplicat.csv
- CliCredito.csv

## Observacao sobre a imagem de boot

A foto incorreta foi removida. A tela de boot do app esta preparada para usar
`wwwroot/assets/logo-boot.png` em tela cheia, mas a arte anexada na conversa ainda
nao existe como arquivo local no workspace; por isso ela nao entrou nesta ISO.

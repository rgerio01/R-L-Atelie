# Relatorio Inicial de Banco de Dados

## Banco predominante

O legado usa arquivos Paradox/BDE:

- 965 arquivos `.DB`
- 944 arquivos `.PX`
- 733 arquivos `.XG0`
- 732 arquivos `.YG0`
- 249 arquivos `.XG1`
- 249 arquivos `.YG1`
- 8 arquivos `.MB`
- 5 arquivos `.DBF`

## Hipotese operacional

Os arquivos `.DB` representam tabelas. Os arquivos `.PX`, `.XG*` e `.YG*` representam indices primarios/secundarios. Arquivos `.MB` podem conter campos memo/blob.

## Proximas acoes

1. Criar copia de trabalho somente leitura dos bancos para `MOD\data\original-readonly`.
2. Usar ferramenta compativel com Paradox/BDE para extrair schemas.
3. Gerar dicionario de dados com campos, tipos, indices e relacoes inferidas.
4. Validar integridade em copia, nunca no original.
5. Definir modelo alvo para PostgreSQL/Supabase ou outro banco moderno.

## Rollback

Nenhuma alteracao foi feita nos arquivos originais. A restauracao operacional continua baseada em `D:\AtelieProd\Equipexe` e no backup ZIP criado.

# Matriz UI -> Banco

Data: 2026-05-23

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\matriz-ui-banco.csv`

## Objetivo

Cruzar telas/modulos com tabelas, SQLs e entidades inferidas a partir da UI ja mapeada.

## Resultado

- Entradas UI -> banco: 757

## Campos da matriz

- Tela
- Modulo
- Entidade principal
- Entidades secundarias
- Tabelas lidas
- Tabelas alteradas
- Botoes
- Acoes
- Permissoes
- Relatorios
- Fluxo anterior
- Fluxo posterior
- Evidencia
- Pendencias

## Status de evidencia

Nesta rodada, a matriz e majoritariamente:

- Confirmado por UI-banco estatico: quando a tabela/SQL aparece em extracao de layout/string.
- Hipotese por string SQL: quando o texto sugere consulta, mas nao ha tela dinamica capturada.
- Nao confirmado por runtime: nenhuma entrada foi promovida a runtime sem ProcMon/diff.

## Modulos com maior volume

- LavFacilLan
- Estoque
- LavSoft
- SAT
- Financeiro
- NFE
- Senhas

## Como validar dinamicamente

Para promover uma entrada para `confirmado por runtime`:

1. Abrir a tela no runtime MOD.
2. Rodar ProcMon filtrado por processo.
3. Executar a acao/tela.
4. Capturar arquivos `.DB`, `.PX`, `.INI`, `.XML` acessados.
5. Se houver gravacao, capturar diff antes/depois.
6. Atualizar a matriz com tabela lida/alterada e campo afetado.

## Conclusao

A matriz permite priorizar quais telas devem ser abertas primeiro na validacao dinamica. Ela nao deve ser usada sozinha para afirmar escrita em banco sem diff ou log.

# Matriz Tela -> Acao -> Tabela

Data: 2026-05-23

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\matriz-tela-acao-tabela.csv`

## Objetivo

Mapear menus, telas e acoes operacionais para entidades/tabelas candidatas, separando evidencia de hipotese.

## Resultado

- Acoes/telas/menus consolidados: 1.936

## Campos da matriz

- Tela ou menu
- Modulo
- Acao inferida
- Entidade inferida
- Tabela consulta
- Tabela insere
- Tabela altera
- Tabela exclui
- Campo muda
- Log gerado
- Regra de negocio
- Permissao exigida
- Evidencia
- Status

## Tipos de acao inferidos

- consulta/abre tela;
- inclusao;
- alteracao;
- cancelamento/exclusao;
- financeiro/fechamento;
- relatorio/impressao.

## Status de evidencia

- Confirmado por UI: quando veio de menu/string/permissao ja mapeada.
- Hipotese por nome/string: quando a acao foi inferida pelo texto do menu.
- Nao confirmado por runtime: quando falta diff ou ProcMon.

## Regra importante

Mesmo quando a UI indica uma acao como "Alterar" ou "Cancelar", a tabela realmente afetada ainda deve ser validada dinamicamente. O legado pode usar:

- cancelamento logico;
- tabela paralela `*Can`;
- campo `Cancelado`;
- log em outra tabela;
- alteracao indireta via rotina compartilhada.

## Como validar dinamicamente

1. Escolher acao.
2. Capturar snapshot dos arquivos candidatos.
3. Executar a acao com dado teste no MOD.
4. Capturar diff.
5. Registrar campos alterados.
6. Registrar logs gerados.
7. Atualizar status para `confirmado por runtime`.

## Conclusao

A matriz fornece o roteiro de validacao tela-a-tela e botao-a-botao. Ela e a ponte entre UX reverse engineering e modelo de banco.

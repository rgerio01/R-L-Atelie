# Analise de Usuarios e Permissoes

Data: 2026-05-23

## Artefatos

- `matriz-usuarios-legado.csv`
- `matriz-permissoes-legado-classificada.csv`
- `matriz-usuario-perfil-tela-botao-acao-tabela.csv`
- `matriz-perfis-novo-sistema.csv`

## Usuarios legados identificados

Total: 9

Perfis/grupos observados:

- OPERA: 8
- MASTE: 1

## Observacoes

- `GABRIELA` foi classificada como Administrador Geral por grupo legado `MASTE` e regra administrativa ja definida no projeto.
- Usuarios em grupo `OPERA` foram classificados inicialmente como Operador.
- `TipUsuario` possui valores como `S` e `U`, mas o significado exato ainda nao foi confirmado por UI/runtime.
- Permissoes `NivelI`, `NivelA`, `NivelE`, `NivelT` existem no schema, mas sua semantica precisa de validacao.

## Regra de evidencia

- Usuario: confirmado por schema em `Usuarios.DB`.
- Permissao: confirmado por schema em `Nivel.DB`.
- Tela/acao: confirmado por UI quando vindo de menu/string/permissao; runtime pendente.

## Matriz conceitual Usuario -> Perfil -> Tela -> Botao -> Acao -> Tabela -> Permissao

Arquivo principal: `matriz-usuario-perfil-tela-botao-acao-tabela.csv`.

Base consolidada:

- usuarios legados: 9.
- permissoes legadas classificadas: 438.
- acoes/telas cruzadas: 1.936.
- permissoes novas propostas: 52.

Classificacao inicial de criticidade:

- baixa: visualizacao, consulta, listagem e acesso sem alteracao financeira/fiscal.
- media: cadastro operacional, edicao simples, impressao e manutencao de dados nao criticos.
- alta: alteracao de valores, estoque, fechamento, baixa, exclusao, exportacao e alteracao de status operacional.
- critica: cancelamento, estorno, desconto, permissao, usuario, caixa, fiscal, licenciamento, credenciais e configuracao.
- administrativa: usuarios, perfis, permissoes, parametros e logs.
- financeira: baixa, pagamento, caixa, desconto, acrescimo, recebimento, contas e repasses.
- fiscal: SAT, NFE, cancelamento fiscal, inutilizacao, emissao e contingencia.
- operacional: atendimento, ROL/OS, produtos, estoque, cliente e relatorios.

## Usuarios legados com classificacao inicial

| Usuario | Grupo legado | Perfil NextGen inicial | Status | Evidencia | Pendencia |
|---|---:|---|---|---|---|
| GABRIELA | MASTE | Administrador Geral | Ativo a validar | Schema `Usuarios.DB` + regra de projeto | Confirmar ultimo acesso e permissoes por runtime |
| BRENA | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |
| BRUNA | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |
| CID | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |
| EDU | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |
| FAT | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |
| LUCI | OPERA | Operador/Loja | Ativo a validar | Schema `Usuarios.DB` + regra futura de recebimento vendas | Confirmar papel operacional real |
| MICHELE | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |
| ROSIMEIRE | OPERA | Operador | Ativo a validar | Schema `Usuarios.DB` | Confirmar telas utilizadas |

Observacao: `Ativo/Inativo` e `ultimo acesso` dependem de campo/tabela de auditoria ou captura runtime ainda nao confirmada.

## Perfis do novo sistema

| Perfil | Escopo | Permissoes principais |
|---|---|---|
| Administrador Geral | Controle total da instalacao, licencas, dispositivos, credenciais e auditoria | `admin.*`, `usuarios.*`, `permissoes.*`, `licencas.*`, `configuracoes.*`, `auditoria.visualizar` |
| Administrador da Loja | Administracao operacional sem acesso a segredos tecnicos | `usuarios.gerenciar_loja`, `caixa.*`, `vendas.*`, `clientes.*`, `relatorios.*` |
| Operador | Uso operacional diario | `clientes.visualizar`, `clientes.criar`, `os.criar`, `vendas.criar`, `relatorios.operacionais` |
| Caixa | Recebimento e fechamento de caixa | `caixa.abrir`, `caixa.fechar`, `pagamentos.pix`, `pagamentos.cartao`, `pagamentos.dinheiro` |
| Financeiro | Contas, baixas, repasses, taxas e relatorios financeiros | `financeiro.visualizar`, `financeiro.editar`, `financeiro.baixar_pagamento`, `taxas.visualizar`, `repasses.gerenciar` |
| Estoque | Produtos, entradas, saidas, ajustes e inventario | `produtos.visualizar`, `produtos.editar`, `estoque.ajustar`, `estoque.inventario` |
| Atendimento | Clientes, ROL/OS e acompanhamento | `clientes.*`, `os.criar`, `os.editar_basico`, `os.imprimir` |
| Tecnico | Execucao e status tecnico da OS/ROL | `os.visualizar`, `os.alterar_status_tecnico`, `os.registrar_observacao` |
| Auditor | Leitura de auditoria e relatorios sem alterar dados | `auditoria.visualizar`, `logs.visualizar`, `relatorios.visualizar` |
| Somente leitura | Consulta controlada | `*.visualizar` |

## Permissoes criticas obrigatorias no NextGen

- `usuarios.criar`, `usuarios.editar`, `usuarios.bloquear`, `usuarios.resetar_senha`.
- `permissoes.atribuir`, `permissoes.remover`, `perfis.editar`.
- `vendas.cancelar`, `vendas.aplicar_desconto`, `vendas.estornar`.
- `pagamentos.pix`, `pagamentos.cartao`, `pagamentos.dinheiro`, `pagamentos.cancelar`, `pagamentos.reconciliar`.
- `caixa.abrir`, `caixa.fechar`, `caixa.suprimento`, `caixa.sangria`.
- `financeiro.baixar_pagamento`, `financeiro.estornar`, `financeiro.editar_vencimento`.
- `estoque.ajustar`, `estoque.baixar_manual`, `estoque.inventario`.
- `fiscal.emitir`, `fiscal.cancelar`, `fiscal.contingencia`.
- `licencas.gerenciar`, `licencas.renovar`, `licencas.bloquear_dispositivo`.
- `taxas.configurar`, `taxas.visualizar`, `taxas.repassar`.
- `relatorios.exportar`, `relatorios.financeiro`, `relatorios.auditoria`.

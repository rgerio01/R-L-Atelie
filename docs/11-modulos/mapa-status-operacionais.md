# Mapa de Status Operacionais

Data: 2026-05-23

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\mapa-status-operacionais.csv`

## Resultado

Campos classificados como status: 97.

Foi executado tambem perfilamento readonly das tabelas prioritarias. Esse perfilamento encontrou 20 valores distintos de status em dados reais copiados, sem alterar a base.

Distribuicao por dominio:

- Configuracao/Admin: 29
- Movimentos/OS/ROL: 22
- Nao classificado: 14
- Notas/Fiscal: 14
- Clientes: 10
- Financeiro: 3
- Usuarios/Permissoes: 3
- Produtos/Servicos: 2

## Padroes de status encontrados

Campos e termos recorrentes:

- `Cancelado`
- `Status`
- `Situacao`
- `Sit*`
- `Posicao`
- `Baixa`
- `Bloq*`
- `Entreg*`
- `Fech*`

## Valores observados em dados readonly

Significado ainda nao confirmado por UI/runtime:

- `Clientes.TipoEntrega`: `P` em 5.064 registros.
- `MovCab.Posicao`: `S` em 20.138 registros; `E` em 87 registros.
- `MovCab.Cancelado`: `S` em 198 registros.
- `MovCab.SitRol`: `P` em 20.048 registros.
- `MovCab.FixEntrega`: `True` em 27.146 registros; `False` em 4.826 registros.
- `MovCab.PosicaoAnt`: `E` em 6.260 registros; `S` em 393 registros.
- `CliCredito.Sit`: `B` em 95 registros; `C` em 2 registros.
- `Notas.Cancelada`: `S` em 486 registros.
- `Duplicat.Baixa`: `S` em 19.251 registros; `N` em 466 registros.
- `Duplicat.Cancelado`: `S` em 463 registros.
- `Produt.BloqAltPre`: `N` em 121 registros.
- `MovSatCliOcor.SitOcor`: `0` em 4.303 registros.
- `Titulos.Baixa`: `S` em 99 registros; `N` em 4 registros.

## Status criticos por dominio

### Clientes

Campos candidatos:

- cancelamento/status em `Clientes`, `GruClientes`, `FunCli`, `FunCliRou` e tabelas relacionadas.

Evidencia:

- confirmado por schema quando campo existe.
- significado dos valores ainda nao confirmado.

Pendencia:

- levantar valores distintos por tabela em copia readonly;
- abrir tela de cadastro/consulta de cliente e comparar com labels.

### OS/ROL

Campos candidatos:

- `MovCab.Posicao`
- cancelamentos em tabelas auxiliares;
- status/localizacao em `MovLocRol` e `CadLocRol`;
- campos de situacao em `MovControle`.

Evidencia:

- confirmado por schema para campos existentes.
- ciclo de vida ainda e hipotese ate validação dinâmica.

Pendencia:

- criar/acompanhar ROL teste no MOD;
- capturar diff antes/depois;
- identificar valores possiveis de `Posicao`.

### Financeiro

Campos candidatos:

- `Duplicat.Baixa`
- cancelamentos de boletos/titulos;
- status de boleto/transacao.

Pendencia:

- validar estados aberto/baixado/cancelado/estornado.

### Estoque

Status direto menos evidente.

Pendencia:

- validar `TipoES`, `Cancelado` e encerramento como parte do estado do estoque.

### Fiscal/SAT

Campos candidatos:

- `Cancelada`
- `Cancelado`
- status fiscal em SAT/NFE.

Pendencia:

- separar status fiscal real de status operacional interno.

## Regra de seguranca

Nenhum significado de codigo deve ser assumido sem evidência. Exemplo: `Posicao=1` nao deve ser chamado de "aberto" sem amostra/tela/runtime.

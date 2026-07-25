# Perfil de Dados Prioritarios Readonly

Data: 2026-05-23

## Escopo

Leitura somente leitura das tabelas prioritarias do Paradox, executada via ODBC 32-bit sobre:

`D:\AtelieProd\MOD\data\original-readonly\Equipexe`

Nenhum dado foi gravado ou alterado.

## Artefatos gerados

- `perfil-tabelas-prioritarias-linhas.csv`
- `perfil-status-valores-distintos.csv`
- `perfil-valores-monetarios-estatisticas.csv`
- `perfil-datas-faixas.csv`
- `perfil-campos-presenca.csv`
- `perfil-tabelas-prioritarias-falhas.csv`

Script:

- `D:\AtelieProd\MOD\apps\tools\profile-priority-paradox-data-32bit.ps1`

## Resultado

- Tabelas perfiladas com sucesso: 44
- Campos perfilados: 893
- Valores distintos de status capturados: 20
- Estatisticas de valor/quantidade: 33
- Faixas de datas: 21
- Falhas: 0

## Maiores tabelas prioritarias

| Tabela | Linhas | Colunas | Dominio |
|---|---:|---:|---|
| `MovCab` | 31.972 | 58 | ROL/OS/movimento |
| `Notas` | 21.679 | 37 | fiscal/nota |
| `Duplicat` | 19.717 | 60 | financeiro/duplicatas |
| `Clientes` | 5.064 | 124 | clientes |
| `MovSatCliOcor` | 4.303 | 21 | SAT/ocorrencias |
| `Nivel` | 438 | 13 | permissoes |
| `Produt` | 121 | 48 | produtos/servicos |
| `CliCredito` | 106 | 14 | credito cliente/financeiro |
| `Titulos` | 103 | 36 | contas a pagar |
| `Usuarios` | 9 | 14 | usuarios |

## Status observados

Importante: os valores foram observados diretamente nos dados readonly, mas o significado semantico ainda precisa de UI/runtime.

Exemplos:

- `Clientes.TipoEntrega`: valor `P` em 5.064 registros.
- `MovCab.Posicao`: valor `S` em 20.138 registros; valor `E` em 87 registros.
- `MovCab.Cancelado`: valor `S` em 198 registros.
- `MovCab.SitRol`: valor `P` em 20.048 registros.
- `MovCab.FixEntrega`: `True` em 27.146 registros; `False` em 4.826 registros.
- `CliCredito.Sit`: `B` em 95 registros; `C` em 2 registros.
- `Notas.Cancelada`: valor `S` em 486 registros.
- `Duplicat.Baixa`: `S` em 19.251 registros; `N` em 466 registros.
- `Duplicat.Cancelado`: valor `S` em 463 registros.
- `Produt.BloqAltPre`: valor `N` em 121 registros.
- `MovSatCliOcor.SitOcor`: valor `0` em 4.303 registros.
- `Titulos.Baixa`: `S` em 99 registros; `N` em 4 registros.

## Valores e quantidades observados

Exemplos relevantes:

- `MovCab.ValTot`: 31.947 ocorrencias, minimo 0, maximo 3000.
- `MovCab.TotPecas`: 31.971 ocorrencias, minimo 0, maximo 48.
- `MovCab.DescontoROL`: 31.971 ocorrencias, minimo 0, maximo 50.
- `MovCab.DescontoValor`: 31.971 ocorrencias, minimo 0, maximo 130.
- `Notas.ValNot`: 21.679 ocorrencias, minimo -15, maximo 1500.
- `Duplicat.ValFat`: 19.717 ocorrencias, minimo -15, maximo 1500.
- `Duplicat.ValDup`: 19.717 ocorrencias, minimo -15, maximo 1500.
- `Duplicat.ValDupPag`: 19.251 ocorrencias, minimo -15, maximo 1170.
- `MovEst.Qde`: 1 ocorrencia na base atual.
- `Titulos.ValTit`: 103 ocorrencias, minimo 3, maximo 2279,73.

## Leitura de dominio

Achados reforcados por ocorrencia direta:

- `MovCab` e a tabela operacional mais volumosa da base prioritária, reforcando o papel como cabecalho de ROL/movimento.
- `Duplicat` tem volume alto e campos de baixa/pagamento, reforcando papel financeiro/recebiveis.
- `Notas` tem volume proximo a `Duplicat`, reforcando vinculo fiscal/financeiro.
- `Clientes` possui 5.064 registros, confirmando entidade central.
- `MovSatCliOcor` tem 4.303 registros, indicando historico/ocorrencias SAT relevante mesmo quando outras tabelas SAT estao vazias.
- `MovEst` tem apenas 1 registro na base atual; estoque existe no schema, mas esta base operacional parece ter baixo uso de movimento de estoque.

## Classificacao de evidencia

- Contagem de linhas: confirmado por ocorrencia direta readonly.
- Existencia de campo: confirmado por schema.
- Significado de codigo/status: nao confirmado ate UI/runtime.
- Formula/calculo: nao confirmado ate runtime/diff.
- Relacionamento por codigo comum: confirmado por schema quando campos existem em ambas as tabelas, mas cardinalidade exata requer dados/runtime.

## Pendencias

- Extrair valores distintos de status com significado visual em tela.
- Validar `MovCab.Posicao`, `SitRol`, `FixEntrega`, `Cancelado`.
- Validar `Duplicat.Baixa` e `Cancelado` em tela financeira.
- Validar se `Notas.ValNot`, `Duplicat.ValFat` e `MovCab.ValTot` fecham por ROL/nota.
- Validar motivo de valores negativos em `Notas` e `Duplicat`.
- Confirmar se estoque e pouco usado ou se a movimentacao principal esta em outro conjunto de tabelas.

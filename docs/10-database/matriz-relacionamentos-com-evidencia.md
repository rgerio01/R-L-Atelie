# Matriz de Relacionamentos com Evidencia

Data: 2026-05-23

## Clientes -> MovCab

Origem: Clientes

Destino: MovCab

Campos: CodCli -> CodCli

Tipo: 1:N

Evidencia: campo CodCli existe em Clientes e MovCab; telas/menus indicam fluxo ROL/cliente

Confianca: alta

Status: confirmado por schema + UI

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Clientes -> Duplicat

Origem: Clientes

Destino: Duplicat

Campos: CodCli -> CodCli

Tipo: 1:N

Evidencia: campo CodCli existe em Clientes e Duplicat; Duplicat contem vencimento/baixa/valor

Confianca: alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Clientes -> Notas

Origem: Clientes

Destino: Notas

Campos: CodCli -> CodCli

Tipo: 1:N

Evidencia: campo CodCli existe em Clientes e Notas

Confianca: alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## MovCab -> Notas

Origem: MovCab

Destino: Notas

Campos: NumNot -> NumNot

Tipo: N:1 ou 1:1

Evidencia: campo NumNot existe em MovCab e Notas; papel exato pendente

Confianca: media

Status: hipotese por nome/string

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## MovCab -> CliCredito

Origem: MovCab

Destino: CliCredito

Campos: ROL -> Rol

Tipo: 1:N

Evidencia: campo Rol em CliCredito e ROL em MovCab

Confianca: media-alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## ProdEst -> MovEst

Origem: ProdEst

Destino: MovEst

Campos: CodProEst -> CodProEst

Tipo: 1:N

Evidencia: campo CodProEst existe em ProdEst e MovEst

Confianca: alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Produt -> MovCab/Itens

Origem: Produt

Destino: MovCab/Itens

Campos: CodPro -> CodPro

Tipo: 1:N

Evidencia: CodPro aparece em tabelas de controle/itens, mas item exato do ROL precisa validar

Confianca: media

Status: hipotese por nome/string

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Usuarios -> MovCab

Origem: Usuarios

Destino: MovCab

Campos: CodUsuario -> CodUsuario

Tipo: 1:N

Evidencia: CodUsuario aparece em Usuarios e MovCab

Confianca: media-alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Usuarios -> MovEst

Origem: Usuarios

Destino: MovEst

Campos: CodUsuario -> CodUsuario

Tipo: 1:N

Evidencia: CodUsuario aparece em Usuarios e MovEst

Confianca: media-alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Duplicat -> Boletos/DupBoleto

Origem: Duplicat

Destino: Boletos/DupBoleto

Campos: NumDup/NumFat -> NumDup/NumFat

Tipo: 1:N

Evidencia: campos NumFat/NumDup aparecem em duplicata e boleto

Confianca: media

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## Notas -> NotaFisPag

Origem: Notas

Destino: NotaFisPag

Campos: NumNot/NumNotFis -> NumNotFis

Tipo: 1:N

Evidencia: campos de nota aparecem em Notas e NotaFisPag, exato join pendente

Confianca: media

Status: hipotese por nome/string

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois

## SAT -> NotaSatCanc

Origem: SAT

Destino: NotaSatCanc

Campos: NumNotSat -> NumNotSat

Tipo: 1:0..1

Evidencia: campo NumNotSat em NotaSat e NotaSatCanc

Confianca: alta

Status: confirmado por schema

Como validar dinamicamente: abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois


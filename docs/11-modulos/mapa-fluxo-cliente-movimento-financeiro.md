# Mapa de Fluxo Cliente -> Movimento/ROL -> Financeiro

Data: 2026-05-23

## Fluxo confirmado/hipotetico por evidência

1. Cliente nasce em `Clientes` (`CodCli`, `NomCli`, documento, endereco, contato).
2. ROL/movimento nasce em `MovCab`, com `ROL`, `CodCli`, datas, status/posicao, totais e usuario.
3. Pagamento/credito/nota se conectam por `CodCli`, `ROL`, `NumNot`, `NumFat` conforme tabela.
4. `Duplicat` representa duplicatas/recebiveis por cliente, vencimento, baixa e valor pago.
5. `Notas` e `NotaFisPag` representam fiscal/pagamento de nota.

## Status de evidência

- `Clientes.CodCli -> MovCab.CodCli`: confirmado por schema + UI.
- `Clientes.CodCli -> Duplicat.CodCli`: confirmado por schema.
- `MovCab.NumNot -> Notas.NumNot`: hipótese por nome/string, validar dinamicamente.
- `MovCab.ROL -> CliCredito.Rol`: confirmado por schema.

## Validação dinâmica

Criar cliente teste no MOD, capturar diff em `Clientes`; criar ROL teste, capturar diff em `MovCab`; registrar pagamento teste, capturar diff em `Duplicat`, `CliCredito`, `Notas` e `NotaFisPag`. Executar somente com snapshot e rollback.

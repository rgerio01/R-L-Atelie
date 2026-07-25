# Mapa de Fluxo Produto -> Estoque -> Financeiro

Data: 2026-05-23

## Fluxo confirmado/hipotetico por evidência

1. Produto/servico operacional aparece em `Produt`.
2. Produto de estoque aparece em `ProdEst`.
3. Movimento de estoque aparece em `MovEst`, com `CodProEst`, `Qde`, `TipoES`, `ValUnit`, `ValTot`, `CodUsuario`.
4. Cancelamento/encerramento aparece em `MovEstCan` e `MovEstEnc`.
5. Relação estoque -> financeiro ainda não está confirmada diretamente; deve ser validada por fluxo de venda/OS/baixa.

## Status de evidência

- `ProdEst.CodProEst -> MovEst.CodProEst`: confirmado por schema.
- `Produt.CodPro -> itens do ROL`: hipótese por nome/string, validar por tela e diff.
- `MovEst.ValTot/ValUnit -> financeiro`: hipótese por nome/string, não confirmado.

## Validação dinâmica

Criar/alterar produto teste no MOD, capturar diff em `Produt`/`ProdEst`; lançar entrada/baixa controlada, capturar diff em `MovEst`; simular venda/ROL com produto, verificar se há baixa automatica e impacto financeiro.

# Procedimento de Validacao Dinamica de Dominio

Data: 2026-05-23

## Objetivo

Confirmar, em ambiente MOD, os relacionamentos e regras hoje classificados como schema/UI/hipotese.

## Regra de seguranca

- Nao executar no original.
- Usar apenas runtime MOD.
- Criar backup/snapshot antes.
- Usar dados de teste claramente identificados.
- Registrar diff antes/depois.
- Documentar rollback.

## Testes controlados

### 1. Cliente

1. Capturar hash/listagem de `Clientes.DB`, `CliContato.DB`, `ClientesObs.DB`.
2. Abrir tela de cliente.
3. Criar cliente teste `MOD_TESTE_CLIENTE`.
4. Capturar arquivos alterados via ProcMon.
5. Extrair diff lógico.
6. Reverter ou marcar como teste.

### 2. ROL/MovCab

1. Capturar `MovCab.DB` e tabelas auxiliares.
2. Criar ROL teste para cliente MOD.
3. Registrar `ROL`, `CodCli`, datas, `ValTot`, `TotPecas`, `Posicao`, `CodUsuario`.
4. Confirmar tabelas de itens/pecas.

### 3. Produto/Estoque

1. Criar produto teste.
2. Criar movimento de entrada.
3. Criar movimento de baixa.
4. Confirmar `Produt`, `ProdEst`, `MovEst`, `MovEstCan` se houver cancelamento.

### 4. Financeiro

1. Registrar pagamento controlado.
2. Observar `Duplicat`, `CliCredito`, `Notas`, `NotaFisPag`, `FecCaixa`.
3. Confirmar valor bruto, desconto, valor pago, baixa e caixa.

### 5. Relatorio

1. Gerar relatorio por cliente/ROL/financeiro.
2. Capturar tabelas lidas via ProcMon.
3. Confirmar filtros e totais.

## Saida esperada

Para cada teste:

- evidência antes;
- ação executada;
- evidência depois;
- diff;
- status promovido para confirmado por runtime ou mantido como hipótese;
- rollback.

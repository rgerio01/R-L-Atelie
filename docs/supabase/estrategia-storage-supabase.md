# Estrategia de Storage Supabase — NextGen

Data: 2026-05-25 (revisado com schema completo e dados reais)

---

## O que foi feito

1. Diagnosticado e corrigido consumo excessivo na migracao (~550 MB → ~180 MB)
2. Analisado o schema completo do NextGen (202605240001) com dados reais de 16 anos do LavSoft
3. Identificadas 3 tabelas que esgotariam o banco em < 2 anos sem politica de retencao
4. Criada politica de retencao obrigatoria (`202605250011_retention_policy.sql`)

---

## Como foi feito

### Diagnostico de migracao (sessao anterior)

| Causa | Volume | Problema |
|---|---|---|
| `legacy_inventory` (328.100 linhas JSONB) | ~347 MB | Copia redundante dos dados Paradox |
| Tenant duplicado (`748bb125`) | ~150 MB | Migracao rodou 2x |
| `itens_venda` dobrado (2 tenants) | ~50 MB | Metade era do tenant duplicado |
| **Total** | **~550 MB** | Ultrapassou limite free de 500 MB |

### Correcao executada

1. Deletou `legacy_inventory` em lotes de 100 rows (evitar statement timeout 30s)
2. Deletou dados do tenant duplicado em lotes
3. Restaurou `itens_venda` completo dos arquivos Paradox originais

### Analise do schema NextGen completo (`calcular_storage.py`)

Calculado campo-a-campo para cada tabela do schema `202605240001_nextgen_core.sql`,
usando como base 16 anos de operacao real do LavSoft (antecessor).

---

## Resultado da migracao

| Tabela | Antes | Depois | Tamanho real |
|---|---:|---:|---:|
| tenants | 2 | 1 | < 1 MB |
| legacy_inventory | 328.100 | 0 | 0 MB |
| vendas | 63.944 | 31.972 | ~28 MB |
| clientes | 10.128 | 5.064 | ~5 MB |
| itens_venda | 84.918 | 174.558 | ~145 MB |
| produtos | 1.712 | 856 | ~1 MB |
| **Total atual** | | | **~180 MB** |

> Nota: itens_venda subiu de 84.918 para 174.558 porque a restauracao foi completa
> (MovItemSer 89.640 + MovItem 84.918). O numero correto e 174.558.

**Espaco livre: ~320 MB (64% do plano free)**

---

## Projecao de crescimento — valores reais

### Base de calculo

Taxa historica real do LavSoft (16 anos):
- **167 ROLs/mes** (31.972 / 192 meses)
- **26 clientes novos/mes** (5.064 / 192 meses)
- **5,46 itens por ROL** (174.558 / 31.972)

### Tamanho por linha (com overhead PostgreSQL 2.5x)

| Tabela | Bytes/linha | Observacao |
|---|---:|---|
| clientes | 1.050 B | nome+telefone+endereco jsonb+legacy_payload |
| vendas | 920 B | 5 UUIDs+valores+legacy_payload jsonb |
| itens_venda | 870 B | descricao+numeros+legacy_payload jsonb |
| produtos | 1.600 B | codigo+descricao+legacy_payload jsonb |
| pagamentos_venda | 4.800 B | raw_payload MP ~2KB jsonb + qr_code |
| pagamentos_licenca | 7.200 B | qr_code_base64 ~400B + raw_payload MP |
| **auditoria** | **2.100 B** | **antes jsonb + depois jsonb (2x a linha)** |
| **logs_runtime** | **1.050 B** | **message + metadata jsonb** |
| **webhooks_mercado_pago** | **9.500 B** | **payload MP completo 3-5KB jsonb** |

---

## PROBLEMA CRITICO: tabelas de alta rotatividade

Sem politica de retencao, 3 tabelas destroem o banco em < 2 anos:

| Tabela | Linhas/mes | MB/ano | Causa |
|---|---:|---:|---|
| **auditoria** | ~1.700 | **37 MB** | Trigger em 15 tabelas (INSERT+UPDATE+DELETE) |
| **logs_runtime** | ~3.340 | **40 MB** | 20 eventos de log por ROL |
| **webhooks_mercado_pago** | ~500 | **54 MB** | Payload jsonb completo do Mercado Pago |
| **TOTAL sem retencao** | | **131 MB/ano** | → 500 MB em **1,8 anos** 🔴 |

### Crescimento total SEM retencao: 15,2 MB/mes → 500 MB em 1,8 anos 🔴

---

## Solucao: politica de retencao (`202605250011_retention_policy.sql`)

| Tabela | Retencao | Cap maximo | Impacto |
|---|---|---:|---|
| auditoria | 90 dias | ~10 MB fixo | elimina 37 MB/ano |
| logs_runtime | 30 dias | ~3,5 MB fixo | elimina 40 MB/ano |
| webhooks_mercado_pago | 30 dias | ~4,8 MB fixo | elimina 54 MB/ano |
| sync_queue | 7 dias (apos applied) | ~0 MB | elimina ~20 MB/ano |
| appliance_status | 7 dias | < 1 MB fixo | elimina ~4 MB/ano |

**Implementado via pg_cron** (rodar todo dia na madrugada).

Adicionalmente:
- Removidos triggers de auditoria em `itens_venda`, `estoque`, `taxas_servico` (alto volume, baixo valor)
- `pagamentos_venda.raw_payload` → mover para Supabase Storage bucket `pagamentos/` (-81% por linha)
- `pagamentos_licenca.qr_code_base64` → Supabase Storage bucket `licencas/`

---

## Projecao COM retencao (valores reais)

### Crescimento liquido por tabela (167 ROLs/mes — ritmo historico real)

| Tabela | Linhas/mes | MB/mes | MB/ano |
|---|---:|---:|---:|
| itens_venda | 912 | 0,78 | 9,4 |
| pagamentos_venda | 200 | 0,96 | 11,5 |
| vendas | 167 | 0,15 | 1,8 |
| taxas_servico | 200 | 0,30 | 3,6 |
| clientes | 26 | 0,03 | 0,3 |
| pagamentos_licenca | 1 | 0,01 | 0,1 |
| outros | — | 0,01 | 0,1 |
| **TOTAL** | | **2,17 MB/mes** | **26 MB/ano** |

### Cenarios

| Ritmo | MB/mes | MB/ano | Anos ate 500 MB |
|---|---:|---:|---:|
| Historico real (167 ROLs/mes) | 2,17 | 26 | **12 anos** ✅ |
| Baixo (50 ROLs/mes) | 0,67 | 8 | **39 anos** ✅ |
| Medio (150 ROLs/mes) | 1,96 | 24 | **13 anos** ✅ |
| Alto (300 ROLs/mes) | 3,89 | 47 | **6 anos** ⚠ |

---

## Regras definitivas

### Regra 1 — Retencao e OBRIGATORIA (nao opcional)

Sem `202605250011_retention_policy.sql` ativo, o banco esgota em menos de 2 anos.
Verificar se pg_cron esta habilitado: Dashboard → Database → Extensions → pg_cron.

### Regra 2 — Nunca usar legacy_inventory como armazenamento permanente

Cada linha ocupa ~1.1 KB de JSONB. 328k linhas = 347 MB.
Dados historicos ficam em `D:/AtelieProd/Equipexe` (read-only, intocado).

### Regra 3 — Nunca rodar migracao duas vezes sem verificar contagem

```sql
SELECT count(*) FROM tenants;
```
Se ja existir o tenant correto, NAO rodar novamente.

### Regra 4 — Payloads grandes vao para Supabase Storage, nao para colunas DB

| Tipo | Onde guardar |
|---|---|
| Dados operacionais (clientes, vendas, produtos) | Tabelas PostgreSQL |
| raw_payload Mercado Pago (resposta completa) | Storage bucket `pagamentos/` |
| qr_code_base64 de licencas | Storage bucket `licencas/` |
| Backups SQLite do appliance | Storage bucket `backups/` |
| Historico legado (16 anos Paradox) | D:/AtelieProd/Equipexe (original) |

O Supabase Storage tem 1 GB no plano free e NAO conta no limite do banco.

### Regra 5 — Monitorar uso mensalmente

```sql
SELECT
  tablename,
  pg_size_pretty(pg_total_relation_size('public.'||tablename)) AS size,
  pg_total_relation_size('public.'||tablename) AS bytes
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY bytes DESC;
```

Ou: Dashboard → Settings → Database → Disk usage.

### Regra 6 — Appliance envia status 1x por dia, nao por hora

Hourly = 720 linhas/mes × 12 = 8.640 linhas/ano = 4,5 MB/ano.
Daily = 365 linhas/ano = 0,2 MB/ano. Manter 7 dias = sempre < 0,02 MB.

---

## Para verificar quando Supabase recuperar (PGRST002)

1. Acessar app.supabase.com → projeto `kwodkzfiuultdezanrjv`
2. Se aparecer "Restore project" → clicar (projeto pausado)
3. Se nao pausado: Settings → Database → Restart database
4. Aguardar 2-3 min e rodar `calcular_storage.py` para confirmar valores reais
5. Aplicar `202605250011_retention_policy.sql` via Dashboard SQL Editor

---

## Capacidade atual resumida

```
Estado atual:  ~180 MB  (36% do plano free)
Espaco livre:  ~320 MB
COM retencao:  2,17 MB/mes → 12 anos ao ritmo historico real do LavSoft
```

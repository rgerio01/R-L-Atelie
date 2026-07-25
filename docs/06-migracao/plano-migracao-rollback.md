# Plano de Migracao e Rollback

## Fase 0 - Preservacao

Concluida nesta etapa:

- Backup completo do original.
- Estrutura MOD isolada.
- Inventario inicial.
- API de autenticacao em homologacao.

## Fase 1 - Dicionario de dados

1. Copiar bancos para `MOD\data\original-readonly`.
2. Extrair schemas Paradox.
3. Identificar tabelas mestres, movimentos, logs, usuarios, parametros e fiscais.
4. Criar dicionario de dados.

## Fase 2 - Mapeamento funcional

1. Executar o sistema legado em ambiente controlado.
2. Capturar telas, menus e relatorios.
3. Associar cada tela a tabelas e executaveis.
4. Definir equivalencia na nova aplicacao.

## Fase 3 - Migracao tecnica

1. Criar banco alvo.
2. Criar scripts idempotentes de migracao.
3. Validar contagens, somatorios e amostras.
4. Executar testes de usuario em homologacao.

## Fase 4 - Corte operacional

1. Congelar escrita no legado.
2. Fazer backup final.
3. Migrar delta.
4. Validar operacao.
5. Liberar nova versao.

## Rollback de corte

1. Interromper nova versao.
2. Preservar logs e banco MOD.
3. Restaurar uso do legado a partir do ultimo estado validado.
4. Registrar divergencias de dados ocorridas durante a janela.

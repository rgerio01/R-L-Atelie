# Plano de Migracao EquipeExe para NextGen

Data: 2026-05-23

## Principio

O EquipeExe original permanece intacto. A migracao deve usar copia readonly, validacao por amostragem e rollback documentado.

## Etapas

1. Congelar snapshot readonly do Paradox.
2. Importar tabelas prioritarias para staging.
3. Mapear entidades: clientes, MovCab/ROL, Duplicat, Produt, ProdEst, MovEst, Notas, NotaFisPag, SAT e ocorrencias.
4. Validar contagens e somatorios.
5. Normalizar para banco NextGen.
6. Validar telas e relatorios equivalentes.
7. Executar piloto operacional.
8. Rodar paralelo legado + NextGen.
9. Migrar modulo por modulo.

## Regras de validacao

- quantidade de clientes importados;
- quantidade de movimentos/ROL;
- somatorio financeiro de duplicatas;
- quantidade de produtos;
- saldos de estoque quando confirmados;
- notas/SAT por periodo;
- pagamentos por forma;
- divergencias documentadas.

## Rollback

- manter backup do banco NextGen antes de cada carga;
- carga deve ser repetivel e versionada;
- nenhum script de migracao altera Paradox original;
- em falha, descartar staging/NextGen e repetir a partir da copia readonly.

## Pontos criticos

- semantica de status ainda precisa validacao runtime/UI;
- relacionamento exato de OS/ROL pode ser distribuido;
- estoque parece pouco usado em `MovEst` nesta copia;
- permissoes `NivelI/A/E/T` exigem confirmacao semantica.

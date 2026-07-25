# Mapa Funcional de Telas, Menus e Relatorios

Data: 2026-05-23

Escopo: executaveis principais do legado analisados por strings legiveis, sem alterar o original.

## Arquivos gerados

- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-funcional-telas\mapa-funcional-executaveis.csv`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-funcional-telas\correlacao-menu-permissao-executavel.csv`

## Resultado por executavel

- Estoque: 1749 textos funcionais candidatos
- Financeiro: 972 textos funcionais candidatos
- Gerenciador: 197 textos funcionais candidatos
- LavFacilLan: 5576 textos funcionais candidatos
- LavSoft: 6781 textos funcionais candidatos
- NFE: 776 textos funcionais candidatos
- SAT: 2673 textos funcionais candidatos

## Resultado por categoria

- cadastro: 6077
- operacional: 5501
- fiscal: 1590
- outro: 1546
- relatorio/impressao: 1360
- financeiro/caixa: 1327
- comunicacao/update: 1036
- autenticacao/permissao: 287

## Leitura tecnica

- Este mapa indica o que cada executavel aparenta expor em menus, telas, mensagens, relatorios e acoes.
- A coluna `StatusLayout` informa que a posicao exata ainda nao foi extraida de forma confiavel.
- A posicao de campos, botoes e grades deve ser confirmada por captura dinamica no runtime MOD.
- A correlacao com permissao usa nome/texto e deve ser tratada como indicio, nao como prova final.

## Proxima validacao

1. Abrir o runtime MOD com atualizacao bloqueada.
2. Acessar cada menu principal com usuario administrador MOD.
3. Capturar janela, titulo, controles visiveis, posicoes, atalhos, botoes e relatorios gerados.
4. Cruzar captura visual com `Nivel.DB`, strings estaticas e tabelas Paradox.

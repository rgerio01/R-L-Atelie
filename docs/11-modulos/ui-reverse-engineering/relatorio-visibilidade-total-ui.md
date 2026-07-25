# Relatorio de Visibilidade Total da UI - EquipeExe

Data: 2026-05-23

## Escopo

Analise controlada da interface do EquipeExe a partir de:

- strings funcionais dos executaveis principais;
- permissoes e rotinas em `Nivel.DB`;
- blocos Delphi TPF0 extraidos dos binarios;
- vinculos estaticos entre UI e banco;
- inventario de assets visuais do legado;
- baselines de runtime MOD ja coletados.

O original em `D:\AtelieProd\Equipexe` nao foi alterado.

## Artefatos gerados

- `mapa-telas-funcional-consolidado.csv`
- `mapa-menus-submenus-acoes-consolidado.csv`
- `mapa-permissoes-ui-consolidado.csv`
- `mapa-componentes-ui.csv`
- `mapa-layouts-delphi-tpf0.csv`
- `mapa-ui-banco-interligacoes.csv`
- `mapa-assets-visuais.csv`
- `resumo-ui-por-modulo.csv`

Script:

- `D:\AtelieProd\MOD\apps\tools\build-ui-visibility-maps.py`

## Resultado quantitativo

- Telas/acoes/textos funcionais candidatos: 18.724
- Menus/submenus/acoes consolidados: 1.936
- Blocos Delphi TPF0 extraidos: 521
- Vinculos UI -> banco/SQL: 757
- Assets visuais inventariados: 1.598

Distribuicao por modulo:

| Modulo | Textos/telas/acoes | Menus/acoes | Layouts TPF0 | Vinculos banco |
|---|---:|---:|---:|---:|
| LavSoft | 6.781 | 601 | 329 | 144 |
| LavFacilLan | 5.576 | 237 | 174 | 275 |
| SAT | 2.673 | 342 | 0 | 122 |
| Estoque | 1.749 | 318 | 18 | 153 |
| Financeiro | 972 | 288 | 0 | 34 |
| NFE | 776 | 119 | 0 | 16 |
| Gerenciador | 197 | 31 | 0 | 0 |

## Leitura por dominio funcional

Textos/telas/acoes:

- lavanderia/ROL: 5.183
- operacional/outro: 3.456
- cadastro: 3.338
- financeiro/caixa: 1.625
- estoque/produto: 1.552
- fiscal: 1.510
- autenticacao/permissao: 1.158
- relatorio/impressao: 581
- sync/update/API: 321

Menus/acoes:

- cadastro: 469
- operacional/outro: 403
- fiscal: 264
- estoque/produto: 201
- autenticacao/permissao: 190
- lavanderia/ROL: 183
- financeiro/caixa: 142
- relatorio/impressao: 79
- sync/update/API: 5

## Modulos com maior densidade visual

### LavSoft

Papel: modulo principal operacional.

Evidencias:

- maior volume de textos funcionais;
- 329 blocos TPF0;
- 601 menus/acoes;
- forte presenca de ROL, entrega, caixa, cadastros, relatorios, permissao e bloqueios.

Leitura UX:

LavSoft deve ser tratado como a referencia primaria da experiencia operacional. A nova geracao precisa preservar velocidade de acesso a ROL, cliente, entrega, caixa e relatorios.

### LavFacilLan

Papel: modulo operacional com alta densidade de telas e vinculos de banco.

Evidencias:

- 5.576 textos/telas/acoes;
- 174 blocos TPF0;
- 275 vinculos banco/SQL;
- comunicacao real observada em runtime.

Leitura UX:

E candidato a conter fluxos operacionais ricos e telas com grids/consultas. Deve ser priorizado para captura dinamica visual.

### Estoque

Papel: modulo de produto/estoque.

Evidencias:

- 1.749 textos/telas/acoes;
- 318 menus/acoes;
- 18 blocos TPF0;
- 153 vinculos banco/SQL.

Leitura UX:

Abrange produtos, entradas, baixas, consulta, fiscal e relatorios de estoque.

### SAT/NFE

Papel: fiscal/atendimento/solicitacoes.

Evidencias:

- SAT concentra 342 menus/acoes e 122 vinculos banco;
- NFE tem dominio fiscal e possiveis componentes/licencas de terceiros.

Leitura UX:

Devem ser separados em nova arquitetura por responsabilidade fiscal, com logs e permissao granular.

### Financeiro

Papel: financeiro, caixa, titulos, relatorios e licenca/bloqueio.

Evidencias:

- 288 menus/acoes;
- sinais de `ArquivoLicenca`, `FE_Licenca`, `Vencimento`, `BloqSec*`.

Leitura UX:

Deve preservar rotinas de fechamento, pagamento, baixa, relatorios e bloqueios administrativos.

### Gerenciador

Papel: modulo administrativo/remoto.

Evidencias:

- Windows Forms/.NET;
- menus de download/upload/tarefas/logs/opcoes;
- endpoints remotos de autenticacao, registro, atualizacao e nuvem.

Leitura UX:

Na nova geracao, suas funcoes devem migrar para painel administrativo e servicos backend, nao para operacao manual acoplada.

## Componentes UI e framework

Evidencia tecnica:

- predominancia Delphi/Borland/VCL nos modulos principais;
- TPF0 indica formularios Delphi embutidos;
- componentes detectados em extracao parcial: `TTable`, `TMenuItem`, `TQuery`;
- `Gerenciador.exe` indica Windows Forms/.NET.

Limitacao:

A extracao estatica recupera muitos textos e blocos, mas nem sempre nome do form, caption, posicao, `Left/Top/Width/Height` e handlers ficam completos. Posicao real precisa de captura dinamica.

## Assets visuais

Inventario:

- `.bmp`: 1.491
- `.jpg`: 102
- `.gif`: 5

Leitura:

O legado usa muitos bitmaps, especialmente em `Figuras`. A nova UI pode substituir assets gradualmente, mas deve mapear antes quais imagens sao logotipo, marcas, icones, fundos, etiquetas ou recursos operacionais.

## Interligacao UI -> banco

Vinculos extraidos: 757.

Distribuicao:

- LavFacilLan: 275
- Estoque: 153
- LavSoft: 144
- SAT: 122
- Financeiro: 34
- NFE: 16
- Senhas: 13

Leitura:

Os vinculos sao indicios de tabelas/queries usadas por telas. Devem ser validados por ProcMon e tracing BDE durante navegacao real.

## Permissoes

Foram consolidadas operacoes de permissao a partir de `Nivel.DB` e correlacoes com strings dos executaveis.

Leitura:

- permissao parece organizada por `CodSistema`, `Op`, usuario e filial;
- operacoes usam nomes de handlers/menu, como `AlteraCliente1`, `CFAbertura1`, `Anotacoes1`;
- niveis `I/A/E/T` ainda precisam de semantica confirmada.

## Proxima fase recomendada

1. Executar captura dinamica por modulo no runtime MOD.
2. Para cada tela aberta, registrar:
   - titulo da janela;
   - menus visiveis;
   - botoes;
   - grids;
   - campos obrigatorios;
   - mensagens;
   - uso de memoria;
   - arquivos DB acessados;
   - conexoes TCP.
3. Cruzar captura com:
   - `mapa-telas-funcional-consolidado.csv`;
   - `mapa-menus-submenus-acoes-consolidado.csv`;
   - `mapa-ui-banco-interligacoes.csv`;
   - `mapa-permissoes-ui-consolidado.csv`.

## Conclusao

A UI do EquipeExe ja possui mapa estatico amplo o suficiente para orientar a reconstrucao. O ponto critico agora e transformar indicios estaticos em evidencia visual dinamica, tela por tela, preservando o caminho operacional dos usuarios.

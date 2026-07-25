# Mapa Tela -> Banco

Data: 2026-05-23

## Fonte

Este mapa consolida os vinculos estaticos entre interface e banco.

CSV detalhado:

- `D:\AtelieProd\MOD\docs\11-modulos\mapa-telas-banco.csv`

Origem:

- `docs\11-modulos\ui-reverse-engineering\mapa-ui-banco-interligacoes.csv`
- layouts Delphi TPF0;
- strings SQL embutidas;
- nomes de tabelas/datasets.

## Resultado

Vinculos UI -> banco/SQL consolidados: 757.

Distribuicao observada:

- `LavFacilLan`: 275
- `Estoque`: 153
- `LavSoft`: 144
- `SAT`: 122
- `Financeiro`: 34
- `NFE`: 16
- `Senhas`: 13

## Leitura por modulo

### LavSoft

Modulo principal. Usa tabelas e queries relacionadas a:

- cliente;
- ROL;
- caixa;
- entrega;
- relatorios;
- permissao;
- fiscal.

### LavFacilLan

Modulo com maior numero de vinculos UI-banco. Deve ser priorizado em captura dinamica porque provavelmente concentra telas com grids, consultas e rotinas operacionais.

### Estoque

Vinculos fortes com:

- produtos;
- movimentos de estoque;
- entrada/baixa;
- relatorios.

### SAT/NFE

Vinculos fiscais e de atendimento/ocorrencia.

### Financeiro

Vinculos com pagamento, nota, relatorio, impressao, licenca/bloqueio e permissao.

### Senhas

Vinculos com usuarios, nivel e permissoes.

## Como validar cada tela

Para cada tela real:

1. Abrir no runtime MOD.
2. Registrar caminho de menu.
3. Capturar screenshot.
4. Rodar ProcMon filtrado por processo.
5. Identificar `.DB`, `.PX`, `.INI`, `.XML` acessados.
6. Comparar com `mapa-telas-banco.csv`.
7. Registrar se a tela apenas consulta ou tambem grava.

## Campos obrigatorios no mapa dinamico futuro

- modulo;
- tela;
- menu origem;
- botao/acao;
- tabela lida;
- tabela gravada;
- campos lidos;
- campos gravados;
- usuario;
- permissao;
- tempo de abertura;
- impacto em memoria;
- logs gerados.

## Conclusao

O mapa atual e suficiente para orientar a captura tela-a-tela, mas ainda precisa de validacao dinamica para separar consulta, insercao, alteracao e exclusao reais.

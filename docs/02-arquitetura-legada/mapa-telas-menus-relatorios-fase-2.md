# Mapa Inicial de Telas, Menus e Relatorios - Fase 2

## Metodo

Foi feita extracao de strings dos executaveis principais:

- `LavSoft.exe`
- `LavFacilLan.exe`
- `Gerenciador.exe`
- `Financeiro.exe`
- `Estoque.exe`
- `NFE.exe`
- `SAT.exe`

Arquivos gerados:

- `strings-executaveis\*.strings.txt`
- `mapa-telas-menus-relatorios-inicial.csv`
- `mapa-executaveis-filtrado.csv`

## Resultado quantitativo

Itens filtrados no mapa inicial: 2.217.

Distribuicao por executavel e categoria esta em `mapa-telas-menus-relatorios-inicial.csv`.

## Achados iniciais

### LavSoft

Modulo principal de lavanderia/operacao. Foram encontradas referencias a:

- ROL, pecas, recepcao, entrega, caixa, cancelamento.
- Impressao fiscal, etiquetas, relatorios e reemissao.
- Usuario, senha e permissao em cancelamentos.
- Chamada externa para `Senhas.exe`.

### LavFacilLan

Modulo operacional relacionado ao LavSoft. Foram encontradas referencias a:

- Controle de lavagem, caixa, impressao, nota e permissao.
- Consultas de metadados InterBase/Firebird em strings internas, indicando bibliotecas de acesso mais amplas que o Paradox.
- Chamada externa para `Senhas.exe`.

### Gerenciador

Modulo com sinais de aplicacao .NET/Windows Forms:

- `menuStrip1`, `ToolStripMenuItem`, `MainMenuStrip`.
- Mensagens de usuario/senha invalida.
- Recursos `Gerenciador.Forms.Menu`.

### Financeiro

Modulo financeiro com referencias a:

- Nota fiscal, relatorios, impressao.
- CodUsuario, menu e rotinas financeiras.

### Estoque

Modulo de estoque com referencias a:

- Cadastro, produto, fiscal, relatorios e operacao de estoque.

### NFE

Modulo fiscal de nota eletronica com referencias a:

- NFE, nota, menu, operacao fiscal e impressao.

### SAT

Modulo de suporte/atendimento/fiscal com referencias a:

- SAT, solicitacoes, usuario, permissao, cadastro, ocorrencias e relatorios.

## Proxima validacao

O mapa por strings e indicativo, nao substitui execucao assistida. A proxima fase deve abrir cada modulo em homologacao, capturar telas e cruzar cada menu com:

- rotina interna;
- tabela Paradox;
- permissao em `Nivel.DB`;
- relatorio gerado;
- dependencia externa.

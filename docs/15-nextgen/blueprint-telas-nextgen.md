# Blueprint Telas NextGen

Data: 2026-05-23

## Objetivo

Definir as telas da nova geracao com base nos fluxos e entidades mapeados no legado.

## Telas prioritarias

### Clientes

Telas:

- Lista de clientes
- Cadastro/edicao de cliente
- Contatos do cliente
- Enderecos
- Observacoes
- Historico de ROL/OS
- Historico financeiro

Campos base:

- codigo legado;
- nome;
- CPF/CNPJ;
- RG/IE;
- telefone;
- celular/WhatsApp;
- email;
- endereco;
- bairro/cidade/UF/CEP;
- grupo;
- tabela de preco;
- status.

Acoes:

- criar;
- editar;
- inativar;
- consultar historico;
- abrir ROL para cliente;
- exportar/relatorio.

### OS / ROL

Telas:

- Entrada de ROL
- Consulta de ROL
- Detalhe do ROL
- Itens/pecas/servicos
- Localizacao/status
- Entrega
- Cancelamento
- Impressao/etiqueta

Campos base:

- numero ROL;
- numero OS legado;
- cliente;
- data entrada;
- previsao entrega;
- status;
- vendedor/atendente;
- total pecas;
- total valor;
- nota vinculada;
- observacoes.

Acoes:

- criar;
- editar;
- adicionar item;
- aplicar desconto;
- pagar;
- entregar;
- cancelar;
- imprimir.

### Produtos e Servicos

Telas:

- Lista de produtos/servicos
- Cadastro de produto
- Cadastro de servico
- Tabela de preco
- Kits/pacotes

Acoes:

- criar;
- editar;
- inativar;
- ajustar preco;
- vincular estoque;
- consultar uso em ROL.

### Estoque

Telas:

- Produtos de estoque
- Entrada
- Baixa
- Ajuste
- Encerramento
- Historico
- Relatorios

Campos base:

- produto;
- quantidade;
- tipo entrada/saida;
- valor unitario;
- valor total;
- motivo;
- usuario;
- data.

### Financeiro / Caixa

Telas:

- Caixa dia a dia
- Abertura/fechamento
- Recebimentos
- Duplicatas
- Creditos de cliente
- Contas a pagar
- Boletos
- Relatorios financeiros

Acoes:

- receber;
- baixar;
- estornar;
- cancelar;
- sangria;
- fechar caixa;
- emitir recibo.

### Fiscal

Telas:

- Notas fiscais
- Emissao
- Cancelamento
- SAT
- NFE
- Logs fiscais

### Permissoes

Telas:

- Usuarios
- Grupos/perfis
- Permissoes por modulo
- Permissoes por tela
- Permissoes por acao
- Auditoria de acessos

## Padroes UX

- lista + detalhe para cadastros;
- fluxo guiado para operacoes criticas;
- atalhos preservados em operacao de ROL/caixa;
- grids densos com filtros;
- logs por entidade;
- validacoes inline;
- confirmacao para acoes destrutivas.

## Mapa legado -> nextgen

- `Clientes.DB` -> tela Clientes.
- `MovCab.DB` -> tela OS/ROL.
- `Produt.DB`/`ProdEst.DB` -> telas Produtos/Estoque.
- `MovEst.DB` -> tela Movimentacao de estoque.
- `Duplicat.DB` -> tela Recebiveis.
- `Notas.DB`/`NotaFisPag.DB` -> telas Fiscal/Notas.
- `Usuarios`/`Nivel`/`Senhas` -> telas Permissoes.

## Observabilidade por tela

Toda tela deve registrar:

- tempo de abertura;
- consultas executadas;
- quantidade de registros;
- usuario;
- acao;
- erro;
- memoria;
- offline/online;
- pendencias de sync.

## Conclusao

As telas nextgen devem ser redesenhadas, mas nao reinventar a operacao. O caminho operacional atual precisa continuar reconhecivel para preservar produtividade.

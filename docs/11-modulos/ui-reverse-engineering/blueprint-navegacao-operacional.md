# Blueprint de Navegacao Operacional

Data: 2026-05-23

## Objetivo

Definir a leitura funcional da navegacao atual para orientar a reconstrucao moderna sem perder produtividade operacional.

## Principios de preservacao

- Preservar nomes operacionais familiares: ROL, entrega, caixa, cliente, estoque, nota fiscal, SAT/NFE.
- Manter atalhos de alta frequencia sempre visiveis.
- Evitar transformar fluxo operacional em pagina de marketing ou dashboard decorativo.
- Priorizar densidade organizada, leitura rapida e baixa latencia.
- Separar administracao, operacao, fiscal, financeiro, estoque e relatorios.

## Navegacao alvo por dominio

### Operacao Lavanderia / ROL

Entrada principal:

- Entrada/Lancamento de ROL
- Entrega de ROL
- Entrega por Pecas
- Pagamento de ROL
- Cancelamento/Reemissao
- Localizacao/Producao
- Passadoria/Terceirizacao

Padrao UX:

- tela de trabalho densa;
- busca rapida por ROL/cliente;
- grade central;
- acoes fixas por icone/botao;
- confirmacoes para cancelar/desbloquear.

### Caixa e Financeiro

Entrada principal:

- Caixa dia a dia
- Fechamento
- Pagamentos
- Creditos
- Cobrancas
- Faturamento
- Nota fiscal
- Descontos

Padrao UX:

- telas com resumo numerico;
- trilha de auditoria;
- permissao por acao;
- fechamento com etapas claras e rollback administrativo.

### Cadastros

Entrada principal:

- Clientes
- Usuarios
- Filiais
- Tabelas de preco
- Formas/condicoes de pagamento
- Servicos, tecidos, cores, marcas, defeitos

Padrao UX:

- lista + detalhe;
- busca persistente;
- filtros por status;
- historico de alteracao;
- importacao/exportacao controlada.

### Estoque

Entrada principal:

- Produtos
- Entrada no estoque
- Baixa de estoque
- Consulta de saldo
- Atualizacao/encerramento
- Relatorios de saida

Padrao UX:

- grade de produtos;
- movimentacao com origem/destino;
- estoque atual sempre visivel;
- relatorios acessiveis a partir da tela.

### Fiscal

Entrada principal:

- NFE
- SAT
- Impressora fiscal
- Nota fiscal
- Leitura X / memoria fiscal
- Destrava impressora fiscal

Padrao UX:

- status de certificado/dispositivo;
- logs fiscais;
- tratamento de falha claro;
- isolamento de licencas/componentes de terceiros.

### Relatorios

Entrada principal:

- Movimento analitico/sintetico
- Movimento por produto/servico
- Relatorio de entrega
- Conferencia de ROL
- Estoque no cliente
- Comissao
- Faturamento

Padrao UX:

- filtros reutilizaveis;
- preview;
- exportacao;
- agendamento futuro;
- permissao por relatorio.

### Administracao

Entrada principal:

- Usuarios
- Grupos
- Permissoes
- Filiais
- Logs
- Dispositivos
- Licenciamento
- Atualizacao
- Sincronizacao

Padrao UX:

- separado da operacao diaria;
- logs e auditoria sempre acessiveis;
- feature flags;
- acoes criticas com confirmacao.

## Modelo de permissao nextgen

Permissoes por acao:

- `rol.criar`
- `rol.entregar`
- `rol.cancelar`
- `caixa.fechar`
- `financeiro.baixar`
- `cliente.editar`
- `estoque.movimentar`
- `fiscal.emitir`
- `relatorio.visualizar`
- `usuarios.gerenciar`
- `devices.gerenciar`
- `licensing.gerenciar`

## Mapeamento legado -> moderno

Exemplo:

- `AlteraCliente1` -> `cliente.editar`
- `CFAbertura1` -> `caixa.abrir`
- `CFFechamento1` -> `caixa.fechar`
- `CFDestravaImpressoraFiscal1` -> `fiscal.impressora_destravar`
- `Anotacoes1` -> `operacao.anotacoes`

## Observabilidade de UI

Cada tela nova deve registrar:

- tempo de abertura;
- tempo de carregamento da grade;
- quantidade de registros carregados;
- consultas executadas;
- acoes do usuario;
- erros visiveis e silenciosos;
- consumo de memoria por tela;
- chamadas externas.

## Conclusao

A nova navegacao deve ser modular, mas nao pode esconder as rotinas operacionais que hoje estao a poucos cliques. O desenho correto e uma interface de trabalho: compacta, rapida, auditavel e familiar.

# Blueprint UX NextGen - EquipeExe

Data: 2026-05-23

## Objetivo

Projetar a futura UX do EquipeExe preservando produtividade operacional e familiaridade, mas com arquitetura moderna, observabilidade, permissoes granulares e operacao offline-first.

## Direcao de produto

A nova interface deve ser uma ferramenta de trabalho corporativa, nao uma landing page. O foco e operacao rapida, repetitiva, auditavel e leve.

## Estrutura proposta

### Shell principal

- barra superior compacta com filial, usuario, status offline/sync e alertas;
- navegacao lateral por modulo;
- area central de trabalho;
- rodape tecnico opcional com status de banco, fila e ultima sincronizacao;
- busca global por ROL, cliente, produto, nota e relatorio.

### Modulos

- Operacao / ROL
- Caixa
- Financeiro
- Clientes
- Estoque
- Fiscal
- Relatorios
- Administracao
- Sincronizacao
- Auditoria

## Padroes de tela

### Lista + detalhe

Para cadastros e consultas:

- filtro fixo no topo;
- grade densa;
- painel de detalhe lateral ou tela dedicada;
- acoes com permissao por botao.

### Fluxo guiado

Para fechamento, emissao fiscal, baixa e cancelamento:

- etapas claras;
- validacoes visiveis;
- resumo antes de confirmar;
- logs e rollback administrativo.

### Tela operacional rapida

Para ROL/entrega/caixa:

- entrada por codigo/leitor;
- atalhos de teclado;
- botoes principais fixos;
- impressao/etiqueta acessivel;
- minima troca de tela.

## Componentes padrao

- grids com coluna congelada, filtros e ordenacao;
- modais apenas para confirmacoes e escolhas pequenas;
- drawer/painel lateral para historico e auditoria;
- toolbar por tela com acoes principais;
- badges de status para pago, aberto, cancelado, entregue, pendente;
- logs por entidade.

## Observabilidade embutida

Cada tela deve medir:

- tempo de abertura;
- tempo de consulta;
- tamanho do resultado;
- erro de banco;
- erro de sync;
- erro de permissao;
- uso de memoria;
- eventos administrativos.

## Permissoes

Toda acao interativa deve passar por:

- permissao por modulo;
- permissao por tela;
- permissao por botao;
- permissao por entidade/filial quando necessario.

Exemplos:

- `rol.cancelar`
- `caixa.sangria`
- `financeiro.baixar_titulo`
- `fiscal.emitir_nfe`
- `estoque.ajustar_saldo`
- `admin.usuarios.editar`

## Offline-first

Estados visiveis:

- online;
- offline;
- sincronizando;
- pendencias;
- conflito;
- erro remoto.

Regras:

- operacao local continua sem internet;
- sync em fila;
- licenca local assinada;
- permissao cacheada;
- auditoria local obrigatoria.

## Supabase futuro

Supabase deve controlar:

- Auth;
- usuarios;
- permissoes;
- dispositivos;
- sessoes;
- licencas;
- feature flags;
- auditoria;
- telemetria administrativa.

O frontend deve tratar Supabase como camada remota, nunca como requisito unico para abrir telas operacionais comuns.

## Migracao UX

1. Recriar mapas e nomes atuais.
2. Capturar telas reais.
3. Definir equivalencia legado -> nextgen.
4. Criar prototipo de navegacao.
5. Rodar pilotos por modulo.
6. Ativar em paralelo com legado.
7. Medir tempo de tarefa e erros.
8. Migrar definitivamente por modulo.

## Conclusao

A UX nextgen deve ser moderna, mas nao "nova por vaidade". O usuario deve reconhecer a logica operacional, ganhar velocidade e ter mais seguranca, auditoria e clareza.

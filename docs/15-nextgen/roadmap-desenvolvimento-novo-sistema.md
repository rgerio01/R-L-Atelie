# Roadmap de Desenvolvimento do Novo Sistema

Data: 2026-05-23

## Fase 1 - Fundacao

- criar repositorio NextGen;
- definir stack;
- banco local;
- migracoes;
- usuarios/perfis/permissoes;
- auditoria;
- logs estruturados.

## Fase 2 - Dominio operacional

- clientes;
- produtos;
- estoque basico;
- OS/ROL;
- vendas;
- caixa;
- relatorios operacionais.

## Fase 3 - Pagamentos

- dinheiro;
- PIX via backend;
- conciliacao;
- webhooks;
- relatorio de pagamentos;
- taxa de servico.

## Fase 4 - Licenciamento

- planos;
- pagamento de licenca para Rogerio;
- vencimento;
- entitlement offline;
- dispositivos autorizados.

## Fase 5 - Cartao/Point

- cadastro de terminais;
- criacao de ordem Point;
- confirmacao por status/webhook;
- parcelamento;
- cancelamento/estorno;
- reconciliacao.

## Fase 6 - Migracao Paradox

- importador readonly;
- mapeamento entidade a entidade;
- validacao de totais;
- relatorios comparativos;
- rollback documentado.

## Fase 7 - Supabase/cloud

- tenant;
- RLS;
- sync;
- auditoria cloud;
- feature flags;
- painel administrativo.

## Fase 8 - Homologacao operacional

- teste com dados reais copiados;
- comparacao com EquipeExe;
- treinamento;
- ajuste de telas;
- aceite por modulo.

## Fase 9 - Go-live gradual

- iniciar com modulo nao critico;
- manter legado em paralelo;
- monitorar divergencias;
- ativar caixa/pagamentos por janela controlada;
- congelar escrita no legado somente apos aceite.

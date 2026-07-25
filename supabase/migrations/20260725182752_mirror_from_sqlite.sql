-- Espelho (backup) do banco local SQLite (pdv.db) + auth-store.json.
--
-- Este schema NAO é o banco principal do sistema — o kiosk/PDV continua
-- rodando 100% offline contra o SQLite local. Este é só um destino de
-- réplica, escrito periodicamente (a cada 3h) por
-- apps/tools/sync_to_supabase.py, usando a connection string direta
-- (bypassa RLS por ser role postgres). RLS fica ligado em todas as
-- tabelas sem nenhuma policy para anon/authenticated: ninguém lê isso
-- pela Data API pública ainda.
--
-- Sem FKs entre as tabelas de propósito: o objetivo é backup fiel dos
-- dados como estão no SQLite (que já não tem integridade referencial
-- garantida hoje — PRAGMA foreign_keys não é reforçado em toda conexão
-- no backend atual), não recriar constraints que podem rejeitar linhas
-- durante a sincronização.

create table if not exists configuracoes (
  chave text primary key,
  valor text not null,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table configuracoes enable row level security;

create table if not exists clientes (
  id bigint primary key,
  nome text not null,
  documento text,
  telefone text,
  celular text,
  email text,
  logradouro text,
  numero text,
  bairro text,
  cidade text,
  estado text,
  cep text,
  observacoes text,
  limite_credito numeric(14,2) not null default 0,
  desconto_percent numeric(5,2) not null default 0,
  ativo boolean not null default true,
  created_at timestamptz,
  updated_at timestamptz,
  created_by text not null default '',
  synced_at timestamptz not null default now()
);
alter table clientes enable row level security;

create table if not exists clientes_historico (
  id bigint primary key,
  cliente_id bigint not null,
  evento text not null,
  detalhe text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table clientes_historico enable row level security;
create index if not exists ix_clientes_historico_cliente on clientes_historico(cliente_id);

create table if not exists clientes_credito (
  cliente_id bigint primary key,
  saldo numeric(14,2) not null default 0,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table clientes_credito enable row level security;

create table if not exists clientes_credito_movimentos (
  id bigint primary key,
  cliente_id bigint not null,
  tipo text not null,
  valor numeric(14,2) not null,
  descricao text,
  referencia text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table clientes_credito_movimentos enable row level security;
create index if not exists ix_clientes_credito_mov_cliente on clientes_credito_movimentos(cliente_id);

create table if not exists servicos (
  id bigint primary key,
  codigo text not null,
  descricao text not null,
  categoria text not null default '',
  preco numeric(14,2) not null default 0,
  ativo boolean not null default true,
  created_at timestamptz,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table servicos enable row level security;

create table if not exists ordens_servico (
  id bigint primary key,
  numero text not null,
  cliente_id bigint not null,
  status text not null default 'aberta',
  data_entrada date,
  data_promessa date,
  data_entrega date,
  data_pagamento date,
  valor_total numeric(14,2) not null default 0,
  desconto numeric(14,2) not null default 0,
  valor_final numeric(14,2) not null default 0,
  valor_pago numeric(14,2) not null default 0,
  metodo_pagamento text,
  troco numeric(14,2) not null default 0,
  observacoes text,
  motivo_cancelamento text,
  usuario_entrada text not null default '',
  usuario_entrega text,
  usuario_pagamento text,
  created_at timestamptz,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table ordens_servico enable row level security;
create index if not exists ix_ordens_servico_cliente on ordens_servico(cliente_id);
create index if not exists ix_ordens_servico_status on ordens_servico(status);

create table if not exists os_itens (
  id bigint primary key,
  os_id bigint not null,
  servico_id bigint,
  descricao text not null,
  tipo_tecido text,
  cor text,
  marca text,
  defeito text,
  quantidade numeric(10,2) not null default 1,
  valor_unitario numeric(14,2) not null default 0,
  valor_total numeric(14,2) not null default 0,
  status text not null default 'pendente',
  observacao text,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table os_itens enable row level security;
create index if not exists ix_os_itens_os on os_itens(os_id);

create table if not exists os_historico (
  id bigint primary key,
  os_id bigint not null,
  evento text not null,
  status_anterior text,
  status_novo text,
  detalhe text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table os_historico enable row level security;
create index if not exists ix_os_historico_os on os_historico(os_id);

create table if not exists pagamentos (
  id bigint primary key,
  os_id bigint not null,
  metodo text not null,
  valor numeric(14,2) not null,
  troco numeric(14,2) not null default 0,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table pagamentos enable row level security;
create index if not exists ix_pagamentos_os on pagamentos(os_id);

create table if not exists caixa_sessoes (
  id bigint primary key,
  data date,
  usuario text not null,
  valor_abertura numeric(14,2) not null default 0,
  valor_contado numeric(14,2),
  status text not null default 'aberta',
  observacao_fechamento text,
  created_at timestamptz,
  fechado_em timestamptz,
  synced_at timestamptz not null default now()
);
alter table caixa_sessoes enable row level security;

create table if not exists caixa_movimentos (
  id bigint primary key,
  sessao_id bigint not null,
  tipo text not null,
  valor numeric(14,2) not null,
  descricao text,
  os_id bigint,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table caixa_movimentos enable row level security;
create index if not exists ix_caixa_movimentos_sessao on caixa_movimentos(sessao_id);

create table if not exists financeiro (
  id bigint primary key,
  cliente_id bigint not null,
  os_id bigint,
  tipo text not null default 'a_receber',
  status text not null default 'aberto',
  valor numeric(14,2) not null,
  vencimento date,
  data_recebimento date,
  valor_recebido numeric(14,2),
  metodo_recebimento text,
  observacao text,
  usuario text not null,
  created_at timestamptz,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table financeiro enable row level security;
create index if not exists ix_financeiro_cliente on financeiro(cliente_id);
create index if not exists ix_financeiro_status on financeiro(status);

create table if not exists legacy_records (
  id bigint primary key,
  tabela text not null,
  legacy_pk text,
  payload text not null,
  imported_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table legacy_records enable row level security;

create table if not exists orcamentos (
  id bigint primary key,
  numero text not null,
  cliente_id bigint not null,
  status text not null default 'aberto',
  data_entrada date,
  data_promessa date,
  data_validade date,
  valor_total numeric(14,2) not null default 0,
  desconto numeric(14,2) not null default 0,
  valor_final numeric(14,2) not null default 0,
  observacoes text,
  convertido_rol_id bigint,
  usuario_entrada text not null default '',
  created_at timestamptz,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table orcamentos enable row level security;
create index if not exists ix_orcamentos_cliente on orcamentos(cliente_id);

create table if not exists orc_itens (
  id bigint primary key,
  orc_id bigint not null,
  servico_id bigint,
  descricao text not null,
  tipo_tecido text,
  cor text,
  marca text,
  quantidade numeric(10,2) not null default 1,
  valor_unitario numeric(14,2) not null default 0,
  valor_total numeric(14,2) not null default 0,
  observacao text,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table orc_itens enable row level security;
create index if not exists ix_orc_itens_orc on orc_itens(orc_id);

create table if not exists agenda (
  id bigint primary key,
  rol_id bigint,
  orc_id bigint,
  cliente_id bigint not null,
  data_agendamento date not null,
  hora_agendamento text not null default '09:00',
  duracao_minutos integer not null default 30,
  tipo text not null default 'entrega',
  observacao text,
  status text not null default 'agendado',
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table agenda enable row level security;
create index if not exists ix_agenda_data on agenda(data_agendamento);

create table if not exists legacy_params (
  id bigint primary key,
  fonte text not null,
  secao text not null,
  chave text not null,
  valor text not null,
  synced_at timestamptz not null default now()
);
alter table legacy_params enable row level security;

create table if not exists legacy_coverage (
  id bigint primary key,
  area text not null,
  item text not null,
  fonte text not null,
  status text not null default 'pendente',
  observacao text,
  updated_by text,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table legacy_coverage enable row level security;

create table if not exists catalogos (
  id bigint primary key,
  tipo text not null,
  codigo text not null,
  descricao text not null,
  ativo boolean not null default true,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table catalogos enable row level security;

create table if not exists indenizacoes (
  id bigint primary key,
  os_id bigint,
  cliente_id bigint not null,
  descricao text not null,
  valor numeric(14,2) not null default 0,
  status text not null default 'aberta',
  motivo text,
  observacao text,
  usuario text not null,
  created_at timestamptz,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table indenizacoes enable row level security;
create index if not exists ix_indenizacoes_cliente on indenizacoes(cliente_id);

create table if not exists guardaroupa (
  id bigint primary key,
  cliente_id bigint not null,
  descricao text not null,
  categoria text,
  cor text,
  marca text,
  quantidade integer not null default 1,
  localizacao text,
  data_entrada date,
  data_saida date,
  status text not null default 'guardado',
  observacao text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table guardaroupa enable row level security;
create index if not exists ix_guardaroupa_cliente on guardaroupa(cliente_id);

create table if not exists terceirizacao (
  id bigint primary key,
  os_id bigint,
  fornecedor text not null,
  descricao text not null,
  valor numeric(14,2) not null default 0,
  data_envio date,
  data_retorno_prevista date,
  data_retorno date,
  status text not null default 'enviado',
  observacao text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table terceirizacao enable row level security;

create table if not exists fidelidade (
  cliente_id bigint primary key,
  pontos integer not null default 0,
  updated_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table fidelidade enable row level security;

create table if not exists fidelidade_movimentos (
  id bigint primary key,
  cliente_id bigint not null,
  pontos integer not null,
  tipo text not null,
  referencia text,
  observacao text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table fidelidade_movimentos enable row level security;
create index if not exists ix_fidelidade_mov_cliente on fidelidade_movimentos(cliente_id);

create table if not exists doacoes (
  id bigint primary key,
  os_id bigint,
  cliente_id bigint not null,
  descricao text not null,
  valor numeric(14,2) not null default 0,
  data_doacao date,
  status text not null default 'pendente',
  motivo_cancelamento text,
  observacao text,
  usuario text not null,
  created_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table doacoes enable row level security;
create index if not exists ix_doacoes_cliente on doacoes(cliente_id);

-- Licenciamento (fonte: auth-store.json, nao SQLite). So os campos
-- relevantes a licenca sao espelhados aqui — NUNCA hash/salt de senha
-- nem a SigningKey de assinatura de token. Autenticacao continua 100%
-- local; isso e so para o fornecedor acompanhar/gerenciar licencas de
-- forma centralizada.
create table if not exists usuarios_licenca (
  id uuid primary key,
  username text not null,
  display_name text,
  roles text[] not null default '{}',
  is_active boolean not null default true,
  must_change_password boolean not null default false,
  license_plano text,
  license_vence_em timestamptz,
  license_inicio_em timestamptz,
  last_login_at timestamptz,
  synced_at timestamptz not null default now()
);
alter table usuarios_licenca enable row level security;
create unique index if not exists ix_usuarios_licenca_username on usuarios_licenca(lower(username));

-- Nenhuma policy criada de propósito: com RLS ligado e sem policy,
-- anon/authenticated (as roles da Data API pública) não conseguem ler
-- nem escrever nada aqui. Só a conexão direta via SUPABASE_DB_URL (role
-- postgres, que ignora RLS) consegue.

-- Colunas que o espelho original deixou de fora: complemento (apto/bloco/casa)
-- e os campos de vinculo com o legado (Paradox), que impedem localizar o
-- registro correto na hora de atualizar endereco a partir do Equipexe.
alter table clientes add column if not exists complemento text;
alter table clientes add column if not exists legacy_codigo text;
alter table clientes add column if not exists data_nascimento text;
alter table clientes add column if not exists cartao_fidelidade text;
alter table clientes add column if not exists contato text;

create index if not exists ix_clientes_legacy_codigo on clientes(legacy_codigo);

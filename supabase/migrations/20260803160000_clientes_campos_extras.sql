-- Campos adicionais do cadastro legado que ainda faltavam: segundo telefone
-- extra, sexo, grupo/classificacao do cliente, vendedor responsavel e limite
-- de faturamento (existem no Equipexe, nao estavam sendo trazidos).
alter table clientes add column if not exists telefone3 text;
alter table clientes add column if not exists sexo text;
alter table clientes add column if not exists grupo_cliente text;
alter table clientes add column if not exists vendedor_codigo text;
alter table clientes add column if not exists limite_faturamento numeric(14,2);

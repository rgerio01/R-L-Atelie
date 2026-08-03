-- Hora de retirada (complementa data_promessa) usada no bloco "RETIRA NA
-- LOJA" impresso no recibo.
alter table ordens_servico add column if not exists hora_promessa text;

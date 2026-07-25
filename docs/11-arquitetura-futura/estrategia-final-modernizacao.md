# Estrategia Final de Modernizacao

Data: 2026-05-23

## Direcao

A modernizacao deve ser incremental, observavel e reversivel.

## Trilha 1 - Recuperacao operacional

- preservar original;
- operar homologacao em MOD;
- bloquear update legado no MOD;
- copiar dependencias controladamente;
- mapear menus, banco, relatorios e autenticacao;
- monitorar executaveis principais.

## Trilha 2 - Visibilidade total

- completar matriz de visibilidade;
- capturar execucoes dinamicas;
- mapear telas por evidencia visual;
- classificar comunicacoes externas;
- identificar licenciamento/hardware binding;
- mapear gargalos.

## Trilha 3 - Nucleo moderno

- consolidar API local;
- autenticar usuarios;
- gerenciar permissoes;
- registrar auditoria;
- criar banco local moderno;
- criar contratos de modulos.

## Trilha 4 - Migracao funcional

- migrar cadastros;
- migrar usuarios/permissoes;
- migrar financeiro/caixa;
- migrar operacional/ROL;
- migrar relatorios;
- migrar fiscal com extremo cuidado.

## Trilha 5 - Cloud hibrida

- introduzir tenant/company/branch;
- criar sync engine;
- integrar Supabase;
- ativar feature flags;
- ativar licenciamento central;
- ativar painel administrativo.

## Decisoes pendentes

- tecnologia final do app desktop;
- estrategia de UI;
- limites de operacao offline;
- politica de conflito financeiro;
- politica fiscal;
- politica de device binding;
- politica de atualizacao.

## Riscos criticos

- BDE/Paradox e locks;
- fiscal/SAT/NFE;
- update/sync legado;
- licenciamento oculto;
- dependencias 32-bit;
- drivers locais;
- ausencia de documentacao original.

## Recomendacao

Manter duas frentes:

- frente de laboratorio: entender e medir o legado;
- frente de arquitetura: preparar modulo moderno equivalente, sem pressa de substituir fluxos criticos.

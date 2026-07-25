# Classificacao de Entidades de Negocio

Data: 2026-05-23

## Clientes

Entidade: Clientes

Tabelas candidatas: CliContato, Clientes, Clientes-, ClientesInt, ClientesObs, FunCli, FunCliCan, FunCliRou, FunCliRouM, FunCliRouP, FunCliRouUni, GruClientes, GruClientes2, GruClientes3, GruClientes4

Campos principais: Clientes.CodCli, Clientes.CodTab, Clientes.CodPai, Clientes.CodUf, Clientes.CodCid, Clientes.CodRam, Clientes.CodCpg, Clientes.CodFpg, Clientes.CodTab2, Clientes.CodTab3, Clientes.CodTab4, Clientes.CodTab5, Clientes.CodTab6, Clientes.CodTab7, Clientes.CodTab8, Clientes.CodTab9, Clientes.CodEmpree, Clientes.CodTransp, Clientes.CodVen, Clientes.CodCliSec, Clientes.CodCon, Clientes.CodTipEnt, Clientes.CodVenCan, Clientes.CodCan, Clientes.CodFaixa, Clientes.CodConCli, Clientes.CodCart, CliContato.CodCli, Clientes.CodCli, Clientes.CodTab, Clientes.CodPai, Clientes.CodUf, Clientes.CodCid, Clientes.CodRam, Clientes.CodCpg, Clientes.CodFpg, Clientes.CodTab2, Clientes.CodTab3, Clientes.CodTab4, Clientes.CodTab5, Clientes.CodTab6, Clientes.CodTab7, Clientes.CodTab8, Clientes.CodTab9, Clientes.CodEmpree, Clientes.CodTransp, Clientes.CodVen, Clientes.CodCliSec, Clientes.CodCon, Clientes.CodTipEnt, Clientes.CodVenCan, Clientes.CodCan, Clientes.CodFaixa, Clientes.CodConCli, Clientes.CodCart, Clientes-.CodCli, Clientes-.CodTab, Clientes-.CodPai, Clientes-.CodUf, Clientes-.CodCid, Clientes-.CodRam, Clientes-.CodCpg, Clientes-.CodFpg, Clientes-.CodTab2, Clientes-.CodTab3, Clientes-.CodTab4, Cliente

Campos financeiros: Clientes.SaldoAnterior, Clientes.ObsSaldo, Clientes.SaldoAnterior, Clientes.ObsSaldo, Clientes-.SaldoAnterior, Clientes-.ObsSaldo, FunCliRou.IntAval, FunCliRou.Valor

Campos de status: Clientes.Status, Clientes.Status, Clientes-.Status, FunCli.SitFun, FunCliRou.SitRoupa, FunCliRouM.SitMan, FunCli.SitFun, FunCliRou.SitRoupa, FunCliRouM.SitMan

Campos de data: Clientes.EstEntCli, Clientes.DatCad, Clientes.EntPVisita, Clientes.TipoEntrega, Clientes.DatNas, Clientes.ConEmiCar, Clientes.EmiNot, Clientes.EmiBol, Clientes.EmitDup, Clientes.DatCan, Clientes.ComisVen, Clientes.OEmissor, Clientes.HorCan, Clientes.DiaPadPag, Clientes.ControlEnt, Clientes.DatFil, CliContato.DatNas, Clientes.EstEntCli, Clientes.DatCad, Clientes.EntPVisita, Clientes.TipoEntrega, Clientes.DatNas, Clientes.ConEmiCar, Clientes.EmiNot, Clientes.EmiBol, Clientes.EmitDup, Clientes.DatCan, Clientes.ComisVen, Clientes.OEmissor, Clientes.HorCan

Campos de relacionamento: Clientes.CodCli, Clientes.GruCli, Clientes.CodTab, Clientes.CodPai, Clientes.CodUf, Clientes.CodCid, Clientes.CodRam, Clientes.CodCpg, Clientes.CodFpg, Clientes.CodTab2, Clientes.CodTab3, Clientes.CodTab4, Clientes.CodTab5, Clientes.CodTab6, Clientes.CodTab7, Clientes.CodTab8, Clientes.CodTab9, Clientes.CodEmpree, Clientes.CodTransp, Clientes.CodVen, Clientes.Unidade, Clientes.CodCliSec, Clientes.CodCon, Clientes.GruCli2, Clientes.GruCli3, Clientes.GruCli4, Clientes.Marcacao, Clientes.MarcaAdm, Clientes.ResUsuario, Clientes.CodTipEnt, Clientes.CodVenCan, Clientes.CodCan, Clientes.CodFaixa, Clientes.CodConCli, Clientes.CodCart, CliContato.CodCli, Clientes.CodCli, Clientes.GruCli, Clientes.CodTab, Clientes.CodPai

Telas relacionadas: -Cliente:, :Clientes", :Clientes" C, !Importar Dados Cliente Fidelidade, "Clientes" C, [ Cliente Controla Entrada ], *Cliente, \Clientes, \Clientes.db" C, \Clientes", \Clientes" C, \Clientes" Where CgcCli =', \Clientes" Where CPFCli =', |Grupo em que pertence o cliente, |LogoMarca do cliente

Menus relacionados: Altera Cliente, Clientes Cadastros Entre, -Envia e-mail automaticamente para os clientes, Extrato Cliente, Extrato Cliente, Cliente:, |Grupo em que pertence o cliente, cadastrado em outro cliente., do cadastro de clientes todos os e-mails relacionados no arquivo, -Cliente:, :Clientes", :Clientes" C, !Importar Dados Cliente Fidelidade, "Clientes" C, [ Cliente Controla Entrada ]

Relatorios relacionados: digo Cliente na Etiqueta Lavavel, vel salvar o log do faturamento para o cliente atual, Impresso todos os Clientes, Confirma a Impress, Movimento por Grupo Clientes, o cliente acima Atendente (Somente Impressora 7-Compact), RELATORIO ESTOQUE NO CLIENTE

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## MovCab / OS / ROL

Entidade: MovCab / OS / ROL

Tabelas candidatas: CadLocRol, ControleEti, IndenRol, MovCab, MovCabDes, MovControle, MovLocRol

Campos principais: MovControle.CodCont, MovControle.Seq, MovControle.CodUsuario, MovControle.CodSistema, MovControle.SeqOcor, MovControle.CodCli, MovControle.CodTipCon, MovControle.CodUsuarioAnal, MovControle.CodUsuarioResp, MovControle.CodUsuarioConc, MovControle.CodContCon, MovControle.CodMelhor, MovControle.CodUsuarioMel, MovControle.CodUsuAprov, MovControle.CodRespConclu, MovControle.CodUsuAprovCom, MovControle.CodRespAnalise, MovControle.CodUsuarioConclu, CadLocRol.CodLocRol, ControleEti.Rol, ControleEti.CodPro, ControleEti.SeqPro, ControleEti.RolSeqPro, IndenRol.SeqInden, IndenRol.Rol, IndenRol.CodPro, IndenRol.SeqCai, MovCab.ROL, MovCab.CodCli, MovCab.CodTab, MovCab.CodTipSer, MovCab.CodTipEnt, MovCab.CodPra, MovCab.NumGR, MovCab.NumOS, MovCab.CodVen, MovCab.NumNot, MovCab.CodUsuario, MovCab.NumNotAdi, MovCab.SeqExport, MovCab.RolAnterior, MovCab.CodCliAnterior, MovCab.CodLoc, MovCab.CodEsp, MovCab.CodPas, MovCab.NumNotFis, MovCab.SeqCaixa, MovCab.CodDesc, MovCab.CodVenEnt, MovCab.CodLav, MovCab.CodConf, MovCab.RolOrc, MovCab.NumNotFisF, MovCab.CodVenCan, MovCabDes.Rol, MovCabDes.CodDesc, MovLocRol.Rol, MovLocRol.CodLocRol, MovLocRol.CodVen, MovLocRol.CodUsuario

Campos financeiros: MovCab.TotPecas

Campos de status: MovControle.Situacao, MovControle.HorAprov, MovControle.ObsAprov, MovControle.UsuAprov, MovControle.Status, MovControle.StatusInf, MovControle.StatusAprovCom, MovControle.UsuAprovCom, MovControle.HorAprovCom, MovCab.Posicao, MovCab.SitRol, MovCab.PosicaoAnt

Campos de data: MovControle.DatLan, MovControle.HorLan, MovControle.ObsEntrega, MovControle.DatAnalise, MovControle.DatPrevSol, MovControle.DatPrevExec, MovControle.DatConc, MovControle.DatAprov, MovControle.DatCan, MovControle.HorCan, MovControle.MotivoCan, MovControle.UsuarioCan, MovControle.DatAprovCom, MovControle.DatSegAna, CadLocRol.MotivoCan, CadLocRol.DatCan, ControleEti.Data, MovCab.DatEntLoja, MovCab.DatLan, MovCab.DatEnt, MovCab.ValTot, MovCab.DataCanc, MovCab.MotivoCanc, MovCab.HorLan, MovCab.HorEnt, MovCab.DatPas, MovCab.FixEntrega, MovCab.DatEntRol, MovCab.HorEntRol, MovCab.HorCan

Campos de relacionamento: MovControle.CodCont, MovControle.Seq, MovControle.CodUsuario, MovControle.CodSistema, MovControle.SeqOcor, MovControle.CodCli, MovControle.CodTipCon, MovControle.CodUsuarioAnal, MovControle.CodUsuarioResp, MovControle.CodUsuarioConc, MovControle.CodContCon, MovControle.CodMelhor, MovControle.CodUsuarioMel, MovControle.CodUsuAprov, MovControle.CodRespConclu, MovControle.CodUsuAprovCom, MovControle.CodRespAnalise, MovControle.CodUsuarioConclu, CadLocRol.CodLocRol, ControleEti.Rol, ControleEti.CodPro, ControleEti.SeqPro, ControleEti.RolSeqPro, IndenRol.SeqInden, IndenRol.Rol, IndenRol.CodPro, IndenRol.SeqCai, MovCab.ROL, MovCab.CodCli, MovCab.CodTab, MovCab.CodTipSer, MovCab.CodTipEnt, MovCab.CodPra, MovCab.NumGR, MovCab.NumOS, MovCab.CodVen, MovCab.NumNot, MovCab.Unidade, MovCab.CodUsuario, MovCab.NumNotAdi

Telas relacionadas: Desconto Porcentagem, RolOrc, -Select * From "C:\Equipexe\Lav\Filial\MovCab", -Select * From "S:\EquipExe\Lav\Filial\MovCab", ,from "c:\EquipExe\Lav\Filial\movcab" M,, *Select*From"C:\EquipExe\Lav\Filial\MovCab", + "c:\EquipExe\Lav\Filial\movcab" M, 8 'c:\Equipexe\Lav\Filial\MovCab' M,, AgruparporCdigoCliente1, AgruparporCdigoCliente1Click, ForcaCorteMeioRol, Informa o valor de desconto por Porcentagem, Tb_ClientesHorCan, Tb_MovItemCorCodProL, Tb_OrcCabCodUsuario

Menus relacionados: Desconto Porcentagem - Se valor final for 0,zera os valores, DescontoPorc1, Lb_DescontoPorc, DescontoPorc1, Lb_DescontoPorc, Filtros - Desconto por Valor ou Porcentagem, Ed_PorcDescontoPag, Ed_PorcDescontoPagExit, Ed_PorcDescontoPag, Ed_PorcDescontoPagExit, "ControlaDescontosporCadastri1Click, order By Z.SatPara,Z.CodOcorCla

Relatorios relacionados: hipotese por nome/string; captura pendente

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## Duplicat / Financeiro

Entidade: Duplicat / Financeiro

Tabelas candidatas: Boletos, BoletosCan, BoletosDupCan, BoletosLot, CliCredito, DupBoleto, Duplicat, FecCaixa, MovIniCaixa, TitGru, TitGruLan, Titulos

Campos principais: CliCredito.NumNot, CliCredito.CodCli, CliCredito.SeqExporta, CliCredito.CodPacote, CliCredito.Rol, FecCaixa.SeqCaixa, FecCaixa.CodUsuario, MovIniCaixa.Seq, TitGru.SeqGru, TitGru.CodFor, TitGruLan.SeqGru, TitGruLan.SeqGruLan, TitGruLan.CodCla, TitGruLan.CodSubC, TitGruLan.CodCenCus, Titulos.NumTit, Titulos.Seq, Titulos.CodFor, Titulos.CodBan, Titulos.CodAgeBan, Titulos.NumDev, Titulos.SeqCai, Titulos.CodCla, Titulos.SeqBan, Titulos.NumBan, Titulos.CodSubC, Titulos.CodCenCus, Titulos.CodOpe, Titulos.CodOpeSeq, Titulos.CodClaProj, Titulos.CodUnid, Boletos.NossoNumero, Boletos.CodUsuario, Boletos.SeqLot, BoletosCan.NossoNumero, BoletosCan.CodUsuario, BoletosDupCan.NossoNumero, BoletosDupCan.NumFat, BoletosDupCan.NumDup, BoletosLot.SeqLot, BoletosLot.CodUsuario, DupBoleto.Seq, DupBoleto.NumFat, DupBoleto.NumDup, DupBoleto.CodUsuario, DupBoleto.CodBarras, DupBoleto.idTransacao, DupBoleto.NumeroPedido, Duplicat.NumFat, Duplicat.NumDup, Duplicat.CodCli, Duplicat.CodFpg, Duplicat.CodCpg, Duplicat.CodBan, Duplicat.CodAgeBan, Duplicat.NumDev, Duplicat.SeqCai, Duplicat.CodVen, Duplicat.NumNot, Duplicat.NossoNumero, Duplicat.CodCla, Duplicat.CodSubC, Duplicat.CodCenCus, Duplicat.CodOpe, Duplica

Campos financeiros: CliCredito.ValCre, CliCredito.VerVal, MovIniCaixa.Valor, TitGruLan.Valor, Titulos.ValDev, Boletos.ValBol, DupBoleto.ValBoleto, Duplicat.ValFat, Duplicat.ValDup, Duplicat.ValDev, Duplicat.Juros

Campos de status: CliCredito.Sit, DupBoleto.Status

Campos de data: CliCredito.DatCre, CliCredito.DatVal, FecCaixa.DatEnc, MovIniCaixa.Data, MovIniCaixa.DatCan, MovIniCaixa.MotivoCan, TitGru.DatEmi, TitGru.DatVen, TitGru.ValTit, TitGru.DatCan, TitGru.MotivoCan, Titulos.DatEmi, Titulos.DatVen, Titulos.ValTot, Titulos.ValTit, Titulos.DatPag, Titulos.ValTitPag, Titulos.FixLan, Titulos.MotivoCan, Titulos.DatSusp, Titulos.DatCan, Boletos.DatEmi, Boletos.HorEmi, BoletosCan.DatCan, BoletosCan.HorCan, BoletosCan.MotivoCan, BoletosLot.DatEmi, BoletosLot.HorEmi, DupBoleto.DatEmi, Duplicat.DatEmi

Campos de relacionamento: CliCredito.NumNot, CliCredito.CodCli, CliCredito.SeqExporta, CliCredito.CodPacote, CliCredito.Rol, FecCaixa.SeqCaixa, FecCaixa.CodUsuario, MovIniCaixa.Seq, TitGru.SeqGru, TitGru.CodFor, TitGruLan.SeqGru, TitGruLan.SeqGruLan, TitGruLan.CodCla, TitGruLan.CodSubC, TitGruLan.CodCenCus, Titulos.NumTit, Titulos.Seq, Titulos.CodFor, Titulos.CodBan, Titulos.CodAgeBan, Titulos.NumDev, Titulos.SeqCai, Titulos.CodCla, Titulos.SeqBan, Titulos.NumBan, Titulos.CodSubC, Titulos.CodCenCus, Titulos.CodOpe, Titulos.CodOpeSeq, Titulos.CodClaProj, Titulos.CodUnid, Boletos.NossoNumero, Boletos.CodUsuario, Boletos.SeqLot, BoletosCan.NossoNumero, BoletosCan.CodUsuario, BoletosDupCan.NossoNumero, BoletosDupCan.NumFat, BoletosDupCan.NumDup, BoletosLot.SeqLot

Telas relacionadas: (|Selecione a forma de pagamento desejada, C:\EquipExe\Pag\Filial, Ed_FormaPagamento, Ed_FormaPagamentoExit, Ed_FormaPagamentoKeyPress, Ed_FormaPagamentoL, Forma de Pagamento, Forma de pagamento Cancelada, Forma de pagamento n, Forma de pagamento que esses cr, Informe a forma de pagamento padr, Pagar (Filial), Receber (Filial), Tb_ClientesContato, Tb_ClientesF5Contato

Menus relacionados: CaixaDiaDia1, CaixaDiaDia14, CaixaDiaDia1Click, Fechamento de Caixa Dia/Dia, Movimento de Caixa Dia/Dia, - use o Cancelamento do Pagamento, Tecle Enter para Continuar, "CancelamentodoUltimoEncerramCaixa1(, CFFundodeCaixa1, CFFundodeCaixa1@, CFFundodeCaixa1Click, CFFundodeCaixa1, CFFundodeCaixa1Click, CFSangriaCaixa1, CFSangriaCaixa1Click, CFSangriaCaixa1D

Relatorios relacionados: Qry_MovimentoContato, RelativePage, PadroesBoletoImpresso, Qry_MovimentoContato, AMENTO REFERENTE FECHAMENTO DE CAIXA RESUMO - FILIAL, Imprimindo Resumo do Caixa ..., rio realizar o cancelamento de pagamento de cada nota pelo Menu faturamento. Segue o n, Digite a Seq. do Caixa a Ser Impresso, PadroesBoletoImpresso, Qry_MovimentoSeqCaixa, Qry_MovimentoSeqCaixaL, RelativePage, Imprimindo Resumo do Caixa ..., o de Faturamentos no caixa matricial (O.F.), ComissaoPagamento1

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## Produt / Produtos

Entidade: Produt / Produtos

Tabelas candidatas: CadPro, CadProcAnexo, CadProcItem, CadProces, CadProcesAlt, CadProducao, CadProjSat, Produt, ProdutImp

Campos principais: CadPro.CodPro, CadProcAnexo.CodProces, CadProcAnexo.Seq, CadProces.CodProces, CadProces.CodClaProc, CadProcesAlt.CodProces, CadProcesAlt.Seq, CadProcesAlt.CodUsuario, CadProcItem.CodProces, CadProcItem.Seq, CadProcItem.CodProcInt, CadProducao.Codigo, Produt.CodPro, Produt.CodFor, Produt.CodProEst, Produt.CodEst, Produt.CodPro2, Produt.CodProTer, ProdutImp.CodPro, CadPro.CodPro, CadProjSat.CodProjSat, CadProjSat.CodCli

Campos financeiros: Produt.ValCom, Produt.ValComL, Produt.ISS, ProdutImp.Valor

Campos de status: CadProces.Status, CadProjSat.SitProj

Campos de data: CadPro.DatNas, CadProces.MotivoCan, CadProces.DatCan, CadProces.TipCad, CadProcesAlt.DatAlt, CadProcItem.DatCan, CadProcItem.MotivoCan, Produt.DatCan, Produt.MotivoCan, Produt.EntSai, Produt.BloqAltPre, Produt.TipPagTer, CadPro.DatNas, CadProjSat.DatIni, CadProjSat.DatPrevFin, CadProjSat.DatFin

Campos de relacionamento: CadPro.CodPro, CadProcAnexo.CodProces, CadProcAnexo.Seq, CadProces.CodProces, CadProces.Responsavel, CadProces.CodClaProc, CadProcesAlt.CodProces, CadProcesAlt.Seq, CadProcesAlt.CodUsuario, CadProcItem.CodProces, CadProcItem.Seq, CadProcItem.CodProcInt, CadProcItem.Responsavel, CadProducao.Codigo, Produt.CodPro, Produt.Unidade, Produt.CodFor, Produt.CodProEst, Produt.CodEst, Produt.CFOP, Produt.CodPro2, Produt.CodProTer, ProdutImp.CodPro, CadPro.CodPro, CadProjSat.CodProjSat, CadProjSat.CodCli, CadProjSat.SatPara

Telas relacionadas: )Desmarca o Produto no Controle de Limites, Cadastro de Marcas, cb_Servico, Desmarca produto marcado, DS_TabelaPreco, Marca produto, marcado para entrada posterior no estoque, No Financeiro, quando gerar proximo vez, estando marcado ir, Qry_TabelaPreco, SelecionaServico, SERVICOS PRESTADOS, TabelaPreco, TabelaPrecoClick, Tb_ClientesMarcaAdm, Tb_ClientesMarcacao

Menus relacionados: /Bloqueia Cores Repetidas no Controle de Entrada, Bloqueia Cores (Controle Ent.), Cadastro de Cores, CB_CoresEntrada, CB_CoresEntrada,, :\Equipexe\Sincroniza\Envio\Defeitos.xml, Defeitos encontrados no Produto, Cadastro de Defeitos, Defeitos encontrados no Produto, MarcaEntrega, MarcaEntregaClick, F7 - Marca/Desmarca Entrega, MarcaEntrega, MarcaEntregaClick, :\Equipexe\Sincroniza\Envio\Marcas.xml

Relatorios relacionados: Qry_MovimentoMarcaAdm, Qry_MovimentoMarcacao, Qry_MovimentoMarcaVenda, Qry_MovimentoMarcaAdm, Qry_MovimentoMarcaAdm$, Qry_MovimentoMarcacao, Qry_MovimentoMarcaVenda, Marca / Desmarca Impress, MarcaDesmarcaImpresso1, MarcaDesmarcaImpresso1Click, MarcaDesmarcaImpresso1D, MOVIMENTO P/ SERVICOS, o foi marcado nenhum delivery para impress, Qry_MovimentoPrecoCalculado, Qry_MovimentoPrecoCalculado|

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## ProdEst / Estoque Produto

Entidade: ProdEst / Estoque Produto

Tabelas candidatas: ProdEst, ProdEstKit, ProdEstPac, TabProdEst

Campos principais: ProdEst.CodProEst, ProdEst.CodUniPro, ProdEst.CodCla, ProdEst.CodSubC, ProdEst.CodMarEst, ProdEst.CodCenCus, ProdEst.CodGruEst1, ProdEst.CodGruEst2, ProdEst.CodGruEst3, ProdEst.CodGruEst4, ProdEst.CodClasFis, ProdEstKit.CodPro, ProdEstKit.CodEst, ProdEstKit.CodKit, ProdEstPac.CodProEst, ProdEstPac.CodProEstP, TabProdEst.CodTabEst, TabProdEst.CodProEst, ProdEst.CodProEst, ProdEst.CodUniPro, ProdEst.CodCla, ProdEst.CodSubC, ProdEst.CodMarEst, ProdEst.CodCenCus, ProdEst.CodGruEst1, ProdEst.CodGruEst2, ProdEst.CodGruEst3, ProdEst.CodGruEst4

Campos financeiros: nao identificado

Campos de status: nao identificado

Campos de data: ProdEst.MotivoCan, ProdEst.DatCan, ProdEst.MotivoCan, ProdEst.DatCan

Campos de relacionamento: ProdEst.CodProEst, ProdEst.CodUniPro, ProdEst.GruProEst, ProdEst.CodCla, ProdEst.CodSubC, ProdEst.CodMarEst, ProdEst.CodCenCus, ProdEst.CodGruEst1, ProdEst.CodGruEst2, ProdEst.CodGruEst3, ProdEst.CodGruEst4, ProdEst.CFOP, ProdEst.ICMS, ProdEst.IPI, ProdEst.CodClasFis, ProdEst.TipICMS, ProdEstKit.CodPro, ProdEstKit.CodEst, ProdEstKit.CodKit, ProdEstPac.CodProEst, ProdEstPac.CodProEstP, TabProdEst.CodTabEst, TabProdEst.CodProEst, ProdEst.CodProEst, ProdEst.CodUniPro, ProdEst.GruProEst, ProdEst.CodCla, ProdEst.CodSubC, ProdEst.CodMarEst, ProdEst.CodCenCus, ProdEst.CodGruEst1, ProdEst.CodGruEst2, ProdEst.CodGruEst3, ProdEst.CodGruEst4

Telas relacionadas: 0Select * From "C:\Equipexe\Est\Filial\MovEst" M,, Cadastro de Estoques, Estoque (Filial), Favor informar o estoque..., Nao constam produtos no estoque, prio estoque, favor verifique o cadastro, Tb_ClientesSaldoAnterior, Tb_MovEstCodUsuario, Tb_MovEstEncCalCodUsuario, Tb_MovEstEncCalCodUsuario8, ((((((M.ValTot * M.DescontoRol)/100)-M.ValTot)*-1)-M.DescontoValor)-N.ValBasNot)as Saldo, Tb_MovEstCFiscal, -Estoque:, (Encerramento|Faz Encerramento do estoque, | Estoque:

Menus relacionados: Entrada Estoque, EntradaEstoque, EntradaEstoque1, EntradaEstoque1<, EntradaEstoque1Click, Informa Rol no Estoque, Informa Rol no Estoque, InformaRolnoEstoque1, InformaRolnoEstoque1Click, InformaRolnoEstoque1Click%, RelEstoquenoCli1, RelEstoquenoCli1\, RelEstoquenoCli1Click, SaldoControleEntrada1, SaldoControleEntrada1Click

Relatorios relacionados: )Relatorio de controle de saida do estoque, Movimentos de Estoque, Movimentos|Movimentos de Estoque, ProdEstFilRel_FC.Qry_Movimento, -Movimento de Estoque - Estoque:, Grava Movimento de Estoque, Qry_MovimentoSaldo, Qry_MovimentoSaldoh, dia em 50% / * Faixa Cinza = Estoque maior que o movimento em 50%., Grava Movimento de Estoque, Movimento de Estoque - Estoque:, Movimento de Saldo, Movimento por produto - Saldo, Movimentoprodutosaldo1, Movimentoprodutosaldo1Click

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## MovEst / Movimento Estoque

Entidade: MovEst / Movimento Estoque

Tabelas candidatas: MovEst, MovEstCan, MovEstEnc, MovEstLan

Campos principais: MovEst.SeqLan, MovEst.CodEst, MovEst.CodProEst, MovEst.CodInteg, MovEst.CodUsuario, MovEst.CodClaEst, MovEst.SeqExporta, MovEst.CodFun, MovEstCan.CodEst, MovEstCan.CodProEst, MovEstCan.SeqLan, MovEstCan.CodUsuario, MovEstEnc.CodEst, MovEstEnc.CodProEst, MovEstEnc.CodUsuario, MovEstEnc.SeqExporta, MovEst.SeqLan, MovEst.CodEst, MovEst.CodProEst, MovEst.CodInteg, MovEst.CodUsuario, MovEst.CodClaEst, MovEst.SeqExporta, MovEstLan.Sequencia, MovEstLan.CodTab, MovEstLan.CodPro

Campos financeiros: MovEst.ValUnit, MovEst.ValUnit

Campos de status: nao identificado

Campos de data: MovEst.DatLan, MovEst.ValTot, MovEstCan.DatCan, MovEstCan.MotivoCan, MovEstEnc.DatEnc, MovEst.DatLan, MovEst.ValTot, MovEstLan.DatLan

Campos de relacionamento: MovEst.SeqLan, MovEst.CodEst, MovEst.CodProEst, MovEst.TipInteg, MovEst.CodInteg, MovEst.CodUsuario, MovEst.CodClaEst, MovEst.SeqExporta, MovEst.CodFun, MovEstCan.CodEst, MovEstCan.CodProEst, MovEstCan.SeqLan, MovEstCan.CodUsuario, MovEstEnc.CodEst, MovEstEnc.CodProEst, MovEstEnc.CodUsuario, MovEstEnc.SeqExporta, MovEst.SeqLan, MovEst.CodEst, MovEst.CodProEst, MovEst.TipInteg, MovEst.CodInteg, MovEst.CodUsuario, MovEst.CodClaEst, MovEst.SeqExporta, MovEstLan.Sequencia, MovEstLan.CodTab, MovEstLan.CodPro

Telas relacionadas: 0Select * From "C:\Equipexe\Est\Filial\MovEst" M,, Cadastro de Estoques, Estoque (Filial), Favor informar o estoque..., Nao constam produtos no estoque, prio estoque, favor verifique o cadastro, Tb_ClientesSaldoAnterior, Tb_MovEstCodUsuario, Tb_MovEstEncCalCodUsuario, Tb_MovEstEncCalCodUsuario8, ((((((M.ValTot * M.DescontoRol)/100)-M.ValTot)*-1)-M.DescontoValor)-N.ValBasNot)as Saldo, Tb_MovEstCFiscal, -Estoque:, (Encerramento|Faz Encerramento do estoque, | Estoque:

Menus relacionados: Entrada Estoque, EntradaEstoque, EntradaEstoque1, EntradaEstoque1<, EntradaEstoque1Click, Informa Rol no Estoque, Informa Rol no Estoque, InformaRolnoEstoque1, InformaRolnoEstoque1Click, InformaRolnoEstoque1Click%, RelEstoquenoCli1, RelEstoquenoCli1\, RelEstoquenoCli1Click, SaldoControleEntrada1, SaldoControleEntrada1Click

Relatorios relacionados: )Relatorio de controle de saida do estoque, Movimentos de Estoque, Movimentos|Movimentos de Estoque, ProdEstFilRel_FC.Qry_Movimento, -Movimento de Estoque - Estoque:, Grava Movimento de Estoque, Qry_MovimentoSaldo, Qry_MovimentoSaldoh, dia em 50% / * Faixa Cinza = Estoque maior que o movimento em 50%., Grava Movimento de Estoque, Movimento de Estoque - Estoque:, Movimento de Saldo, Movimento por produto - Saldo, Movimentoprodutosaldo1, Movimentoprodutosaldo1Click

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## Notas

Entidade: Notas

Tabelas candidatas: NotaSat, NotaSatCanc, Notas, NotasEsc

Campos principais: NotasEsc.NumNot, NotasEsc.CodCli, NotasEsc.CodUsuario, NotasEsc.CodTransp, NotasEsc.CodVen, NotasEsc.NumNotFis, Notas.NumNot, Notas.CodCli, Notas.CodUsuario, Notas.CodTransp, Notas.CodVen, Notas.NumNotFis, Notas.CodEsp, Notas.NumeroEsp, Notas.RolPrincip, Notas.SeqCaixa, Notas.SeqExport, Notas.NumNotFisF, Notas.CodVenCan, NotaSat.NumNotSat, NotaSat.CodCli, NotaSat.CodUsuario, NotaSatCanc.NumNotSat, NotaSatCanc.CodUsuario

Campos financeiros: NotasEsc.ValNot, NotasEsc.ValBasNot, NotasEsc.BaseISS, NotasEsc.ISS, Notas.ValNot, Notas.ValBasNot, Notas.BaseISS, Notas.ISS, NotaSat.ValNot, NotaSat.ValBasNot

Campos de status: nao identificado

Campos de data: NotasEsc.DatEmi, NotasEsc.DataCanc, NotasEsc.MotivoCanc, Notas.DatEmi, Notas.DataCanc, Notas.MotivoCanc, Notas.HorPag, Notas.HorCan, NotaSat.DatEmi, NotaSat.HorEmi, NotaSatCanc.DatCan, NotaSatCanc.HorCan, NotaSatCanc.MotivoCan

Campos de relacionamento: NotasEsc.NumNot, NotasEsc.CodCli, NotasEsc.CodUsuario, NotasEsc.TipNota, NotasEsc.CodTransp, NotasEsc.CodVen, NotasEsc.NumNotFis, Notas.NumNot, Notas.CodCli, Notas.CodUsuario, Notas.TipNota, Notas.CodTransp, Notas.CodVen, Notas.NumNotFis, Notas.CodEsp, Notas.MarcaEsp, Notas.NumeroEsp, Notas.RolPrincip, Notas.SeqCaixa, Notas.SeqExport, Notas.NumNotFisF, Notas.CodVenCan, Notas.CupNFiscal, NotaSat.NumNotSat, NotaSat.CodCli, NotaSat.CodUsuario, NotaSat.TipNota, NotaSatCanc.NumNotSat, NotaSatCanc.CodUsuario

Telas relacionadas: |Senha do usu, c_DigitaSenha, Digite a senha do usu, Ed_Senha, Ed_SenhaEnter, Ed_SenhaExit, Ed_SenhaKeyPress, EIdSocksServerPermissionError, F_Senha, F9 - Senha, FC_DigSenha, Im_Senha, Lb_TitSenha, Sb_Senha, Sb_SenhaClick

Menus relacionados: AlteraCliente1, AlteraCliente18, AlteraCliente1Click, AlteraCliente1Click$, AlteraodeProdutos1, AlteraodeProdutos1, CartadeCobranca1, CartadeCobranca1Click, ClientesCad1, CobrancaDiversas, CobrancaDiversasClick, CobrancaDiversas, CobrancaDiversasClick, ComissoRol1, ComissoRol1Click

Relatorios relacionados: A impressora n, a.RDB$RELATION_NAME, Abrir relat, Abrir...|Abre um relat, Arquivo de Relat, b.RDB$TYPE_NAME IN ('RELATION', 'VIEW',, CB_ImpressoNota, CB_Impressora, CImpressaoNoata_u, Confirma a impress, Database name: %s!The data were changed. Save them?3Relational operators require a field and a constant Expression expected but %s found, DBG_Movimento, DBG_MovimentoDblClick, DBG_MovimentoDblClick", DBG_MovimentoDrawColumnCell

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## NotaFisPag

Entidade: NotaFisPag

Tabelas candidatas: NotaFisPag

Campos principais: NotaFisPag.NumNotFis, NotaFisPag.CodVenCan

Campos financeiros: NotaFisPag.ValNot, NotaFisPag.ValBasNot, NotaFisPag.PorISS, NotaFisPag.ISS

Campos de status: nao identificado

Campos de data: NotaFisPag.DatEmi, NotaFisPag.DatCanc, NotaFisPag.MotCanc, NotaFisPag.HorCan

Campos de relacionamento: NotaFisPag.NumNotFis, NotaFisPag.TipNota, NotaFisPag.CodVenCan

Telas relacionadas: (|Selecione a forma de pagamento desejada, C:\EquipExe\Pag\Filial, Ed_FormaPagamento, Ed_FormaPagamentoExit, Ed_FormaPagamentoKeyPress, Ed_FormaPagamentoL, Forma de Pagamento, Forma de pagamento Cancelada, Forma de pagamento n, Forma de pagamento que esses cr, Informe a forma de pagamento padr, Pagar (Filial), Receber (Filial), Tb_ClientesContato, Tb_ClientesF5Contato

Menus relacionados: CaixaDiaDia1, CaixaDiaDia14, CaixaDiaDia1Click, Fechamento de Caixa Dia/Dia, Movimento de Caixa Dia/Dia, - use o Cancelamento do Pagamento, Tecle Enter para Continuar, "CancelamentodoUltimoEncerramCaixa1(, CFFundodeCaixa1, CFFundodeCaixa1@, CFFundodeCaixa1Click, CFFundodeCaixa1, CFFundodeCaixa1Click, CFSangriaCaixa1, CFSangriaCaixa1Click, CFSangriaCaixa1D

Relatorios relacionados: Qry_MovimentoContato, RelativePage, PadroesBoletoImpresso, Qry_MovimentoContato, AMENTO REFERENTE FECHAMENTO DE CAIXA RESUMO - FILIAL, Imprimindo Resumo do Caixa ..., rio realizar o cancelamento de pagamento de cada nota pelo Menu faturamento. Segue o n, Digite a Seq. do Caixa a Ser Impresso, PadroesBoletoImpresso, Qry_MovimentoSeqCaixa, Qry_MovimentoSeqCaixaL, RelativePage, Imprimindo Resumo do Caixa ..., o de Faturamentos no caixa matricial (O.F.), ComissaoPagamento1

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## SAT / Ocorrencias

Entidade: SAT / Ocorrencias

Tabelas candidatas: MovSatCli, MovSatCliOcor, MovSatCliPro, MovSatFor, MovSatForOcor, MovSatInt, MovSatIntOcor, NotaSat, NotaSatCanc

Campos principais: MovSatCli.SeqSatCli, MovSatCli.CodCli, MovSatCli.CodSatTip, MovSatCli.CodSatSit, MovSatCli.CodUsuario, MovSatCli.CodSatCla, MovSatCli.CodSatSubC, MovSatCli.CodSatTipS, MovSatCli.SeqOcor, MovSatCli.CodSatGru, MovSatCli.NumNotSat, MovSatCli.CodErro, MovSatCli.CodSatPad, MovSatCli.CodProces, MovSatCli.SeqProcCad, MovSatCliOcor.CodCli, MovSatCliOcor.SeqOcor, MovSatCliOcor.CodUsuario, MovSatCliOcor.CodOcorCla, MovSatCliOcor.CodOcorSubC, MovSatCliOcor.CodProces, MovSatCliOcor.SeqProcCad, MovSatCliPro.SeqSatCli, MovSatCliPro.CodPro, MovSatFor.SeqSatFor, MovSatFor.CodFor, MovSatFor.CodSatTip, MovSatFor.CodSatSit, MovSatFor.CodUsuario, MovSatFor.CodSatCla, MovSatFor.CodSatSubC, MovSatFor.CodSatTipS, MovSatFor.SeqOcor, MovSatFor.CodSatGru, MovSatFor.CodErro, MovSatFor.CodSatPad, MovSatFor.CodProces, MovSatFor.SeqProcCad, MovSatForOcor.CodFor, MovSatForOcor.SeqOcor, MovSatForOcor.CodUsuario, MovSatForOcor.CodOcorCla, MovSatForOcor.CodOcorSubC, MovSatForOcor.CodProces, MovSatForOcor.SeqProcCad, MovSatInt.SeqSatInt, MovSatInt.CodInt, MovSatInt.CodSatTip, MovSatInt.CodSatSit, MovSatInt.CodUsuario, MovSatInt.CodSatCla, MovSatInt.CodSatSubC, MovSatInt.CodSatTipS, MovSatInt.SeqOcor, MovSatInt.CodSa

Campos financeiros: MovSatCliOcor.ValOcor, MovSatCliPro.ValUnit, MovSatCliPro.ValFinal, MovSatForOcor.ValOcor, MovSatIntOcor.ValOcor, NotaSat.ValNot, NotaSat.ValBasNot

Campos de status: MovSatCliOcor.SitOcor, MovSatCliOcor.BloqVisual, MovSatForOcor.SitOcor, MovSatForOcor.BloqVisual, MovSatIntOcor.SitOcor, MovSatIntOcor.BloqVisual

Campos de data: MovSatCli.DatLan, MovSatCli.HorLan, MovSatCli.DatSolucao, MovSatCli.DatPrevSol, MovSatCliOcor.DatLan, MovSatCliOcor.HorLan, MovSatCliOcor.DatPrevSol, MovSatCliOcor.DatSol, MovSatCliOcor.DatAlerta, MovSatCliOcor.DatPrevOri, MovSatFor.DatLan, MovSatFor.HorLan, MovSatFor.DatSolucao, MovSatFor.DatPrevSol, MovSatForOcor.DatLan, MovSatForOcor.HorLan, MovSatForOcor.DatPrevSol, MovSatForOcor.DatSol, MovSatForOcor.DatAlerta, MovSatForOcor.DatPrevOri, MovSatInt.DatLan, MovSatInt.HorLan, MovSatInt.DatSolucao, MovSatInt.DatPrevSol, MovSatIntOcor.DatLan, MovSatIntOcor.HorLan, MovSatIntOcor.DatPrevSol, MovSatIntOcor.DatSol, MovSatIntOcor.DatAlerta, MovSatIntOcor.DatPrevOri

Campos de relacionamento: MovSatCli.SeqSatCli, MovSatCli.CodCli, MovSatCli.CodSatTip, MovSatCli.CodSatSit, MovSatCli.CodUsuario, MovSatCli.SatPara, MovSatCli.CodSatCla, MovSatCli.CodSatSubC, MovSatCli.CodSatTipS, MovSatCli.UsuSolucao, MovSatCli.SeqOcor, MovSatCli.CodSatGru, MovSatCli.NumNotSat, MovSatCli.CodErro, MovSatCli.CodSatPad, MovSatCli.CodProces, MovSatCli.SeqProcCad, MovSatCliOcor.CodCli, MovSatCliOcor.SeqOcor, MovSatCliOcor.CodUsuario, MovSatCliOcor.SatPara, MovSatCliOcor.CodOcorCla, MovSatCliOcor.CodOcorSubC, MovSatCliOcor.CodProces, MovSatCliOcor.SeqProcCad, MovSatCliPro.SeqSatCli, MovSatCliPro.CodPro, MovSatFor.SeqSatFor, MovSatFor.CodFor, MovSatFor.CodSatTip, MovSatFor.CodSatSit, MovSatFor.CodUsuario, MovSatFor.SatPara, MovSatFor.CodSatCla, MovSatFor.CodSatSubC, MovSatFor.CodSatTipS, MovSatFor.UsuSolucao, MovSatFor.SeqOcor, MovSatFor.CodSatGru, MovSatFor.CodErro

Telas relacionadas: Campos TipSat, CodSAt e SeqOcor n, CorSatCla, CorSatClaF, Formato de Imp. DANFE:, MovSatCliOcor, MovSatForOcor, Tb_ClientesDesativado, Tb_ClientesDesativado`, Tb_ClientesF5Desativado, Tb_ClientesF5Desativado<, Tb_ClientesF5MotivoDesat, Tb_ClientesF5MotivoDesat@, Tb_ClientesMotivoDesat, Tb_ClientesMotivoDesatd, :\EquipExe\EquSat

Menus relacionados: Cancelamento da Nota Fiscal, CancelamentodaNotaFiscal1, CancelamentoNotaFiscal1, CancelamentoNotaFiscal1Click, CancelamentoNotaFiscal1, CancelamentoNotaFiscal1|, CancelamentoNotaFiscal1Click, CFDestravaImpressoraFiscal1, CFDestravaImpressoraFiscal1Click, CFDestravaImpressoraFiscal1P, CFDestravaImpressoraFiscal1, CFDestravaImpressoraFiscal1Click, CFLeituraMemriaFiscal1, CFLeituraMemriaFiscal1Click, CFLeituraMemriaFiscal1Click'

Relatorios relacionados: Exibe Resumo por C. Fiscal, ResumoCFiscal, da Nota Fiscal a ser Impressa, NFeResumo, NFeResumo_c, NFeResumo_cPU, TNFeResumo, TNFeResumo_c, CFDestravaImpressoraFiscal1, CFDestravaImpressoraFiscal1Click, CFDestravaImpressoraFiscal1P, Destrava Impressora Fiscal, Impressora Cupom Fiscal General, Movimento Fiscal e N, vel com a impressora fiscal YANCO !

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

## Usuarios/Permissoes

Entidade: Usuarios/Permissoes

Tabelas candidatas: GruUsuarios, Nivel, Senhas, Usuarios

Campos principais: Nivel.CodUsuario, Nivel.CodFil, Nivel.CodSistema, Senhas.CodUsuario, Usuarios.CodUsuario, Usuarios.CodSubC

Campos financeiros: nao identificado

Campos de status: nao identificado

Campos de data: GruUsuarios.MotivoCan, GruUsuarios.DatCan, Nivel.DatCad, Nivel.HorCad, Usuarios.DatCad, Usuarios.DatCan, Usuarios.UsuarioCan, Usuarios.MotivoCan, Usuarios.HorCan

Campos de relacionamento: GruUsuarios.GruUsuario, GruUsuarios.ObsGruUsu, Nivel.CodUsuario, Nivel.CodFil, Nivel.CodSistema, Nivel.ResUsuario, Senhas.CodUsuario, Usuarios.CodUsuario, Usuarios.GruUsuario, Usuarios.TipUsuario, Usuarios.CopUsuario, Usuarios.CodSubC

Telas relacionadas: \Nivel.Db, Nivel, Tb_Nivel, Tb_Nivelh, *S:\basetestes\usuarios\guilherme\GER\DADOS, DS_Usuarios, GruUsuario, Insert Into TabUsuarios (Usuario,CodFranq,CodLoja,Senha,Tipo,Situacao) Values(, Insert Into TabUsuarios (Usuario,CodFranq,Senha,Tipo,Situacao) Values(, Tb_Usuarios, Tb_UsuariosCodUsuario, Tb_UsuariosCodUsuario0, Tb_UsuariosDatCad, Tb_UsuariosGruUsuario, Tb_UsuariosGruUsuario8

Menus relacionados: 'S:\BaseTestes\Usuarios\Carlos\Ger\Dados, +s:\basetestes\usuarios\guilherme\lav\filial, c_Usuarios, c_UsuariosAv, *S:\basetestes\usuarios\guilherme\GER\DADOS, )S:\BaseTestes\Usuarios\Arnaldo\Ger\Filial, DS_Usuarios, DS_Usuarios$, DS_Usuarios4, Ed_UsuSenhaSMTP, GruUsuario, :\EquipExe\Exe\Senhas.Exe, :\EquipExe\Exe\Senhas.Exe, 'S:\BaseTestes\Usuarios\Carlos\Ger\Dados, -Grupo Usuarios:

Relatorios relacionados: hipotese por nome/string; captura pendente

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

Nivel de confianca: alta

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.

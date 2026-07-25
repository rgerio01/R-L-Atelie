# Modelo de Dominio NextGen

Data: 2026-05-23

O modelo deve preservar evidência e rastreabilidade. Cada entidade moderna precisa guardar `legacy_table`, `legacy_key`, `evidence_status` e `migration_batch_id`.

## Clientes

Tabelas legadas: CliContato; Clientes; Clientes-; ClientesInt; ClientesObs; FunCli; FunCliCan; FunCliRou; FunCliRouM; FunCliRouP; FunCliRouUni; GruClientes; GruClientes2; GruClientes3; GruClientes4

Campos relacionamento: Clientes.CodCli; Clientes.GruCli; Clientes.CodTab; Clientes.CodPai; Clientes.CodUf; Clientes.CodCid; Clientes.CodRam; Clientes.CodCpg; Clientes.CodFpg; Clientes.CodTab2; Clientes.CodTab3; Clientes.CodTab4; Clientes.CodTab5; Clientes.CodTab6; Clientes.CodTab7; Clientes.CodTab8; Clientes.CodTab9; Clientes.CodEmpree; Clientes.CodTransp; Clientes.CodVen; Clientes.Unidade; Clientes.CodCliSec; Clientes.CodCon; Clientes.GruCli2; Clientes.GruCli3; Clientes.GruCli4; Clientes.Marcacao; Clientes.MarcaAdm; Clientes.ResUsuario; Clientes.CodTipEnt; Clientes.CodVenCan; Clientes.CodCan; Clientes.CodFaixa; Clientes.CodConCli; Clientes.CodCart; CliContato.CodCli; Clientes.CodCli; Clientes.GruCli; Clientes.CodTab; Clientes.CodPai

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## MovCab / OS / ROL

Tabelas legadas: CadLocRol; ControleEti; IndenRol; MovCab; MovCabDes; MovControle; MovLocRol

Campos relacionamento: MovControle.CodCont; MovControle.Seq; MovControle.CodUsuario; MovControle.CodSistema; MovControle.SeqOcor; MovControle.CodCli; MovControle.CodTipCon; MovControle.CodUsuarioAnal; MovControle.CodUsuarioResp; MovControle.CodUsuarioConc; MovControle.CodContCon; MovControle.CodMelhor; MovControle.CodUsuarioMel; MovControle.CodUsuAprov; MovControle.CodRespConclu; MovControle.CodUsuAprovCom; MovControle.CodRespAnalise; MovControle.CodUsuarioConclu; CadLocRol.CodLocRol; ControleEti.Rol; ControleEti.CodPro; ControleEti.SeqPro; ControleEti.RolSeqPro; IndenRol.SeqInden; IndenRol.Rol; IndenRol.CodPro; IndenRol.SeqCai; MovCab.ROL; MovCab.CodCli; MovCab.CodTab; MovCab.CodTipSer; MovCab.CodTipEnt; MovCab.CodPra; MovCab.NumGR; MovCab.NumOS; MovCab.CodVen; MovCab.NumNot; MovCab.Unidade; MovCab.CodUsuario; MovCab.NumNotAdi

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## Duplicat / Financeiro

Tabelas legadas: Boletos; BoletosCan; BoletosDupCan; BoletosLot; CliCredito; DupBoleto; Duplicat; FecCaixa; MovIniCaixa; TitGru; TitGruLan; Titulos

Campos relacionamento: CliCredito.NumNot; CliCredito.CodCli; CliCredito.SeqExporta; CliCredito.CodPacote; CliCredito.Rol; FecCaixa.SeqCaixa; FecCaixa.CodUsuario; MovIniCaixa.Seq; TitGru.SeqGru; TitGru.CodFor; TitGruLan.SeqGru; TitGruLan.SeqGruLan; TitGruLan.CodCla; TitGruLan.CodSubC; TitGruLan.CodCenCus; Titulos.NumTit; Titulos.Seq; Titulos.CodFor; Titulos.CodBan; Titulos.CodAgeBan; Titulos.NumDev; Titulos.SeqCai; Titulos.CodCla; Titulos.SeqBan; Titulos.NumBan; Titulos.CodSubC; Titulos.CodCenCus; Titulos.CodOpe; Titulos.CodOpeSeq; Titulos.CodClaProj; Titulos.CodUnid; Boletos.NossoNumero; Boletos.CodUsuario; Boletos.SeqLot; BoletosCan.NossoNumero; BoletosCan.CodUsuario; BoletosDupCan.NossoNumero; BoletosDupCan.NumFat; BoletosDupCan.NumDup; BoletosLot.SeqLot

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## Produt / Produtos

Tabelas legadas: CadPro; CadProcAnexo; CadProcItem; CadProces; CadProcesAlt; CadProducao; CadProjSat; Produt; ProdutImp

Campos relacionamento: CadPro.CodPro; CadProcAnexo.CodProces; CadProcAnexo.Seq; CadProces.CodProces; CadProces.Responsavel; CadProces.CodClaProc; CadProcesAlt.CodProces; CadProcesAlt.Seq; CadProcesAlt.CodUsuario; CadProcItem.CodProces; CadProcItem.Seq; CadProcItem.CodProcInt; CadProcItem.Responsavel; CadProducao.Codigo; Produt.CodPro; Produt.Unidade; Produt.CodFor; Produt.CodProEst; Produt.CodEst; Produt.CFOP; Produt.CodPro2; Produt.CodProTer; ProdutImp.CodPro; CadPro.CodPro; CadProjSat.CodProjSat; CadProjSat.CodCli; CadProjSat.SatPara

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## ProdEst / Estoque Produto

Tabelas legadas: ProdEst; ProdEstKit; ProdEstPac; TabProdEst

Campos relacionamento: ProdEst.CodProEst; ProdEst.CodUniPro; ProdEst.GruProEst; ProdEst.CodCla; ProdEst.CodSubC; ProdEst.CodMarEst; ProdEst.CodCenCus; ProdEst.CodGruEst1; ProdEst.CodGruEst2; ProdEst.CodGruEst3; ProdEst.CodGruEst4; ProdEst.CFOP; ProdEst.ICMS; ProdEst.IPI; ProdEst.CodClasFis; ProdEst.TipICMS; ProdEstKit.CodPro; ProdEstKit.CodEst; ProdEstKit.CodKit; ProdEstPac.CodProEst; ProdEstPac.CodProEstP; TabProdEst.CodTabEst; TabProdEst.CodProEst; ProdEst.CodProEst; ProdEst.CodUniPro; ProdEst.GruProEst; ProdEst.CodCla; ProdEst.CodSubC; ProdEst.CodMarEst; ProdEst.CodCenCus; ProdEst.CodGruEst1; ProdEst.CodGruEst2; ProdEst.CodGruEst3; ProdEst.CodGruEst4

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## MovEst / Movimento Estoque

Tabelas legadas: MovEst; MovEstCan; MovEstEnc; MovEstLan

Campos relacionamento: MovEst.SeqLan; MovEst.CodEst; MovEst.CodProEst; MovEst.TipInteg; MovEst.CodInteg; MovEst.CodUsuario; MovEst.CodClaEst; MovEst.SeqExporta; MovEst.CodFun; MovEstCan.CodEst; MovEstCan.CodProEst; MovEstCan.SeqLan; MovEstCan.CodUsuario; MovEstEnc.CodEst; MovEstEnc.CodProEst; MovEstEnc.CodUsuario; MovEstEnc.SeqExporta; MovEst.SeqLan; MovEst.CodEst; MovEst.CodProEst; MovEst.TipInteg; MovEst.CodInteg; MovEst.CodUsuario; MovEst.CodClaEst; MovEst.SeqExporta; MovEstLan.Sequencia; MovEstLan.CodTab; MovEstLan.CodPro

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## Notas

Tabelas legadas: NotaSat; NotaSatCanc; Notas; NotasEsc

Campos relacionamento: NotasEsc.NumNot; NotasEsc.CodCli; NotasEsc.CodUsuario; NotasEsc.TipNota; NotasEsc.CodTransp; NotasEsc.CodVen; NotasEsc.NumNotFis; Notas.NumNot; Notas.CodCli; Notas.CodUsuario; Notas.TipNota; Notas.CodTransp; Notas.CodVen; Notas.NumNotFis; Notas.CodEsp; Notas.MarcaEsp; Notas.NumeroEsp; Notas.RolPrincip; Notas.SeqCaixa; Notas.SeqExport; Notas.NumNotFisF; Notas.CodVenCan; Notas.CupNFiscal; NotaSat.NumNotSat; NotaSat.CodCli; NotaSat.CodUsuario; NotaSat.TipNota; NotaSatCanc.NumNotSat; NotaSatCanc.CodUsuario

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## NotaFisPag

Tabelas legadas: NotaFisPag

Campos relacionamento: NotaFisPag.NumNotFis; NotaFisPag.TipNota; NotaFisPag.CodVenCan

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## SAT / Ocorrencias

Tabelas legadas: MovSatCli; MovSatCliOcor; MovSatCliPro; MovSatFor; MovSatForOcor; MovSatInt; MovSatIntOcor; NotaSat; NotaSatCanc

Campos relacionamento: MovSatCli.SeqSatCli; MovSatCli.CodCli; MovSatCli.CodSatTip; MovSatCli.CodSatSit; MovSatCli.CodUsuario; MovSatCli.SatPara; MovSatCli.CodSatCla; MovSatCli.CodSatSubC; MovSatCli.CodSatTipS; MovSatCli.UsuSolucao; MovSatCli.SeqOcor; MovSatCli.CodSatGru; MovSatCli.NumNotSat; MovSatCli.CodErro; MovSatCli.CodSatPad; MovSatCli.CodProces; MovSatCli.SeqProcCad; MovSatCliOcor.CodCli; MovSatCliOcor.SeqOcor; MovSatCliOcor.CodUsuario; MovSatCliOcor.SatPara; MovSatCliOcor.CodOcorCla; MovSatCliOcor.CodOcorSubC; MovSatCliOcor.CodProces; MovSatCliOcor.SeqProcCad; MovSatCliPro.SeqSatCli; MovSatCliPro.CodPro; MovSatFor.SeqSatFor; MovSatFor.CodFor; MovSatFor.CodSatTip; MovSatFor.CodSatSit; MovSatFor.CodUsuario; MovSatFor.SatPara; MovSatFor.CodSatCla; MovSatFor.CodSatSubC; MovSatFor.CodSatTipS; MovSatFor.UsuSolucao; MovSatFor.SeqOcor; MovSatFor.CodSatGru; MovSatFor.CodErro

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos

## Usuarios/Permissoes

Tabelas legadas: GruUsuarios; Nivel; Senhas; Usuarios

Campos relacionamento: GruUsuarios.GruUsuario; GruUsuarios.ObsGruUsu; Nivel.CodUsuario; Nivel.CodFil; Nivel.CodSistema; Nivel.ResUsuario; Senhas.CodUsuario; Usuarios.CodUsuario; Usuarios.GruUsuario; Usuarios.TipUsuario; Usuarios.CopUsuario; Usuarios.CodSubC

Status inicial: confirmado por schema: tabelas/campos existentes | confirmado por UI: menus/telas correlatos


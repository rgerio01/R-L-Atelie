# Phase 3 — INI Files & Runtime Configuration
# Extracted: 2026-05-24

## MÓDULOS CONFIGURADOS

| Módulo | INI | Função |
|--------|-----|--------|
| LAV | EquLav.Ini (1073 linhas) | Lavanderia/Ateliê — módulo principal |
| GER | EquGer.Ini | Gerenciamento geral, usuários, filiais |
| PAG | EquPag.Ini | Pagamentos, caixa, cheques |
| PEC | EquPec.Ini | Pessoal/RH |
| EST | EquEst.Ini | Estoque |
| ESC | EquEsc.Ini | Escrituração fiscal |
| SAT | EquSat.Ini | SAT/ECF |
| MAN | EquMan.Ini | Manutenção |
| REC | EquRec.Ini | Contas a receber |

## FILIAIS CONFIGURADAS (41 total)

- 00001: R & L OLIVEIRA'S — filial principal da Luci
- ALVCLEAN LAVANDERIA (múltiplas)
- ALVEX LAVANDERIA
- LOPES MODA
- 99999: MOV. ANTIGA - SO BAIXAS (histórico)

## CONFIGURAÇÕES CRÍTICAS LAV

```ini
GeraNuvem=1          ; Sincroniza com nuvem LavSoft
GravaMensRol=1       ; Log de todas as OS
Impressora=Argox (PPLB) Lay Out 2
Impressora Fiscal=Bematech MP-FI
ISS=5%
CodTab=001           ; Tabela de preços padrão
ControlaPecas=1      ; Controla peças por OS
```

## CONFIGURAÇÕES PAG (Bancos)

1. Bradesco (Agência 2303)
2. Banco do Brasil
3. Caixa Econômica Federal
4. Banco Real
5. Itaú

## IMPRESSÃO

| Periférico | Modelo | Porta |
|-----------|--------|-------|
| Etiqueta | Argox Rabbit 214 (PPLB) | - |
| Fiscal | Bematech MP-FI | COM/LPT |
| Cheque | Bematech DP-32 | COM2 |
| Relatórios | HP 550C | LPT1 |
| Boleto | Epson LX matricial | LPT1 |

## CREDENCIAIS PLAINTEXT (RISCO DE SEGURANÇA)

BD.txt: `USER NAME=sa PASSWORD=123`
→ Credenciais BDE/Paradox em arquivo aberto — nunca replicar no NextGen

## APIS LAVSOFT DESCOBERTAS (strings nos executáveis)

### Host: http://www.lavsoft.com.br (HTTP — sem HTTPS!)

### Sync de Catálogos (POST):
- /EnviaCores, /EnviaDefeitos, /EnviaDelivery, /EnviaMarca
- /EnviaServicos, /EnviaCaract, /EnviaFatPre, /EnviaGruPro
- /EnviaPrazos, /EnviaFormasPagamento, /EnviaTabelasPreco
- /EnviaMovimento, /EnviaEntrega, /EnviaTipoEntrada
- Versão New: todos com sufixo "New"

### Recebimento de Dados (GET):
- /RecebeRolsFinalizados
- /RecebeRolsFinalizadosNew

### WebServices ASMX:
- /ws/Equipe/v2/Geral.ASMX
- /ws/Equipe/v2/AtuTabelas.asmx
- /ws/Sincrolav/Dados.asmx
- /ws/Graficos/wsGraficos.asmx
- /ws/minilav7/v3/Loja.asmx
- /ws/nuvem/v1/UploadArquivo.asmx
- /ws/Nuvem/Enviar

### Autenticação e Licenciamento LavSoft:
- GET /VerificaAtualizacoes
- GET /TestaAutentica         ← LICENÇA LAVSOFT
- GET /DownloadDados
- POST /AutenticaGerenciador  ← LOGIN NO GERENCIADOR
- POST /RegistraEstacao        ← REGISTRO DE DISPOSITIVO
- GET /ListarDispositivosPorFilial

## IMPLICAÇÕES PARA NEXTGEN

1. LiveUpdate.exe JÁ ESTÁ bloqueado em MOD/apps/services/LiveUpdate.Disabled/
2. A autenticação LavSoft (/TestaAutentica) PRECISA ser interceptada/bloqueada
3. O sync de dados para LavSoft (/EnviaMovimento etc.) não deve mais ocorrer
4. No NextGen, o sync vai para Supabase (já implementado)
5. Licensing: LavSoft usa /TestaAutentica — NextGen usa licencas + authorize-device

## ÚLTIMAS ATUALIZAÇÕES DOS INI

- EquLav.Ini: 21/05/2026 (ativo)
- EquGer99999.Ini: 26/12/2014 (histórico)
- Estruturas: 30/03/2017

## FORMATO EXECUTÁVEIS

- Delphi 32-bit (era Delphi 7)
- BDE (Borland Database Engine) para acesso Paradox
- WebServices via HTTP SOAP/ASMX (antigo)
- OpenSSL 32-bit incluído (libeay32.dll, ssleay32.dll)

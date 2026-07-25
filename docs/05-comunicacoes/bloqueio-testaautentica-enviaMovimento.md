# Bloqueio /TestaAutentica e /EnviaMovimento — LavSoft

Data de implementacao: 2026-05-24

---

## O que foi feito

Implementacao de intercept completo de todas as comunicacoes HTTP do EquipeExe MOD
com o servidor LavSoft (`www.lavsoft.com.br` / `191.6.218.152:80`).

Endpoints criticos protegidos:

| Endpoint | Metodo | Risco original | Resposta mock |
|---|---|---|---|
| `/TestaAutentica` | GET | Valida licenca com LavSoft — se falhar pode bloquear sistema | `1` (licenca valida) |
| `/AutenticaGerenciador` | POST | Autentica admin com servidor LavSoft | `1` (sucesso) |
| `/RegistraEstacao` | POST | Registra dispositivo com LavSoft | `1` (sucesso) |
| `/VerificaAtualizacoes` | GET | Dispara download de update (ja bloqueado pelo LiveUpdate stub) | `0` (sem updates) |
| `/EnviaMovimento*` | POST | Envia dados operacionais da filial para nuvem LavSoft | `1` (absorvido) |
| `/ws/Nuvem/Enviar` | POST | Upload de dados para nuvem LavSoft | SOAP `<Result>1</Result>` |
| Todos os outros | * | Timeout / crash se indisponivel | `1` (fallback) |

Endpoints `/DownloadDados` e `/VerificaAtualizacoes` ja estavam cobertos pelo
`LiveUpdate.Disabled`, mas agora tambem recebem resposta mock correta.

---

## Como foi feito

### Arquitetura — 3 camadas

```
EquipeExe MOD
    │
    ▼ HTTP GET/POST http://www.lavsoft.com.br/...
    │
[Camada 1 — hosts file]
    127.0.0.1  www.lavsoft.com.br
    127.0.0.1  lavsoft.com.br
    │ (redireciona DNS local)
    ▼ HTTP para 127.0.0.1:80
    │
[Camada 2 — LavSoft Mock Server]
    D:\AtelieProd\MOD\apps\services\LavSoftMock\server.py
    Python ThreadingHTTPServer porta 80
    - /TestaAutentica     → "1"
    - /EnviaMovimento*    → absorve POST, responde "1"
    - *.ASMX              → SOAP OK
    - todos os outros     → "1"
    - Loga tudo em logs\communication\lavsoft-mock-YYYYMMDD.jsonl
    │
[Camada 3 — Firewall outbound]
    Regras por executavel no MOD runtime:
    LavSoft.exe, Gerenciador.exe, LavFacilLan.exe, Estoque.exe,
    Financeiro.exe, SAT.exe, NFE.exe, LiveUpdate.exe
    → Block Outbound para Internet (nao afeta 127.0.0.1)
```

### Arquivos criados

| Arquivo | Funcao |
|---|---|
| `apps/services/LavSoftMock/server.py` | Mock HTTP server Python (porta 80) |
| `apps/tools/apply-lavsoft-intercept.ps1` | Aplica as 3 camadas (requer admin) |
| `apps/tools/rollback-lavsoft-intercept.ps1` | Reverte todas as camadas |
| `apps/tools/start-mod-safe.ps1` | Abre MOD com verificacao de protecoes ativas |

### Como aplicar (passo a passo)

```powershell
# 1. Abrir PowerShell como Administrador
# 2. Aplicar intercept (uma vez, persiste no Windows)
cd D:\AtelieProd\MOD\apps\tools
.\apply-lavsoft-intercept.ps1

# 3. Verificar no hosts file
Get-Content C:\Windows\System32\drivers\etc\hosts | Select-String "lavsoft"
# Esperado: 127.0.0.1  www.lavsoft.com.br
#           127.0.0.1  lavsoft.com.br

# 4. Testar mock server
Invoke-WebRequest http://www.lavsoft.com.br/TestaAutentica
# Esperado: StatusCode=200, Content="1"

# 5. Abrir EquipeExe MOD com verificacao
.\start-mod-safe.ps1
```

### Como reverter

```powershell
# PowerShell como Administrador
cd D:\AtelieProd\MOD\apps\tools
.\rollback-lavsoft-intercept.ps1
```

---

## Resultado esperado

### /TestaAutentica

Antes (sem intercept):
- EquipeExe tenta GET `http://www.lavsoft.com.br/TestaAutentica`
- Servidor LavSoft responde com validacao da licenca
- Se licenca expirar ou servidor mudar regras → sistema pode bloquear

Depois (com intercept):
- DNS resolve `www.lavsoft.com.br` → `127.0.0.1`
- Mock server responde `1` (licenca valida) imediatamente
- EquipeExe nunca sabe que nao chegou ao LavSoft
- Sistema continua operando normalmente, indefinidamente

### /EnviaMovimento

Antes (sem intercept):
- EquipeExe envia dados operacionais (ROLs, entregas, servicos) para nuvem LavSoft
- LavSoft armazena dados do cliente — risco de dependencia e privacidade
- GeraNuvem=1 no EquLav.Ini ativa esse sync automaticamente

Depois (com intercept):
- Dados chegam ao mock server, sao logados localmente e descartados
- Nenhum dado operacional sai para o LavSoft
- Resposta `1` garante que o EquipeExe nao entre em estado de erro
- Dados continuam sendo migrados/sincronizados via Supabase (NextGen)

### Observabilidade

Todos os requests interceptados sao logados em:
`D:\AtelieProd\MOD\logs\communication\lavsoft-mock-YYYYMMDD.jsonl`

Formato:
```json
{"ts":"2026-05-24T10:30:00","method":"GET","path":"/TestaAutentica","status":200,"body_size":0,"note":"known-endpoint"}
{"ts":"2026-05-24T10:30:05","method":"POST","path":"/EnviaMovimento","status":200,"body_size":4096,"note":"sync-absorbed"}
```

---

## Relacao com LiveUpdate.Disabled

| Componente | Bloqueia | Metodo |
|---|---|---|
| LiveUpdate.Disabled | `/VerificaAtualizacoes`, `/DownloadDados` | Stub executavel substituto |
| LavSoftMock + hosts | `/TestaAutentica`, `/EnviaMovimento`, todos os outros | Redirect DNS + mock server |
| Firewall outbound | Todos os executaveis para internet | Regras Windows Firewall por processo |

As 3 solucoes sao complementares e fornecem defesa em profundidade.

---

## Regra preservada

O sistema original em `D:\AtelieProd\Equipexe` NAO foi alterado.
O intercept atua apenas no MOD runtime em `D:\AtelieProd\MOD`.
Rollback completo disponivel sem impacto no EquipeExe original.

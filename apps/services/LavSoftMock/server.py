"""
LavSoft Mock Server — MOD Interceptor
Intercepta todas as requisicoes HTTP ao www.lavsoft.com.br/lavsoft.com.br
e retorna respostas que satisfazem o sistema legado EquipeExe.

Objetivo:
  - /TestaAutentica  → responde "licenca valida" para evitar bloqueio
  - /EnviaMovimento* → absorve dados operacionais (nao envia para LavSoft)
  - Todos os outros  → responde 200 OK para evitar timeout/crash

Porta: 80 (requer urlacl ou execucao como admin)
Host:  0.0.0.0 (aceita conexoes de qualquer interface local)

Executar:
  python server.py

Configuracao previa (admin, uma vez):
  netsh http add urlacl url=http://+:80/ user=EVERYONE
"""

import http.server
import json
import os
import datetime
import sys

sys.stdout.reconfigure(encoding="utf-8")
sys.stderr.reconfigure(encoding="utf-8")

LOG_DIR = r"D:\AtelieProd\MOD\logs\communication"
PORT = 80

# Respostas por endpoint (caminho sem query string, case-insensitive)
RESPONSES = {
    "/testaautentica":          ("text/plain", "1"),
    "/autenticagerenciador":    ("text/plain", "1"),
    "/registraestacao":         ("text/plain", "1"),
    "/verificaatualizacoes":    ("text/plain", "0"),
    "/testeverificaatualizacoes": ("text/plain", "0"),
    "/downloadDados":           ("application/octet-stream", b""),
    "/listardispositivosporfil": ("application/json", '{"dispositivos":[]}'),
    "/receberolsfinalizados":   ("application/json", '{"rols":[]}'),
    "/receberolsfinalizadosnew":("application/json", '{"rols":[]}'),
}

# Prefixos que absorvem dados de sync (POST)
SYNC_PREFIXES = (
    "/enviaMovimento", "/enviaentrega", "/enviamovimento",
    "/enviacores", "/enviadefeitos", "/enviadelivery", "/enviamarca",
    "/enviaservicos", "/enviacaract", "/enviaFatPre", "/enviaGruPro",
    "/enviaPrazos", "/enviaFormasPagamento", "/enviaTabelasPreco",
    "/enviaTipoEntrada", "/ws/nuvem", "/ws/sincrolav", "/ws/equipe",
    "/ws/graficos", "/ws/minilav",
)

SOAP_OK = """<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body><Result>1</Result></soap:Body>
</soap:Envelope>"""


def log_request(method, path, status, body_size, extra=""):
    ts = datetime.datetime.now().isoformat(timespec="seconds")
    today = datetime.date.today().strftime("%Y%m%d")
    log_file = os.path.join(LOG_DIR, f"lavsoft-mock-{today}.jsonl")
    entry = {
        "ts": ts,
        "method": method,
        "path": path,
        "status": status,
        "body_size": body_size,
        "note": extra,
    }
    try:
        os.makedirs(LOG_DIR, exist_ok=True)
        with open(log_file, "a", encoding="utf-8") as f:
            f.write(json.dumps(entry, ensure_ascii=False) + "\n")
    except Exception:
        pass
    print(f"[{ts}] {status} {method} {path} ({body_size}B) {extra}")


class LavSoftMockHandler(http.server.BaseHTTPRequestHandler):
    def log_message(self, fmt, *args):
        pass  # silencia log padrao do BaseHTTPRequestHandler

    def _read_body(self):
        length = int(self.headers.get("Content-Length", 0))
        return self.rfile.read(length) if length > 0 else b""

    def _respond(self, status, content_type, body):
        if isinstance(body, str):
            body = body.encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", content_type)
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Server", "LavSoft/2.0")
        self.end_headers()
        self.wfile.write(body)

    def _handle(self):
        method = self.command
        path_lower = self.path.split("?")[0].lower()
        body = self._read_body()

        # SOAP ASMX services
        if path_lower.endswith(".asmx"):
            log_request(method, self.path, 200, len(body), "soap-mock")
            self._respond(200, "text/xml; charset=utf-8", SOAP_OK)
            return

        # Endpoints conhecidos exatos
        for ep, (ct, resp) in RESPONSES.items():
            if path_lower == ep.lower():
                log_request(method, self.path, 200, len(body), "known-endpoint")
                self._respond(200, ct, resp)
                return

        # Prefixos de sync — absorve POST e responde 200
        for prefix in SYNC_PREFIXES:
            if path_lower.startswith(prefix.lower()):
                log_request(method, self.path, 200, len(body), "sync-absorbed")
                self._respond(200, "text/plain", "1")
                return

        # Qualquer outro endpoint — responde 200 OK vazio
        log_request(method, self.path, 200, len(body), "fallback")
        self._respond(200, "text/plain", "1")

    do_GET = _handle
    do_POST = _handle
    do_HEAD = _handle
    do_OPTIONS = _handle


def main():
    print("=" * 60)
    print("LavSoft Mock Server — MOD Interceptor")
    print(f"Porta: {PORT}  |  Log: {LOG_DIR}")
    print("Aguardando requisicoes do EquipeExe...")
    print("Ctrl+C para encerrar")
    print("=" * 60)

    server = http.server.ThreadingHTTPServer(("0.0.0.0", PORT), LavSoftMockHandler)
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\nServidor encerrado.")
        server.server_close()


if __name__ == "__main__":
    main()

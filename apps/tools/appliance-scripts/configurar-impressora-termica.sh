#!/usr/bin/env bash
# Configura a impressora termica (Diebold Procomp ou compativel ESC/POS) no CUPS
# como fila "raw" — sem driver/PPD, o app ja manda os bytes ESC/POS prontos
# (ver ImpressoraTestar/ImprimirRol em Program.cs). Rode isso UMA VEZ na maquina
# fisica de producao, com a impressora ja ligada e conectada via USB.
set -euo pipefail

echo "=== Detectando impressoras conectadas (USB) ==="
lpinfo -v | grep -i usb || {
  echo "Nenhuma impressora USB detectada. Verifique o cabo/energia e rode de novo."
  exit 1
}

echo
echo "Copie a linha 'direct usb://...' acima referente a Diebold Procomp."
read -rp "Cole aqui a URI completa (ex: usb://Diebold%20Procomp/T30?serial=...): " URI

if [ -z "$URI" ]; then
  echo "URI vazia — abortando."
  exit 1
fi

echo
echo "=== Criando fila 'termica' no CUPS (modo raw, sem driver) ==="
sudo lpadmin -p termica -E -v "$URI" -m raw
sudo cupsenable termica
sudo cupsaccept termica

echo
echo "=== Teste de impressao ==="
printf '\x1b@Ateliê da Luci\nTeste de impressao termica (Diebold Procomp)\n\n\n\x1dV\x01' | lp -d termica -o raw

echo
echo "OK! Fila 'termica' criada. No app, va em Configuracoes > Impressoras e"
echo "selecione 'termica' no campo 'Impressora termica'."

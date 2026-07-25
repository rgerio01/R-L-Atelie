<#
.SYNOPSIS
    Reverte a interceptacao LavSoft aplicada por apply-lavsoft-intercept.ps1.
.NOTES
    REQUER execucao como Administrador.
#>

#Requires -RunAsAdministrator

$ErrorActionPreference = "Stop"

$MOD = "D:\AtelieProd\MOD"
$HOSTS = "C:\Windows\System32\drivers\etc\hosts"

Write-Host "Revertendo intercept LavSoft..."

# Para mock server
$pidFile = "$MOD\logs\communication\lavsoft-mock.pid"
if (Test-Path $pidFile) {
    $pid = Get-Content $pidFile -Raw
    try {
        Stop-Process -Id ([int]$pid) -Force -ErrorAction SilentlyContinue
        Write-Host "  Mock server encerrado (PID=$pid)"
    } catch {}
    Remove-Item $pidFile -Force -ErrorAction SilentlyContinue
}

# Remove entradas do hosts
$lines = Get-Content $HOSTS
$filtered = $lines | Where-Object { $_ -notmatch "lavsoft\.com\.br" }
$filtered | Set-Content $HOSTS -Encoding ASCII
Write-Host "  hosts: entradas lavsoft.com.br removidas"

# Flush DNS
ipconfig /flushdns | Out-Null
Write-Host "  DNS cache limpo"

# Remove regras de firewall
Get-NetFirewallRule -DisplayName "MOD LavSoft Block Outbound*" -ErrorAction SilentlyContinue |
    ForEach-Object {
        Remove-NetFirewallRule -DisplayName $_.DisplayName
        Write-Host "  Firewall regra removida: $($_.DisplayName)"
    }

Write-Host ""
Write-Host "Rollback concluido. O EquipeExe MOD agora pode comunicar com LavSoft (modo original)."

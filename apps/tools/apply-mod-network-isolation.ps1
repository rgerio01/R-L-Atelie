param(
    [switch]$IncludeFiscal
)

$ErrorActionPreference = 'Stop'

$exeDir = 'D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe'
$report = 'D:\AtelieProd\MOD\docs\05-comunicacoes\firewall-isolamento-mod.csv'

$targets = @(
    'LavFacilLan.exe',
    'Estoque.exe',
    'LiveUpdate.exe'
)

if ($IncludeFiscal) {
    $targets += @('NFE.exe', 'SAT.exe')
}

$rows = New-Object System.Collections.Generic.List[object]

foreach ($name in $targets) {
    $path = Join-Path $exeDir $name
    if (!(Test-Path -LiteralPath $path)) {
        $rows.Add([pscustomobject]@{
            Name = $name
            Path = $path
            RuleName = ''
            Action = 'missing'
            Status = 'not_applied'
        })
        continue
    }

    $ruleName = "EquipeExe MOD Block Outbound - $name"
    try {
        $existing = Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue
        if ($existing) {
            Set-NetFirewallRule -DisplayName $ruleName -Enabled True -Action Block -Direction Outbound -ErrorAction Stop | Out-Null
            $action = 'updated'
        } else {
            New-NetFirewallRule `
                -DisplayName $ruleName `
                -Direction Outbound `
                -Program $path `
                -Action Block `
                -Profile Any `
                -Enabled True `
                -Description 'Bloqueio reversivel de comunicacao externa do runtime MOD EquipeExe.' `
                -ErrorAction Stop |
                Out-Null
            $action = 'created'
        }
        $status = 'enabled'
    } catch {
        $action = 'failed'
        $status = $_.Exception.Message
    }

    $rows.Add([pscustomobject]@{
        Name = $name
        Path = $path
        RuleName = $ruleName
        Action = $action
        Status = $status
    })
}

$rows |
    ConvertTo-Csv -NoTypeInformation |
    Out-File -LiteralPath $report -Encoding UTF8

Write-Output "Isolamento MOD aplicado/atualizado."
Write-Output "Relatorio: $report"

$ErrorActionPreference = 'Stop'

$prefix = 'EquipeExe MOD Block Outbound - '
$report = 'D:\AtelieProd\MOD\docs\05-comunicacoes\firewall-isolamento-mod-rollback.csv'

$rules = Get-NetFirewallRule -ErrorAction SilentlyContinue |
    Where-Object { $_.DisplayName -like "$prefix*" }

$rows = New-Object System.Collections.Generic.List[object]

foreach ($rule in $rules) {
    $rows.Add([pscustomobject]@{
        RuleName = $rule.DisplayName
        Enabled = $rule.Enabled
        Action = 'removed'
    })
    Remove-NetFirewallRule -Name $rule.Name
}

$rows |
    ConvertTo-Csv -NoTypeInformation |
    Out-File -LiteralPath $report -Encoding UTF8

Write-Output "Rollback de isolamento MOD concluido."
Write-Output "Regras removidas: $($rows.Count)"
Write-Output "Relatorio: $report"

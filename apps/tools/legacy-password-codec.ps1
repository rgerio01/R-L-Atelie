param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Encode', 'Decode')]
    [string]$Mode,

    [Parameter(Mandatory = $true)]
    [string]$Value
)

$ErrorActionPreference = 'Stop'

if ($Value.Length -gt 5) {
    throw 'O campo Senha legado observado aceita no maximo 5 caracteres.'
}

$delta = if ($Mode -eq 'Encode') { 1 } else { -1 }
$chars = foreach ($ch in $Value.ToCharArray()) {
    [char](([int][char]$ch) + $delta)
}

-join $chars

# Analise de Codificacao de Senha Legado

## Escopo

Analise realizada somente sobre a copia readonly:

D:\AtelieProd\MOD\data\original-readonly\Equipexe\Ger\Dados\Usuarios.DB

Nenhuma alteracao foi feita no legado.

## Resultado preliminar

Foram analisados 9 usuarios da tabela Usuarios.DB.

Distribuicao de tamanho do campo Senha:

- tamanho 5: 9 usuario(s)

O campo Senha tem tamanho fisico 10 no schema, mas os registros ativos observados usam 5 caracteres preenchidos.

## Hipotese forte

Foi gerado arquivo restrito com testes de deslocamento:

logs\analysis\restricted\analise-codificacao-senha-legado-restrito.csv

O arquivo inclui variacoes como:

- caractere ASCII -1;
- caractere ASCII -2;
- caractere ASCII -3;
- digito -1 circular;
- digito +1 circular;
- reversao da string.

Esta abordagem foi criada porque ha lembranca operacional de regra do tipo 1 vira 2, 2 vira 3, indicando possivel cifra simples por deslocamento.

O resultado observado e consistente com codificacao por deslocamento ASCII `+1` no armazenamento:

- para gravar uma senha no banco, cada caractere digitado parece ser armazenado como o proximo caractere ASCII;
- para conferir a senha digitada, o sistema provavelmente compara contra o valor armazenado ou decodifica com ASCII `-1`;
- exemplo tecnico sem usar senha real: valor digitado `12345` seria armazenado como `23456`.

Foi criado utilitario local de simulacao:

`D:\AtelieProd\MOD\apps\tools\legacy-password-codec.ps1`

Exemplo:

```powershell
D:\AtelieProd\MOD\apps\tools\legacy-password-codec.ps1 -Mode Encode -Value 12345
```

Resultado esperado: `23456`.

## Cuidado operacional

Mesmo que a codificacao seja confirmada, a alteracao de senha no legado nao deve ser feita direto no original. O caminho seguro e testar primeiro em copia/homologacao, porque o acesso tambem depende de UsuaSis.DB, Nivel.DB e possivelmente Senhas.exe.

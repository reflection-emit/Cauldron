# RsaAes.Encrypt Method (String, Byte[])
 _**\[This is preliminary documentation and is subject to change.\]**_

Encrypts data

**Namespace:**&nbsp;<a href="N_Cauldron_Cryptography">Cauldron.Cryptography</a><br />**Assembly:**&nbsp;Cauldron.Cryptography (in Cauldron.Cryptography.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static byte[] Encrypt(
	string publicKey,
	byte[] data
)
```


#### Parameters
&nbsp;<dl><dt>publicKey</dt><dd>Type: System.String<br />The public key to use to encrypt the data</dd><dt>data</dt><dd>Type: System.Byte[]<br />The data to encrypt</dd></dl>

#### Return Value
Type: Byte[]<br />The ecrypted data

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*publicKey* is null</td></tr><tr><td>ArgumentNullException</td><td>*data* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Cryptography_RsaAes">RsaAes Class</a><br /><a href="Overload_Cauldron_Cryptography_RsaAes_Encrypt">Encrypt Overload</a><br /><a href="N_Cauldron_Cryptography">Cauldron.Cryptography Namespace</a><br />
# Aes.Encrypt Method (SecureString, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Encrypts a string data

**Namespace:**&nbsp;<a href="N_Cauldron_Cryptography">Cauldron.Cryptography</a><br />**Assembly:**&nbsp;Cauldron.Cryptography (in Cauldron.Cryptography.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static byte[] Encrypt(
	SecureString password,
	string data
)
```


#### Parameters
&nbsp;<dl><dt>password</dt><dd>Type: System.Security.SecureString<br />The password to use to encrypt the data</dd><dt>data</dt><dd>Type: System.String<br />The data to encrypt. The string data will be converted to bytes using UTF8</dd></dl>

#### Return Value
Type: Byte[]<br />The encrypted data represented by bytes

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*password* is null</td></tr><tr><td>ArgumentNullException</td><td>*data* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Cryptography_Aes">Aes Class</a><br /><a href="Overload_Cauldron_Cryptography_Aes_Encrypt">Encrypt Overload</a><br /><a href="N_Cauldron_Cryptography">Cauldron.Cryptography Namespace</a><br />
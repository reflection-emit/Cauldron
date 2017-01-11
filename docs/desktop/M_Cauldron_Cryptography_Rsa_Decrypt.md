# Rsa.Decrypt Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Decrypts encrypted data

**Namespace:**&nbsp;<a href="N_Cauldron_Cryptography">Cauldron.Cryptography</a><br />**Assembly:**&nbsp;Cauldron.Cryptography (in Cauldron.Cryptography.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
[SecurityCriticalAttribute]
public static byte[] Decrypt(
	SecureString privateKey,
	byte[] data
)
```


#### Parameters
&nbsp;<dl><dt>privateKey</dt><dd>Type: System.Security.SecureString<br />The private key used to decrypt</dd><dt>data</dt><dd>Type: System.Byte[]<br />The data to decrypt</dd></dl>

#### Return Value
Type: Byte[]<br />The decrypted data

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*privateKey* is null</td></tr><tr><td>ArgumentNullException</td><td>*data* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Cryptography_Rsa">Rsa Class</a><br /><a href="N_Cauldron_Cryptography">Cauldron.Cryptography Namespace</a><br />
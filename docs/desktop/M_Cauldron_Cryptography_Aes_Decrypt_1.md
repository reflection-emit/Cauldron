# Aes.Decrypt Method (SecureString, Int32, Byte[])
 _**\[This is preliminary documentation and is subject to change.\]**_

Descrypts encrypted data

**Namespace:**&nbsp;<a href="N_Cauldron_Cryptography">Cauldron.Cryptography</a><br />**Assembly:**&nbsp;Cauldron.Cryptography (in Cauldron.Cryptography.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static byte[] Decrypt(
	SecureString password,
	int iterations,
	byte[] data
)
```


#### Parameters
&nbsp;<dl><dt>password</dt><dd>Type: System.Security.SecureString<br />The password used to decrypt data</dd><dt>iterations</dt><dd>Type: System.Int32<br />The number of iteration for the KDF</dd><dt>data</dt><dd>Type: System.Byte[]<br />The data to decrypt</dd></dl>

#### Return Value
Type: Byte[]<br />The decrypted data

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*password* is null</td></tr><tr><td>ArgumentNullException</td><td>*data* is null</td></tr><tr><td>SecurityException</td><td>*iterations* is lower than 1000</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Cryptography_Aes">Aes Class</a><br /><a href="Overload_Cauldron_Cryptography_Aes_Decrypt">Decrypt Overload</a><br /><a href="N_Cauldron_Cryptography">Cauldron.Cryptography Namespace</a><br />
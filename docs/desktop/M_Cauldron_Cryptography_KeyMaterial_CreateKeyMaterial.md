# KeyMaterial.CreateKeyMaterial Method (SecureString, Byte[], Int32)
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates a key material for the symmethric algorithm

**Namespace:**&nbsp;<a href="N_Cauldron_Cryptography">Cauldron.Cryptography</a><br />**Assembly:**&nbsp;Cauldron.Cryptography (in Cauldron.Cryptography.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static KeyMaterial CreateKeyMaterial(
	SecureString password,
	byte[] salt,
	int iterations = 60000
)
```


#### Parameters
&nbsp;<dl><dt>password</dt><dd>Type: System.Security.SecureString<br />The password used for the encryption or description</dd><dt>salt</dt><dd>Type: System.Byte[]<br />The salt forthe symmethric algorithm</dd><dt>iterations (Optional)</dt><dd>Type: System.Int32<br />The number of iteration for the KDF</dd></dl>

#### Return Value
Type: <a href="T_Cauldron_Cryptography_KeyMaterial">KeyMaterial</a><br />The key material

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*password* is null</td></tr><tr><td>ArgumentNullException</td><td>*salt* is null</td></tr><tr><td>SecurityException</td><td>*iterations* is lower than 1000</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Cryptography_KeyMaterial">KeyMaterial Class</a><br /><a href="Overload_Cauldron_Cryptography_KeyMaterial_CreateKeyMaterial">CreateKeyMaterial Overload</a><br /><a href="N_Cauldron_Cryptography">Cauldron.Cryptography Namespace</a><br />
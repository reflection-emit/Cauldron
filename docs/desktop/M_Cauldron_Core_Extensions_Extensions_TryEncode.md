# Extensions.TryEncode Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Tries to encode a byte array to a string by detecting its encoding. 

 It will try to detect the encoding for for UTF-7, UTF-8/16/32 (bom, no bom, little and big endian), and local default codepage, and potentially other codepages.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string TryEncode(
	this byte[] data
)
```


#### Parameters
&nbsp;<dl><dt>data</dt><dd>Type: System.Byte[]<br />The byte array that contains the string to be encoded</dd></dl>

#### Return Value
Type: String<br />The encoded string

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
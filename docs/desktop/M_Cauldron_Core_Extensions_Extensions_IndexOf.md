# Extensions.IndexOf Method (Byte[], Byte[])
 _**\[This is preliminary documentation and is subject to change.\]**_

Searches for the specified byte array and returns the zero-based index of the first occurrence within the entire Array

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static long IndexOf(
	this byte[] data,
	byte[] value
)
```


#### Parameters
&nbsp;<dl><dt>data</dt><dd>Type: System.Byte[]<br />The Array that could contain *value*</dd><dt>value</dt><dd>Type: System.Byte[]<br />The object to locate in the Array. The value can be null for reference types.</dd></dl>

#### Return Value
Type: Int64<br />The zero-based index of the first occurrence of item within the entire Array, if found; otherwise, –1.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_Extensions_IndexOf">IndexOf Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
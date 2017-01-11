# Extensions.Compress Method (String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Compresses a utf8 encoded string using gzip

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static byte[] Compress(
	this string data
)
```


#### Parameters
&nbsp;<dl><dt>data</dt><dd>Type: System.String<br />The data to be compressed</dd></dl>

#### Return Value
Type: Byte[]<br />The compressed string as byte array

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*data* is null</td></tr><tr><td>ArgumentException</td><td>*data* is empty</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_Extensions_Compress">Compress Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
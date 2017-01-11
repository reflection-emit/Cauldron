# ExtensionsConvertions.ToString Method (String, Object[])
 _**\[This is preliminary documentation and is subject to change.\]**_

Replaces the format item in a specified string with the string representation of a corresponding object in a specified array. This also takes into account that *source* can be an inline text for the TextBlock. CurrentCulture is used as IFormatProvider

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string ToString(
	this string source,
	params Object[] args
)
```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.String<br />The formatted string</dd><dt>args</dt><dd>Type: System.Object[]<br />An object array that contains zero or more objects to format.</dd></dl>

#### Return Value
Type: String<br />A copy of format in which the format items have been replaced by the string representation of the corresponding objects in *args*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsConvertions_ToString">ToString Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
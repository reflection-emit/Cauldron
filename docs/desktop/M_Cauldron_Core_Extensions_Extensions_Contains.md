# Extensions.Contains Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns a value indicating whether a specified substring occurs within this string.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool Contains(
	this string text,
	string value,
	StringComparison comparisonType
)
```


#### Parameters
&nbsp;<dl><dt>text</dt><dd>Type: System.String<br />The string to seek from.</dd><dt>value</dt><dd>Type: System.String<br />The string to seek.</dd><dt>comparisonType</dt><dd>Type: System.StringComparison<br />One of the enumeration values that specifies the rules for the search.</dd></dl>

#### Return Value
Type: Boolean<br />True if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
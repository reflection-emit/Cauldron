# Extensions.Right Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns a string containing a specified number of characters from the right side of a string.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string Right(
	this string source,
	int length
)
```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.String<br />String expression from which the rightmost characters are returned.</dd><dt>length</dt><dd>Type: System.Int32<br />Numeric expression indicating how many characters to return. If 0, a zero-length string (Empty) is returned. If greater than or equal to the number of characters in str, the entire string is returned.</dd></dl>

#### Return Value
Type: String<br />Returns a string containing a specified number of characters from the right side of a string.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
# ExtensionsConvertions.ToFloat Method (String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts the string representation of a number in a specified style and culture-specific format to its single-precision floating-point number equivalent. if the string content is "nan" then a NaN is returned.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static float ToFloat(
	this string target
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.String<br />A string that contains a number to convert.</dd></dl>

#### Return Value
Type: Single<br />A single-precision floating-point number equivalent to the numeric value or symbol specified in *target*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*target* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsConvertions_ToFloat">ToFloat Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
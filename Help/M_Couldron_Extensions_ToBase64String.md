# Extensions.ToBase64String Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64 digits.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static string ToBase64String(
	this byte[] target
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Byte[]<br />An array of 8-bit unsigned integers.</dd></dl>

#### Return Value
Type: String<br />The string representation, in base 64, of the contents of *target*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*target* is null.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
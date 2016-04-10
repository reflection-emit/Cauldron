# Extensions.EnclosedIn Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the string enclosed between two defined chars

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static string EnclosedIn(
	this string target,
	char start = '(',
	char end = ')'
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.String<br />The string where to find the enclosed string</dd><dt>start (Optional)</dt><dd>Type: System.Char<br />The starting enclosing character</dd><dt>end (Optional)</dt><dd>Type: System.Char<br />The ending enclosing character</dd></dl>

#### Return Value
Type: String<br />The enclosed string

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
# ExtensionsConvertions.ToBool Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts a string to bool. 

 This will first try to compare the string to "true" and then to "false". If both fails then it will use TryParse(String, Boolean) to parse the string to bool. If that also fails then the string will be compared to "1".

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool ToBool(
	this string target
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.String<br />The value to convert</dd></dl>

#### Return Value
Type: Boolean<br />Returns true if the string is equivalent to true; otherwise false.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type String. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*target* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
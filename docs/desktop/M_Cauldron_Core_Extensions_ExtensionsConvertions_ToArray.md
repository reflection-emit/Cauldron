# ExtensionsConvertions.ToArray Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts a IEnumerable to an array

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Object ToArray(
	this IEnumerable source,
	Type elementType
)
```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.Collections.IEnumerable<br />The IEnumerable to convert</dd><dt>elementType</dt><dd>Type: System.Type<br />The element type contained in the IEnumerable</dd></dl>

#### Return Value
Type: Object<br />An array of *elementType*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IEnumerable. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>source is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
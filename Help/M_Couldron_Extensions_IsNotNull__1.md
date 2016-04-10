# Extensions.IsNotNull(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Checks if the value is null. If not, it will invoke *action*

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void IsNotNull<T>(
	this T value,
	Action<T> action
)

```


#### Parameters
&nbsp;<dl><dt>value</dt><dd>Type: *T*<br />The value to check</dd><dt>action</dt><dd>Type: System.Action(*T*)<br />The action to invoke if *value* is not null</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the value</dd></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
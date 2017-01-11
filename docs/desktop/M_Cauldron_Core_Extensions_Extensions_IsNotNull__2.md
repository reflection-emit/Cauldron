# Extensions.IsNotNull(*T*, *TResult*) Method (*T*, Func(*T*, *TResult*))
 _**\[This is preliminary documentation and is subject to change.\]**_

Checks if the value is null. If not, it will invoke *func*

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static TResult IsNotNull<T, TResult>(
	this T value,
	Func<T, TResult> func
)
where T : class

```


#### Parameters
&nbsp;<dl><dt>value</dt><dd>Type: *T*<br />The value to check</dd><dt>func</dt><dd>Type: System.Func(*T*, *TResult*)<br />The func to invoke if *value* is not null</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the value</dd><dt>TResult</dt><dd>The return type</dd></dl>

#### Return Value
Type: *TResult*<br />The result of *value*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_Extensions_IsNotNull">IsNotNull Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
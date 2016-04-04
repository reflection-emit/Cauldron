# Extensions.Contains(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Determines whether an element is in the array

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static bool Contains<T>(
	this T[] array,
	Func<T, bool> predicate
)

```


#### Parameters
&nbsp;<dl><dt>array</dt><dd>Type: *T*[]<br />The array that could contain the item</dd><dt>predicate</dt><dd>Type: System.Func(*T*, Boolean)<br />A function to test each element for a condition.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of elements in the array</dd></dl>

#### Return Value
Type: Boolean<br />true if item is found in the array; otherwise, false.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
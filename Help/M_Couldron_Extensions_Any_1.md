# Extensions.Any Method (IEnumerable, Func(Object, Boolean))
 _**\[This is preliminary documentation and is subject to change.\]**_

Determines whether any element of a sequence satisfies a condition.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static bool Any(
	this IEnumerable source,
	Func<Object, bool> predicate
)
```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.Collections.IEnumerable<br />An IEnumerable whose elements to apply the predicate to</dd><dt>predicate</dt><dd>Type: System.Func(Object, Boolean)<br />A function to test each element for a condition.</dd></dl>

#### Return Value
Type: Boolean<br />True if any elements in the source sequence pass the test in the specified predicate, otherwise false

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IEnumerable. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="Overload_Couldron_Extensions_Any">Any Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
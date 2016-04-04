# Extensions.IndexOf(*T*) Method (*T*[], *T*)
 _**\[This is preliminary documentation and is subject to change.\]**_

Searches for the specified object and returns the zero-based index of the first occurrence within the entire Array

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static int IndexOf<T>(
	this T[] target,
	T value
)

```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: *T*[]<br />The Array that could contain *value*</dd><dt>value</dt><dd>Type: *T*<br />The object to locate in the Array. The value can be null for reference types.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the Array</dd></dl>

#### Return Value
Type: Int32<br />The zero-based index of the first occurrence of item within the entire Array, if found; otherwise, –1.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="Overload_Couldron_Extensions_IndexOf">IndexOf Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
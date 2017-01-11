# Extensions.Swap(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Swaps two elements in a collection

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IList<T> Swap<T>(
	this IList<T> collection,
	T a,
	T b
)

```


#### Parameters
&nbsp;<dl><dt>collection</dt><dd>Type: System.Collections.Generic.IList(*T*)<br />The collection where the elements should be swaped</dd><dt>a</dt><dd>Type: *T*<br />The first element to swap</dd><dt>b</dt><dd>Type: *T*<br />The second element to swap with</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type that is contained in the collection</dd></dl>

#### Return Value
Type: IList(*T*)<br />The collection

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IList(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
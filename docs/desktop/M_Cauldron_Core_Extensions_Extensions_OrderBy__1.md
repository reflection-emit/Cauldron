# Extensions.OrderBy(*TElements*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Sorts the elements of a sequence in ascending order according to a key.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IOrderedEnumerable<TElements> OrderBy<TElements>(
	this Array array,
	Func<TElements, bool> keySelector
)

```


#### Parameters
&nbsp;<dl><dt>array</dt><dd>Type: System.Array<br />A sequence of values to order.</dd><dt>keySelector</dt><dd>Type: System.Func(*TElements*, Boolean)<br />A function to extract a key from an element.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TElements</dt><dd>Der Typ der Elemente von source.</dd></dl>

#### Return Value
Type: IOrderedEnumerable(*TElements*)<br />An IOrderedEnumerable(TElement) whose elements are sorted according to a key.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Array. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
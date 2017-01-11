# Extensions.Distinct(*TSource*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns distinct elements from a sequence by using a selector to compare values.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<TSource> Distinct<TSource>(
	this IEnumerable<TSource> source,
	Func<TSource, TSource, bool> selector
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.Collections.Generic.IEnumerable(*TSource*)<br />The sequence to remove duplicate elements from.</dd><dt>selector</dt><dd>Type: System.Func(*TSource*, *TSource*, Boolean)<br />An expression used to determines whether the specified object are equal</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements of *source*</dd></dl>

#### Return Value
Type: IEnumerable(*TSource*)<br />An IEnumerable(T) that contains distinct elements from the source sequence.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IEnumerable(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
# ExtensionsConvertions.ToKeyedCollection(*TKey*, *TSource*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates a KeyedCollection(TKey, TItem) from an IEnumerable(T) according to a specified key selector function.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static KeyedCollection<TKey, TSource> ToKeyedCollection<TKey, TSource>(
	this IEnumerable<TSource> source,
	Func<TSource, TKey> keySelector
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.Collections.Generic.IEnumerable(*TSource*)<br />An IEnumerable(T) to create a KeyedCollection(TKey, TItem) from.</dd><dt>keySelector</dt><dd>Type: System.Func(*TSource*, *TKey*)<br />A function to extract a key from each element.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TKey</dt><dd>The type of the key returned by keySelector.</dd><dt>TSource</dt><dd>The type of the elements of source.</dd></dl>

#### Return Value
Type: KeyedCollection(*TKey*, *TSource*)<br />A KeyedCollection(TKey, TItem) that contains values of *source*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IEnumerable(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
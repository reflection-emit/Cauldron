# ConcurrentKeyedCollectionEx(*TKey*, *TItem*) Constructor (Func(*TItem*, *TKey*), IEqualityComparer(*TKey*), Int32)
 _**\[This is preliminary documentation and is subject to change.\]**_

Initializes a new instance of the <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">ConcurrentKeyedCollectionEx(TKey, TItem)</a> class that uses the specified equality comparer and creates a lookup dictionary when the specified threshold is exceeded.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public ConcurrentKeyedCollectionEx(
	Func<TItem, TKey> keySelector,
	IEqualityComparer<TKey> comparer,
	int dictionaryCreationThreshold
)
```


#### Parameters
&nbsp;<dl><dt>keySelector</dt><dd>Type: System.Func(<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">*TItem*</a>, <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">*TKey*</a>)<br />A function to extract a key from each element.</dd><dt>comparer</dt><dd>Type: System.Collections.Generic.IEqualityComparer(<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">*TKey*</a>)<br />The implementation of the IEqualityComparer(T) generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from Default</dd><dt>dictionaryCreationThreshold</dt><dd>Type: System.Int32<br />The number of elements the collection can hold without creating a lookup dictionary (0 creates the lookup dictionary when the first item is added), or –1 to specify that a lookup dictionary is never created.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>dictionaryCreationThreshold is less than –1.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">ConcurrentKeyedCollectionEx(TKey, TItem) Class</a><br /><a href="Overload_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2__ctor">ConcurrentKeyedCollectionEx(TKey, TItem) Overload</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
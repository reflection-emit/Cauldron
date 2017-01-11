# KeyedCollectionEx(*TKey*, *TItem*) Constructor (Func(*TItem*, *TKey*), IEqualityComparer(*TKey*))
 _**\[This is preliminary documentation and is subject to change.\]**_

Initializes a new instance of the <a href="T_Cauldron_Core_Collections_KeyedCollectionEx_2">KeyedCollectionEx(TKey, TItem)</a> class that uses the specified equality comparer and creates a lookup dictionary when the specified threshold is exceeded.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public KeyedCollectionEx(
	Func<TItem, TKey> keySelector,
	IEqualityComparer<TKey> comparer
)
```


#### Parameters
&nbsp;<dl><dt>keySelector</dt><dd>Type: System.Func(<a href="T_Cauldron_Core_Collections_KeyedCollectionEx_2">*TItem*</a>, <a href="T_Cauldron_Core_Collections_KeyedCollectionEx_2">*TKey*</a>)<br />A function to extract a key from each element.</dd><dt>comparer</dt><dd>Type: System.Collections.Generic.IEqualityComparer(<a href="T_Cauldron_Core_Collections_KeyedCollectionEx_2">*TKey*</a>)<br />The implementation of the IEqualityComparer(T) generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from Default</dd></dl>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_KeyedCollectionEx_2">KeyedCollectionEx(TKey, TItem) Class</a><br /><a href="Overload_Cauldron_Core_Collections_KeyedCollectionEx_2__ctor">KeyedCollectionEx(TKey, TItem) Overload</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
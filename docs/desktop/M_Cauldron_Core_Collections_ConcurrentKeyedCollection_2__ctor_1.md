# ConcurrentKeyedCollection(*TKey*, *TItem*) Constructor (IEqualityComparer(*TKey*))
 _**\[This is preliminary documentation and is subject to change.\]**_

Initializes a new instance of the <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">ConcurrentKeyedCollectionEx(TKey, TItem)</a> class that uses the specified equality comparer.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
protected ConcurrentKeyedCollection(
	IEqualityComparer<TKey> comparer
)
```


#### Parameters
&nbsp;<dl><dt>comparer</dt><dd>Type: System.Collections.Generic.IEqualityComparer(<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">*TKey*</a>)<br />The implementation of the IEqualityComparer(T) generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from Default.</dd></dl>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem) Class</a><br /><a href="Overload_Cauldron_Core_Collections_ConcurrentKeyedCollection_2__ctor">ConcurrentKeyedCollection(TKey, TItem) Overload</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
# ConcurrentKeyedCollectionEx(*TKey*, *TItem*).GetKeyForItem Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

When implemented in a derived class, extracts the key from the specified element.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
protected override TKey GetKeyForItem(
	TItem item
)
```


#### Parameters
&nbsp;<dl><dt>item</dt><dd>Type: <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">*TItem*</a><br />The element from which to extract the key.</dd></dl>

#### Return Value
Type: <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">*TKey*</a><br />The key for the specified element.

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2">ConcurrentKeyedCollectionEx(TKey, TItem) Class</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
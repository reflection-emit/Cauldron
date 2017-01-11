# ConcurrentList(*T*).Remove Method (Func(*T*, Boolean))
 _**\[This is preliminary documentation and is subject to change.\]**_

Removes the first occurrence of a specific object from the <a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T)</a>

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public IEnumerable<T> Remove(
	Func<T, bool> predicate
)
```


#### Parameters
&nbsp;<dl><dt>predicate</dt><dd>Type: System.Func(<a href="T_Cauldron_Core_Collections_ConcurrentList_1">*T*</a>, Boolean)<br />A function to test each element for a condition.</dd></dl>

#### Return Value
Type: IEnumerable(<a href="T_Cauldron_Core_Collections_ConcurrentList_1">*T*</a>)<br />A list of items that was removed from the collection

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="Overload_Cauldron_Core_Collections_ConcurrentList_1_Remove">Remove Overload</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
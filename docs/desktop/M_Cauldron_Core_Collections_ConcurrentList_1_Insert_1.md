# ConcurrentList(*T*).Insert Method (Int32, *T*)
 _**\[This is preliminary documentation and is subject to change.\]**_

Inserts an element into the <a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T)</a> at the specified index

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public void Insert(
	int index,
	T item
)
```


#### Parameters
&nbsp;<dl><dt>index</dt><dd>Type: System.Int32<br />The zero-based index at which item should be inserted.</dd><dt>item</dt><dd>Type: <a href="T_Cauldron_Core_Collections_ConcurrentList_1">*T*</a><br />The object to insert. The value can be null for reference types.</dd></dl>

#### Implements
IList(T).Insert(Int32, T)<br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>index is less than 0.-or-index is greater than <a href="P_Cauldron_Core_Collections_ConcurrentList_1_Count">Count</a>.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="Overload_Cauldron_Core_Collections_ConcurrentList_1_Insert">Insert Overload</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
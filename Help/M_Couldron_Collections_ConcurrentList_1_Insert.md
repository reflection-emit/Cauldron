# ConcurrentList(*T*).Insert Method (Int32, Object)
 _**\[This is preliminary documentation and is subject to change.\]**_

Inserts an element into the <a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T)</a> at the specified index

**Namespace:**&nbsp;<a href="N_Couldron_Collections">Couldron.Collections</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public void Insert(
	int index,
	Object value
)
```


#### Parameters
&nbsp;<dl><dt>index</dt><dd>Type: System.Int32<br />The zero-based index at which item should be inserted.</dd><dt>value</dt><dd>Type: System.Object<br />The object to insert. The value can be null for reference types.</dd></dl>

#### Implements
IList.Insert(Int32, Object)<br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>index is less than 0.-or-index is greater than <a href="P_Couldron_Collections_ConcurrentList_1_Count">Count</a>.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="Overload_Couldron_Collections_ConcurrentList_1_Insert">Insert Overload</a><br /><a href="N_Couldron_Collections">Couldron.Collections Namespace</a><br />
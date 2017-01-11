# ConcurrentList(*T*).RemoveAt Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Removes the element at the specified index of the <a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T)</a>

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public void RemoveAt(
	int index
)
```


#### Parameters
&nbsp;<dl><dt>index</dt><dd>Type: System.Int32<br />The zero-based index of the element to remove.</dd></dl>

#### Implements
IList(T).RemoveAt(Int32)<br />IList.RemoveAt(Int32)<br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>index is less than 0.-or-index is equal to or greater than <a href="P_Cauldron_Core_Collections_ConcurrentList_1_Count">Count</a>.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />
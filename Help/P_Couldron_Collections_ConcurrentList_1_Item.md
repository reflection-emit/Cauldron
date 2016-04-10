# ConcurrentList(*T*).Item Property 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets or sets the element at the specified index.

**Namespace:**&nbsp;<a href="N_Couldron_Collections">Couldron.Collections</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public T this[
	int index
] { get; }
```


#### Parameters
&nbsp;<dl><dt>index</dt><dd>Type: System.Int32<br />The zero-based index of the element to get or set.</dd></dl>

#### Return Value
Type: <a href="T_Couldron_Collections_ConcurrentList_1">*T*</a><br />The element at the specified index.

#### Implements
IReadOnlyList(T).Item(Int32)<br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>index is less than 0.-or-index is equal to or greater than <a href="P_Couldron_Collections_ConcurrentList_1_Count">Count</a>.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="N_Couldron_Collections">Couldron.Collections Namespace</a><br />
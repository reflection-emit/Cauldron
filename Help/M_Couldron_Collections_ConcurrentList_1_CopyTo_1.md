# ConcurrentList(*T*).CopyTo Method (*T*[], Int32)
 _**\[This is preliminary documentation and is subject to change.\]**_

Copies a range of elements from the <a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T)</a> to a compatible one-dimensional array, starting at the specified index of the target array.

**Namespace:**&nbsp;<a href="N_Couldron_Collections">Couldron.Collections</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public void CopyTo(
	T[] array,
	int arrayIndex
)
```


#### Parameters
&nbsp;<dl><dt>array</dt><dd>Type: <a href="T_Couldron_Collections_ConcurrentList_1">*T*</a>[]<br />The one-dimensional Array that is the destination of the elements copied from <a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T)</a>. The System.Array must have zero-based indexing.</dd><dt>arrayIndex</dt><dd>Type: System.Int32<br />The zero-based index in array at which copying begins.</dd></dl>

#### Implements
ICollection(T).CopyTo(T[], Int32)<br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>array is null.</td></tr><tr><td>ArgumentOutOfRangeException</td><td>arrayIndex is less than 0.</td></tr><tr><td>ArgumentException</td><td>The number of elements in the source <a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T)</a> is greater than the available space from arrayIndex to the end of the destination array.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="Overload_Couldron_Collections_ConcurrentList_1_CopyTo">CopyTo Overload</a><br /><a href="N_Couldron_Collections">Couldron.Collections Namespace</a><br />
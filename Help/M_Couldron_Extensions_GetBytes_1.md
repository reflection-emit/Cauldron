# Extensions.GetBytes Method (Byte[], Int32, Int32)
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets a specified length of bytes

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static byte[] GetBytes(
	this byte[] target,
	int startingPosition,
	int length
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Byte[]<br />The Array that contains the data to copy.</dd><dt>startingPosition</dt><dd>Type: System.Int32<br />A 32-bit integer that represents the index in the sourceArray at which copying begins.</dd><dt>length</dt><dd>Type: System.Int32<br />A 32-bit integer that represents the number of elements to copy.</dd></dl>

#### Return Value
Type: Byte[]<br />

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>Parameter *startingPosition* and *length* are out of range</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="Overload_Couldron_Extensions_GetBytes">GetBytes Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
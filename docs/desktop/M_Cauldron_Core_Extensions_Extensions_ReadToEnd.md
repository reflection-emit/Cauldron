# Extensions.ReadToEnd Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Reads all characters from the current position to the end of the stream.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string ReadToEnd(
	this Stream stream
)
```


#### Parameters
&nbsp;<dl><dt>stream</dt><dd>Type: System.IO.Stream<br />The stream to read</dd></dl>

#### Return Value
Type: String<br />The rest of the stream as a string, from the current position to the end. If the current position is at the end of the stream, returns an empty string ("").

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Stream. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>OutOfMemoryException</td><td>There is insufficient memory to allocate a buffer for the returned string.</td></tr><tr><td>IOException</td><td>An I/O error occurs.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
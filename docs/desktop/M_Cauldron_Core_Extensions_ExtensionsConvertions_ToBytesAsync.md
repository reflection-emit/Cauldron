# ExtensionsConvertions.ToBytesAsync Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts a Stream to Byte array. If the stream is not seekable, then this will use ReadToEndAsync() to get the stream.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<byte[]> ToBytesAsync(
	this Stream stream
)
```


#### Parameters
&nbsp;<dl><dt>stream</dt><dd>Type: System.IO.Stream<br />The Stream to convert</dd></dl>

#### Return Value
Type: Task(Byte[])<br />An array of bytes

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Stream. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>Parameter *stream* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
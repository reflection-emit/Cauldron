# ExtensionsFileIO.WriteBytesAsync Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Writes an array of bytes of data to the specified file.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task WriteBytesAsync(
	this FileInfo file,
	byte[] content
)
```


#### Parameters
&nbsp;<dl><dt>file</dt><dd>Type: System.IO.FileInfo<br />The file that the byte is written to.</dd><dt>content</dt><dd>Type: System.Byte[]<br />The array of bytes to write.</dd></dl>

#### Return Value
Type: Task<br />No object or value is returned when this method completes.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type FileInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsFileIO">ExtensionsFileIO Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
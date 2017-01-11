# ExtensionsFileIO.WriteTextAsync Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task WriteTextAsync(
	this FileInfo file,
	string content
)
```


#### Parameters
&nbsp;<dl><dt>file</dt><dd>Type: System.IO.FileInfo<br />The file to write to.</dd><dt>content</dt><dd>Type: System.String<br />The string to write to the file.</dd></dl>

#### Return Value
Type: Task<br />No object or value is returned when this method completes.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type FileInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsFileIO">ExtensionsFileIO Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
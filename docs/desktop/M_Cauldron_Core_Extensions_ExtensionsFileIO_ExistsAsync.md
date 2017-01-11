# ExtensionsFileIO.ExistsAsync Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Determines whether the specified file exists.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<bool> ExistsAsync(
	this DirectoryInfo folder,
	string filename
)
```


#### Parameters
&nbsp;<dl><dt>folder</dt><dd>Type: System.IO.DirectoryInfo<br />The folder where the file resides</dd><dt>filename</dt><dd>Type: System.String<br />The filename of the file to check.</dd></dl>

#### Return Value
Type: Task(Boolean)<br />When this method completes, it returns true if the file exists, otherwise false. If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns false regardless of the existence of path.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type DirectoryInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsFileIO">ExtensionsFileIO Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
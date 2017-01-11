# ExtensionsDirectoryInfo.CopyAsync Method (FileInfo, DirectoryInfo)
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates a copy of the file in the specified folder.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<FileInfo> CopyAsync(
	this FileInfo source,
	DirectoryInfo destinationFolder
)
```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.IO.FileInfo<br />The file to be copied</dd><dt>destinationFolder</dt><dd>Type: System.IO.DirectoryInfo<br />The destination folder where the copy of the file is created.</dd></dl>

#### Return Value
Type: Task(FileInfo)<br />When this method completes, it returns a FileInfo that represents the copy of the file created in the *destinationFolder*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type FileInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>IOException</td><td>If file already exists. Only on <a href="T_Cauldron_Core_NameCollisionOption">FailIfExists</a></td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsDirectoryInfo">ExtensionsDirectoryInfo Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsDirectoryInfo_CopyAsync">CopyAsync Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
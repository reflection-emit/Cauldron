# ExtensionsDirectoryInfo.CreateFolderAsync Method (DirectoryInfo, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates a new subfolder with the specified name in the current folder.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<DirectoryInfo> CreateFolderAsync(
	this DirectoryInfo directoryInfo,
	string desiredName
)
```


#### Parameters
&nbsp;<dl><dt>directoryInfo</dt><dd>Type: System.IO.DirectoryInfo<br />The DirectoryInfo representing the current folder</dd><dt>desiredName</dt><dd>Type: System.String<br />The name of the new subfolder to create in the current folder.</dd></dl>

#### Return Value
Type: Task(DirectoryInfo)<br />When this method completes, it returns a DirectoryInfo that represents the new subfolder.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type DirectoryInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsDirectoryInfo">ExtensionsDirectoryInfo Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsDirectoryInfo_CreateFolderAsync">CreateFolderAsync Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
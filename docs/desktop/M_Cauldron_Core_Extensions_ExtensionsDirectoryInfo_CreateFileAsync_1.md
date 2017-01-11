# ExtensionsDirectoryInfo.CreateFileAsync Method (DirectoryInfo, String, CreationCollisionOption)
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates a new file in the current folder. This method also specifies what to do if a file with the same name already exists in the current folder.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<FileInfo> CreateFileAsync(
	this DirectoryInfo directoryInfo,
	string desiredName,
	CreationCollisionOption options
)
```


#### Parameters
&nbsp;<dl><dt>directoryInfo</dt><dd>Type: System.IO.DirectoryInfo<br />The DirectoryInfo representing the current folder</dd><dt>desiredName</dt><dd>Type: System.String<br />The name of the new file to create in the current folder.</dd><dt>options</dt><dd>Type: <a href="T_Cauldron_Core_CreationCollisionOption">Cauldron.Core.CreationCollisionOption</a><br />One of the enumeration values that determines how to handle the collision if a file with the specified desiredName already exists in the current folder.</dd></dl>

#### Return Value
Type: Task(FileInfo)<br />When this method completes, it returns a FileInfo that represents the new file.>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type DirectoryInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*desiredName* is null</td></tr><tr><td>ArgumentException</td><td>*desiredName* is empty</td></tr><tr><td>IOException</td><td>If file already exists. Only on <a href="T_Cauldron_Core_CreationCollisionOption">FailIfExists</a></td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsDirectoryInfo">ExtensionsDirectoryInfo Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsDirectoryInfo_CreateFileAsync">CreateFileAsync Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
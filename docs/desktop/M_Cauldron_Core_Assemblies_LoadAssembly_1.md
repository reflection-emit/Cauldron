# Assemblies.LoadAssembly Method (FileInfo)
 _**\[This is preliminary documentation and is subject to change.\]**_

Loads the contents of an assembly file on the specified path.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void LoadAssembly(
	FileInfo fileInfo
)
```


#### Parameters
&nbsp;<dl><dt>fileInfo</dt><dd>Type: System.IO.FileInfo<br />The path of filename of the assembly</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>NotSupportedException</td><td>The *fileInfo* is a dynamic assembly.</td></tr><tr><td>ArgumentNullException</td><td>The *fileInfo* parameter is null.</td></tr><tr><td>FileLoadException</td><td>A file that was found could not be loaded</td></tr><tr><td>FileNotFoundException</td><td>The *fileInfo* does not exist</td></tr><tr><td>BadImageFormatException</td><td>*fileInfo* is not a valid assembly.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Assemblies">Assemblies Class</a><br /><a href="Overload_Cauldron_Core_Assemblies_LoadAssembly">LoadAssembly Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
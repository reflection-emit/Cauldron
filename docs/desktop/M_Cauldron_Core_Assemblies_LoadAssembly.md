# Assemblies.LoadAssembly Method (DirectoryInfo, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Loads the contents of all assemblies that matches the specified filter

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void LoadAssembly(
	DirectoryInfo directory,
	string filter = "*.dll"
)
```


#### Parameters
&nbsp;<dl><dt>directory</dt><dd>Type: System.IO.DirectoryInfo<br />The directory where the assemblies are located</dd><dt>filter (Optional)</dt><dd>Type: System.String<br />The search string to match against the names of files in <a href="P_Cauldron_Core_ApplicationInfo_ApplicationPath">ApplicationPath</a>. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>FileLoadException</td><td>A file that was found could not be loaded</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Assemblies">Assemblies Class</a><br /><a href="Overload_Cauldron_Core_Assemblies_LoadAssembly">LoadAssembly Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
# AssemblyUtil.LoadAssembly Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Loads the contents of an assembly file on the specified path.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void LoadAssembly(
	string path,
	bool relativPath = true
)
```


#### Parameters
&nbsp;<dl><dt>path</dt><dd>Type: System.String<br />The fully qualified path of the file to load.</dd><dt>relativPath (Optional)</dt><dd>Type: System.Boolean<br />A value that indicates if the parameter *path* is a relative path or not.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>The *path* argument is not an absolute path.</td></tr><tr><td>ArgumentNullException</td><td>The *path* parameter is null.</td></tr><tr><td>FileLoadException</td><td>A file that was found could not be loaded</td></tr><tr><td>FileNotFoundException</td><td>The *path* parameter is an empty string ("") or does not exist</td></tr><tr><td>BadImageFormatException</td><td>*path* is not a valid assembly.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_AssemblyUtil">AssemblyUtil Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
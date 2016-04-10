# AssemblyUtil.GetManifestResourceStream Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Loads the specified manifest resource from this assembly.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static Stream GetManifestResourceStream(
	string resourceInfoName
)
```


#### Parameters
&nbsp;<dl><dt>resourceInfoName</dt><dd>Type: System.String<br />The end of the fully qualified name of the embedded resource</dd></dl>

#### Return Value
Type: Stream<br />The manifest resource; or null if no resources were specified during compilation or if the resource is not visible to the caller.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The resourceInfoName parameter is null</td></tr><tr><td>ArgumentException</td><td>The resourceInfoName parameter is an empty string</td></tr><tr><td>FileLoadException</td><td>A file that was found could not be loaded.</td></tr><tr><td>FileNotFoundException</td><td>*resourceInfoName* was not found.</td></tr><tr><td>BadImageFormatException</td><td>*resourceInfoName* is not a valid assembly.</td></tr><tr><td>NotImplementedException</td><td>Resource length is greater than MaxValue</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_AssemblyUtil">AssemblyUtil Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
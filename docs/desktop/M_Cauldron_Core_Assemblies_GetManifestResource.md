# Assemblies.GetManifestResource Method (Func(AssemblyAndResourceNameInfo, Boolean))
 _**\[This is preliminary documentation and is subject to change.\]**_

Loads the specified manifest resource from this assembly.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static byte[] GetManifestResource(
	Func<AssemblyAndResourceNameInfo, bool> selector
)
```


#### Parameters
&nbsp;<dl><dt>selector</dt><dd>Type: System.Func(<a href="T_Cauldron_Core_AssemblyAndResourceNameInfo">AssemblyAndResourceNameInfo</a>, Boolean)<br /></dd></dl>

#### Return Value
Type: Byte[]<br />The manifest resource; or null if no resources were specified during compilation or if the resource is not visible to the caller.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The *selector* parameter is null</td></tr><tr><td>ArgumentException</td><td>The *selector* parameter is an empty string</td></tr><tr><td>FileLoadException</td><td>A file that was found could not be loaded.</td></tr><tr><td>NotImplementedException</td><td>Resource length is greater than MaxValue</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Assemblies">Assemblies Class</a><br /><a href="Overload_Cauldron_Core_Assemblies_GetManifestResource">GetManifestResource Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
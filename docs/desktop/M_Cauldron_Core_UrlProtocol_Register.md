# UrlProtocol.Register Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Registers the application to a URI scheme.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void Register(
	string name
)
```


#### Parameters
&nbsp;<dl><dt>name</dt><dd>Type: System.String<br />The application uri e.g. exampleApplication://</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>UnauthorizedAccessException</td><td>Process elevation required</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_UrlProtocol">UrlProtocol Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
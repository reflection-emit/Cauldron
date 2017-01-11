# WebAuthenticationBrokerWrapper.AuthenticateAsync Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Starts the asynchronous authentication operation. On Desktop the method has a timeout of 1 minute

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<string> AuthenticateAsync(
	Uri uri,
	Uri callbackUri
)
```


#### Parameters
&nbsp;<dl><dt>uri</dt><dd>Type: System.Uri<br />The starting URI of the web service. This URI must be a secure address of https://.</dd><dt>callbackUri</dt><dd>Type: System.Uri<br />The callback uri of the authentification. This will be used to verify the result value.</dd></dl>

#### Return Value
Type: Task(String)<br />Contains the protocol data when the operation successfully completes.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*uri* is null</td></tr><tr><td>ArgumentException</td><td>*uri* is not a secure address</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_WebAuthenticationBrokerWrapper">WebAuthenticationBrokerWrapper Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
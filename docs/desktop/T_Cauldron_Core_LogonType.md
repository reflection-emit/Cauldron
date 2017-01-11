# LogonType Enumeration
 _**\[This is preliminary documentation and is subject to change.\]**_

The type of logon operation to perform

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public enum LogonType
```


## Members
&nbsp;<table><tr><th></th><th>Member name</th><th>Value</th><th>Description</th></tr><tr><td /><td target="F:Cauldron.Core.LogonType.Interactive">**Interactive**</td><td>2</td><td>This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.</td></tr><tr><td /><td target="F:Cauldron.Core.LogonType.Network">**Network**</td><td>3</td><td>This logon type is intended for high performance servers to authenticate plaintext passwords. The LogonUser function does not cache credentials for this logon type.</td></tr><tr><td /><td target="F:Cauldron.Core.LogonType.Batch">**Batch**</td><td>4</td><td>This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention. This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or web servers.</td></tr><tr><td /><td target="F:Cauldron.Core.LogonType.Service">**Service**</td><td>5</td><td>Indicates a service-type logon. The account provided must have the service privilege enabled.</td></tr><tr><td /><td target="F:Cauldron.Core.LogonType.NetworkCleartext">**NetworkCleartext**</td><td>8</td><td>This logon type preserves the name and password in the authentication package, which allows the server to make connections to other network servers while impersonating the client. A server can accept plaintext credentials from a client, call LogonUser, verify that the user can access the system across the network, and still communicate with other servers.</td></tr><tr><td /><td target="F:Cauldron.Core.LogonType.NewCredentials">**NewCredentials**</td><td>9</td><td>This logon type allows the caller to clone its current token and specify new credentials for outbound connections. The new logon session has the same local identifier but uses different credentials for other network connections.</td></tr></table>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
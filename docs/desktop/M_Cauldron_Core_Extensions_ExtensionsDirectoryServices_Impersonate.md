# ExtensionsDirectoryServices.Impersonate Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Impersonates the given user

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static WindowsImpersonationContext Impersonate(
	this PrincipalContext principalContext,
	string username,
	string password,
	LogonType logonType
)
```


#### Parameters
&nbsp;<dl><dt>principalContext</dt><dd>Type: System.DirectoryServices.AccountManagement.PrincipalContext<br />The principal context of the user</dd><dt>username</dt><dd>Type: System.String<br />The user name of the user to impersonate</dd><dt>password</dt><dd>Type: System.String<br />The password of the user to impersonate</dd><dt>logonType</dt><dd>Type: <a href="T_Cauldron_Core_LogonType">Cauldron.Core.LogonType</a><br />The type of logon operation to perform.</dd></dl>

#### Return Value
Type: WindowsImpersonationContext<br />A WindowsImpersonationContext of the impersonation

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type PrincipalContext. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*username* is null</td></tr><tr><td>ArgumentNullException</td><td>*password* is null</td></tr><tr><td>ArgumentException</td><td>*username* is empty</td></tr><tr><td>ArgumentException</td><td>*password* is empty</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsDirectoryServices">ExtensionsDirectoryServices Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
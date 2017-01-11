# ExtensionsReflection.MatchesArgumentTypes Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns true if the argument types defined by *argumentTypes* matches with the argument types of *method*

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool MatchesArgumentTypes(
	this MethodBase method,
	Type[] argumentTypes
)
```


#### Parameters
&nbsp;<dl><dt>method</dt><dd>Type: System.Reflection.MethodBase<br />The method info which has to be compared to</dd><dt>argumentTypes</dt><dd>Type: System.Type[]<br />The argument types that has to match to</dd></dl>

#### Return Value
Type: Boolean<br />true if the argument types of *method* matches with the argument type defined by *argumentTypes*; otherwise, false.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type MethodBase. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
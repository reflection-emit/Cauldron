# ExtensionsReflection.GetDisplayName(*TEnum*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the name value of the <a href="T_Cauldron_Core_DisplayNameAttribute">DisplayNameAttribute</a> of the enum member

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string GetDisplayName<TEnum>(
	this TEnum enumValue
)
where TEnum : struct, new(), IConvertible

```


#### Parameters
&nbsp;<dl><dt>enumValue</dt><dd>Type: *TEnum*<br />The enum member value</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TEnum</dt><dd>The enum type</dd></dl>

#### Return Value
Type: String<br />The value of <a href="P_Cauldron_Core_DisplayNameAttribute_DisplayName">DisplayName</a>. Returns null if the enum member has no <a href="T_Cauldron_Core_DisplayNameAttribute">DisplayNameAttribute</a>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
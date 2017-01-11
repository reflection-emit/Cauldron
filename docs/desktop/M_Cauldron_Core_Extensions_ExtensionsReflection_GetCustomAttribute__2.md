# ExtensionsReflection.GetCustomAttribute(*TAttib*, *TEnum*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets the attribute of an enum member

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static TAttib GetCustomAttribute<TAttib, TEnum>(
	this TEnum enumValue
)
where TAttib : Attribute
where TEnum : struct, new(), IConvertible

```


#### Parameters
&nbsp;<dl><dt>enumValue</dt><dd>Type: *TEnum*<br />The enum member value</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TAttib</dt><dd>The attribute to retrieve</dd><dt>TEnum</dt><dd>The enum type</dd></dl>

#### Return Value
Type: *TAttib*<br />The custom attribute of the enum member.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>*enumValue* is not an enum</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />
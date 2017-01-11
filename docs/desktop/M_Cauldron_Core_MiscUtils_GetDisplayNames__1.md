# MiscUtils.GetDisplayNames(*TEnum*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the <a href="P_Cauldron_Core_DisplayNameAttribute_DisplayName">DisplayName</a> of an enum type.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IReadOnlyDictionary<TEnum, string> GetDisplayNames<TEnum>()
where TEnum : struct, new(), IConvertible

```


#### Type Parameters
&nbsp;<dl><dt>TEnum</dt><dd>The enum type</dd></dl>

#### Return Value
Type: IReadOnlyDictionary(*TEnum*, String)<br />A dictionary of display names with the enum value member as key

## See Also


#### Reference
<a href="T_Cauldron_Core_MiscUtils">MiscUtils Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
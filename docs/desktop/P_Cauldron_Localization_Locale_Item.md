# Locale.Item Property (Object)
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets the localized string with an object as a key 

 If the *key* is an enum the returned formatting will be: enum value - enum name 

Int64, Int32, UInt32 and UInt64 are formatted using {0:#,#}. 

Double, Single and Decimal are formatted using {0:#,#.00}. 

 Otherwise its tries to retrieve the localized string using the *key*'s type name as key.

**Namespace:**&nbsp;<a href="N_Cauldron_Localization">Cauldron.Localization</a><br />**Assembly:**&nbsp;Cauldron.Localization (in Cauldron.Localization.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public string this[
	Object key
] { get; }
```


#### Parameters
&nbsp;<dl><dt>key</dt><dd>Type: System.Object<br />The object used as key</dd></dl>

#### Return Value
Type: String<br />The localized string

## See Also


#### Reference
<a href="T_Cauldron_Localization_Locale">Locale Class</a><br /><a href="Overload_Cauldron_Localization_Locale_Item">Item Overload</a><br /><a href="N_Cauldron_Localization">Cauldron.Localization Namespace</a><br />
# StringFormatConverter.Convert Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts a value.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Object Convert(
	Object value,
	Type targetType,
	Object parameter,
	CultureInfo culture
)
```


#### Parameters
&nbsp;<dl><dt>value</dt><dd>Type: System.Object<br />The value produced by the binding source.</dd><dt>targetType</dt><dd>Type: System.Type<br />The type of the binding target property.</dd><dt>parameter</dt><dd>Type: System.Object<br />The converter parameter to use.</dd><dt>culture</dt><dd>Type: System.Globalization.CultureInfo<br />The culture to use in the converter.</dd></dl>

#### Return Value
Type: Object<br />A converted value.If the method returns null, the valid null value is used.

#### Implements
IValueConverter.Convert(Object, Type, Object, CultureInfo)<br />

## See Also


#### Reference
<a href="T_Couldron_StringFormatConverter">StringFormatConverter Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
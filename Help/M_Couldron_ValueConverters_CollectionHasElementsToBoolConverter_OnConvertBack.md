# CollectionHasElementsToBoolConverter.OnConvertBack Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts a value

**Namespace:**&nbsp;<a href="N_Couldron_ValueConverters">Couldron.ValueConverters</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
protected override IEnumerable OnConvertBack(
	bool value,
	Object parameter,
	CultureInfo culture
)
```


#### Parameters
&nbsp;<dl><dt>value</dt><dd>Type: System.Boolean<br />The value that is produced by the binding target.</dd><dt>parameter</dt><dd>Type: System.Object<br />The converter parameter to use.</dd><dt>culture</dt><dd>Type: System.Globalization.CultureInfo<br />The culture to use in the converter.</dd></dl>

#### Return Value
Type: IEnumerable<br />A converted value. If the method returns null, the valid null value is used.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>NotImplementedException</td><td>Always throws NotImplementedException. This method is not implemented.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_ValueConverters_CollectionHasElementsToBoolConverter">CollectionHasElementsToBoolConverter Class</a><br /><a href="N_Couldron_ValueConverters">Couldron.ValueConverters Namespace</a><br />
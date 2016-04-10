# CouldronTemplateSelector.SelectTemplate Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

When overridden in a derived class, returns a DataTemplate based on custom logic.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public override DataTemplate SelectTemplate(
	Object item,
	DependencyObject container
)
```


#### Parameters
&nbsp;<dl><dt>item</dt><dd>Type: System.Object<br />The data object for which to select the template.</dd><dt>container</dt><dd>Type: System.Windows.DependencyObject<br />The data-bound object.</dd></dl>

#### Return Value
Type: DataTemplate<br />Returns a DataTemplate or null. The default value is null.

## See Also


#### Reference
<a href="T_Couldron_CouldronTemplateSelector">CouldronTemplateSelector Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
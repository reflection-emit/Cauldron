# ValidatableViewModelBase.GetErrors Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets the validation errors for a specified property or for the entire entity.

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public IEnumerable GetErrors(
	string propertyName
)
```


#### Parameters
&nbsp;<dl><dt>propertyName</dt><dd>Type: System.String<br />The name of the property to retrieve validation errors for; or null or Empty, to retrieve entity-level errors.</dd></dl>

#### Return Value
Type: IEnumerable<br />The validation errors for the property or entity.

#### Implements
INotifyDataErrorInfo.GetErrors(String)<br />

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase Class</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />
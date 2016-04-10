# ValidatableViewModelBase.OnBeforeRaiseNotifyPropertyChanged Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Occured before the <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> event is invoked.

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
protected override bool OnBeforeRaiseNotifyPropertyChanged(
	string propertyName
)
```


#### Parameters
&nbsp;<dl><dt>propertyName</dt><dd>Type: System.String<br />The name of the property where the value change has occured</dd></dl>

#### Return Value
Type: Boolean<br />Returns true if <a href="M_Couldron_ViewModels_ViewModelBase_OnPropertyChanged">OnPropertyChanged(String)</a> should be cancelled. Otherwise false

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase Class</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />
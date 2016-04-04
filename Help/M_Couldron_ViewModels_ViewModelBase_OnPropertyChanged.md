# ViewModelBase.OnPropertyChanged Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Invokes the <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> event

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public void OnPropertyChanged(
	[CallerMemberNameAttribute] string propertyName = ""
)
```


#### Parameters
&nbsp;<dl><dt>propertyName (Optional)</dt><dd>Type: System.String<br />The name of the property where the value change has occured</dd></dl>

#### Implements
<a href="M_Couldron_ViewModels_IViewModel_OnPropertyChanged">IViewModel.OnPropertyChanged(String)</a><br />

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase Class</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />
# ContainerViewModelBase.Register Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Registers a child model to the current ViewModel

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Guid Register(
	IViewModel childViewModel
)
```


#### Parameters
&nbsp;<dl><dt>childViewModel</dt><dd>Type: <a href="T_Couldron_ViewModels_IViewModel">Couldron.ViewModels.IViewModel</a><br />The view model that requires registration</dd></dl>

#### Return Value
Type: Guid<br />The id of the viewmodel

#### Implements
<a href="M_Couldron_ViewModels_IContainerViewModel_Register">IContainerViewModel.Register(IViewModel)</a><br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The parameter *childViewModel* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ContainerViewModelBase">ContainerViewModelBase Class</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />
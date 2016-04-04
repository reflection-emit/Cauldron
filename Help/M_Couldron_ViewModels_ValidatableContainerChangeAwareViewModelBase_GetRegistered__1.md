# ValidatableContainerChangeAwareViewModelBase.GetRegistered(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns a registered Child ViewModel

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public T GetRegistered<T>()
where T : class, IViewModel

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the viewModel</dd></dl>

#### Return Value
Type: *T*<br />The viewModel otherwise null

#### Implements
<a href="M_Couldron_ViewModels_IContainerViewModel_GetRegistered__1">IContainerViewModel.GetRegistered(T)()</a><br />

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase">ValidatableContainerChangeAwareViewModelBase Class</a><br /><a href="Overload_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_GetRegistered">GetRegistered Overload</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />
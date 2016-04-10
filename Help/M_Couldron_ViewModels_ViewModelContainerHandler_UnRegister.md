# ViewModelContainerHandler.UnRegister Method (IViewModel)
 _**\[This is preliminary documentation and is subject to change.\]**_

Unregisters a registered viewmodel. This will also dispose the viewmodel.

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public void UnRegister(
	IViewModel childViewModel
)
```


#### Parameters
&nbsp;<dl><dt>childViewModel</dt><dd>Type: <a href="T_Couldron_ViewModels_IViewModel">Couldron.ViewModels.IViewModel</a><br />The viewmodel that requires unregistration</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The parameter *childViewModel* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ViewModelContainerHandler">ViewModelContainerHandler Class</a><br /><a href="Overload_Couldron_ViewModels_ViewModelContainerHandler_UnRegister">UnRegister Overload</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />
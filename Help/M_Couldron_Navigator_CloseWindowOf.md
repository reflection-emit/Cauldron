# Navigator.CloseWindowOf Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Closes the window to where the given viewmodel was directly assigned to.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static bool CloseWindowOf(
	IViewModel viewModel
)
```


#### Parameters
&nbsp;<dl><dt>viewModel</dt><dd>Type: <a href="T_Couldron_ViewModels_IViewModel">Couldron.ViewModels.IViewModel</a><br />The viewmodel to that was assigned to the window's data context</dd></dl>

#### Return Value
Type: Boolean<br />Returns true if Close() was triggered, otherwise false

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>Parameter *viewModel* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Navigator">Navigator Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
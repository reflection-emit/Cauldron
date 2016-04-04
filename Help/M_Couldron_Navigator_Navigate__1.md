# Navigator.Navigate(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Handles creation of a new Window and association of the viewmodel

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void Navigate<T>()
where T : IViewModel

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The viewModel type to create</dd></dl>

#### Return Value
Type: <br />An awaitable Task

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>Methodname specified in <a href="T_Couldron_ViewModels_NavigatingAttribute">NavigatingAttribute</a> does not exist</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Navigator">Navigator Class</a><br /><a href="Overload_Couldron_Navigator_Navigate">Navigate Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
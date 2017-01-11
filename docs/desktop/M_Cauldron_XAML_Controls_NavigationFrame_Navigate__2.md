# NavigationFrame.Navigate(*TViewModel*, *TResult*) Method (Object[], Func(*TResult*, Task))
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "M:Cauldron.XAML.Controls.NavigationFrame.Navigate``2(System.Object[],System.Func{``1,System.Threading.Tasks.Task})"\]

**Namespace:**&nbsp;<a href="N_Cauldron_XAML_Controls">Cauldron.XAML.Controls</a><br />**Assembly:**&nbsp;Cauldron.XAML (in Cauldron.XAML.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Task Navigate<TViewModel, TResult>(
	Object[] arguments,
	Func<TResult, Task> callback
)
where TViewModel : class, Object, IDialogViewModel<TResult>

```


#### Parameters
&nbsp;<dl><dt>arguments</dt><dd>Type: System.Object[]<br />\[Missing <param name="arguments"/> documentation for "M:Cauldron.XAML.Controls.NavigationFrame.Navigate``2(System.Object[],System.Func{``1,System.Threading.Tasks.Task})"\]</dd><dt>callback</dt><dd>Type: System.Func(*TResult*, Task)<br />\[Missing <param name="callback"/> documentation for "M:Cauldron.XAML.Controls.NavigationFrame.Navigate``2(System.Object[],System.Func{``1,System.Threading.Tasks.Task})"\]</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TViewModel</dt><dd>\[Missing <typeparam name="TViewModel"/> documentation for "M:Cauldron.XAML.Controls.NavigationFrame.Navigate``2(System.Object[],System.Func{``1,System.Threading.Tasks.Task})"\]</dd><dt>TResult</dt><dd>\[Missing <typeparam name="TResult"/> documentation for "M:Cauldron.XAML.Controls.NavigationFrame.Navigate``2(System.Object[],System.Func{``1,System.Threading.Tasks.Task})"\]</dd></dl>

#### Return Value
Type: Task<br />\[Missing <returns> documentation for "M:Cauldron.XAML.Controls.NavigationFrame.Navigate``2(System.Object[],System.Func{``1,System.Threading.Tasks.Task})"\]

## See Also


#### Reference
<a href="T_Cauldron_XAML_Controls_NavigationFrame">NavigationFrame Class</a><br /><a href="Overload_Cauldron_XAML_Controls_NavigationFrame_Navigate">Navigate Overload</a><br /><a href="N_Cauldron_XAML_Controls">Cauldron.XAML.Controls Namespace</a><br />
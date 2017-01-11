# Navigator.NavigateAsync(*T*, *TResult*) Method (Func(*TResult*, Task), Object[])
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "M:Cauldron.XAML.Navigation.Navigator.NavigateAsync``2(System.Func{``1,System.Threading.Tasks.Task},System.Object[])"\]

**Namespace:**&nbsp;<a href="N_Cauldron_XAML_Navigation">Cauldron.XAML.Navigation</a><br />**Assembly:**&nbsp;Cauldron.XAML (in Cauldron.XAML.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Task NavigateAsync<T, TResult>(
	Func<TResult, Task> callback,
	params Object[] parameters
)
where T : class, Object, IDialogViewModel<TResult>

```


#### Parameters
&nbsp;<dl><dt>callback</dt><dd>Type: System.Func(*TResult*, Task)<br />\[Missing <param name="callback"/> documentation for "M:Cauldron.XAML.Navigation.Navigator.NavigateAsync``2(System.Func{``1,System.Threading.Tasks.Task},System.Object[])"\]</dd><dt>parameters</dt><dd>Type: System.Object[]<br />\[Missing <param name="parameters"/> documentation for "M:Cauldron.XAML.Navigation.Navigator.NavigateAsync``2(System.Func{``1,System.Threading.Tasks.Task},System.Object[])"\]</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>\[Missing <typeparam name="T"/> documentation for "M:Cauldron.XAML.Navigation.Navigator.NavigateAsync``2(System.Func{``1,System.Threading.Tasks.Task},System.Object[])"\]</dd><dt>TResult</dt><dd>\[Missing <typeparam name="TResult"/> documentation for "M:Cauldron.XAML.Navigation.Navigator.NavigateAsync``2(System.Func{``1,System.Threading.Tasks.Task},System.Object[])"\]</dd></dl>

#### Return Value
Type: Task<br />\[Missing <returns> documentation for "M:Cauldron.XAML.Navigation.Navigator.NavigateAsync``2(System.Func{``1,System.Threading.Tasks.Task},System.Object[])"\]

#### Implements
<a href="M_Cauldron_XAML_Navigation_INavigator_NavigateAsync__2_1">INavigator.NavigateAsync(T, TResult)(Func(TResult, Task), Object[])</a><br />

## See Also


#### Reference
<a href="T_Cauldron_XAML_Navigation_Navigator">Navigator Class</a><br /><a href="Overload_Cauldron_XAML_Navigation_Navigator_NavigateAsync">NavigateAsync Overload</a><br /><a href="N_Cauldron_XAML_Navigation">Cauldron.XAML.Navigation Namespace</a><br />
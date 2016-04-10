# MessageDialog.ShowAsync Method (String, String, MessageBoxImage, MessageDialogCommand, MessageDialogCommand)
 _**\[This is preliminary documentation and is subject to change.\]**_

Begins an asynchronous operation showing a dialog.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static Task ShowAsync(
	string title,
	string content,
	MessageBoxImage messageBoxImage,
	MessageDialogCommand command1,
	MessageDialogCommand command2
)
```


#### Parameters
&nbsp;<dl><dt>title</dt><dd>Type: System.String<br />The title to display on the dialog, if any.</dd><dt>content</dt><dd>Type: System.String<br />The message to be displayed to the user.</dd><dt>messageBoxImage</dt><dd>Type: <a href="T_Couldron_Core_MessageBoxImage">Couldron.Core.MessageBoxImage</a><br />The icon to show on the dialog</dd><dt>command1</dt><dd>Type: <a href="T_Couldron_Core_MessageDialogCommand">Couldron.Core.MessageDialogCommand</a><br />A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</dd><dt>command2</dt><dd>Type: <a href="T_Couldron_Core_MessageDialogCommand">Couldron.Core.MessageDialogCommand</a><br />A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</dd></dl>

#### Return Value
Type: Task<br />An object that represents the asynchronous operation.

## See Also


#### Reference
<a href="T_Couldron_MessageDialog">MessageDialog Class</a><br /><a href="Overload_Couldron_MessageDialog_ShowAsync">ShowAsync Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
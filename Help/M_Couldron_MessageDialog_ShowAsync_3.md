# MessageDialog.ShowAsync Method (String, String, UInt32, UInt32, MessageDialogCommandList)
 _**\[This is preliminary documentation and is subject to change.\]**_

Begins an asynchronous operation showing a dialog.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static Task ShowAsync(
	string title,
	string content,
	uint defaultCommandIndex,
	uint cancelCommandIndex,
	MessageDialogCommandList commands
)
```


#### Parameters
&nbsp;<dl><dt>title</dt><dd>Type: System.String<br />The title to display on the dialog, if any.</dd><dt>content</dt><dd>Type: System.String<br />The message to be displayed to the user.</dd><dt>defaultCommandIndex</dt><dd>Type: System.UInt32<br />The index of the command you want to use as the default. This is the command that fires by default when users press the ENTER key.</dd><dt>cancelCommandIndex</dt><dd>Type: System.UInt32<br />The index of the command you want to use as the cancel command. This is the command that fires when users press the ESC key.</dd><dt>commands</dt><dd>Type: <a href="T_Couldron_Core_MessageDialogCommandList">Couldron.Core.MessageDialogCommandList</a><br />An array of commands that appear in the command bar of the message dialog. These commands makes the dialog actionable.</dd></dl>

#### Return Value
Type: Task<br />An object that represents the asynchronous operation.

## See Also


#### Reference
<a href="T_Couldron_MessageDialog">MessageDialog Class</a><br /><a href="Overload_Couldron_MessageDialog_ShowAsync">ShowAsync Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
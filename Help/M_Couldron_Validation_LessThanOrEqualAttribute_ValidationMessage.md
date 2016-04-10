# LessThanOrEqualAttribute.ValidationMessage Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Occures on validation 

 Can be used to modify the validation error message.

**Namespace:**&nbsp;<a href="N_Couldron_Validation">Couldron.Validation</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
protected override string ValidationMessage(
	string errorMessage,
	IValidatableViewModel context
)
```


#### Parameters
&nbsp;<dl><dt>errorMessage</dt><dd>Type: System.String<br />The validation error message</dd><dt>context</dt><dd>Type: <a href="T_Couldron_ViewModels_IValidatableViewModel">Couldron.ViewModels.IValidatableViewModel</a><br />The Viewmodel context that has to be validated</dd></dl>

#### Return Value
Type: String<br />A modified validation error message

## See Also


#### Reference
<a href="T_Couldron_Validation_LessThanOrEqualAttribute">LessThanOrEqualAttribute Class</a><br /><a href="N_Couldron_Validation">Couldron.Validation Namespace</a><br />
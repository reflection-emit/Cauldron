# Couldron.Validation Namespace
 _**\[This is preliminary documentation and is subject to change.\]**_

## Classes
&nbsp;<table><tr><th></th><th>Class</th><th>Description</th></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_EqualityAttribute">EqualityAttribute</a></td><td>
Validates if two properties are equal in value 

 Causes a validation error if the values of both properties are not equal</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_GreaterThanAttribute">GreaterThanAttribute</a></td><td>
Validates the property if value is greater than the given value or the given property</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_IsMandatoryAttribute">IsMandatoryAttribute</a></td><td>
Validates a property for mandatory value. 

 Value is null or empty will cause a validation error</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_PasswordStrengthAttribute">PasswordStrengthAttribute</a></td><td>
Validates the password strength the property. 

<a href="T_Couldron_Core_PasswordScore">Blank</a>, <a href="T_Couldron_Core_PasswordScore">VeryWeak</a> and <a href="T_Couldron_Core_PasswordScore">Weak</a> will cause a validation error 

 ATTENTION: Using this Attribute against a SecureString may jeopardize security</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_StringLengthAttribute">StringLengthAttribute</a></td><td>
Validates a property for the string length. 

 A String that is shorter or longer than the specified length will cause a validation error</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_SuppressIsChangedAttribute">SuppressIsChangedAttribute</a></td><td>
Specifies that the property change will not affect the model's change flag</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Validation_ValidationBaseAttribute">ValidationBaseAttribute</a></td><td>
Specifies the validation method for a property</td></tr></table>&nbsp;

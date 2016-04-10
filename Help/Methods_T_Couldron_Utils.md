# Utils Methods
 _**\[This is preliminary documentation and is subject to change.\]**_

The <a href="T_Couldron_Utils">Utils</a> type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_Equals">Equals</a></td><td>
Determines whether *a* is equal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the == operator, then with reflection on 'op_Equality' and as last resort uses Equals(Object, Object) to determine equality</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GetMousePosition">GetMousePosition</a></td><td>
Returns the mouse position on screen</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GetPasswordScore">GetPasswordScore</a></td><td>
Checks the password's strength</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GetStringFromModule">GetStringFromModule(String)</a></td><td>
Gets a (assorted) string from dll text resources.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GetStringFromModule_2">GetStringFromModule(UInt32)</a></td><td>
Gets a string from user32.dll text resources.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GetStringFromModule_1">GetStringFromModule(String, UInt32)</a></td><td>
Gets a string from the dll defined by *moduleName* text resources.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GreaterThan">GreaterThan</a></td><td>
Determines whether *a* is greater than *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the > operator, then with reflection on 'op_GreaterThan'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_GreaterThanOrEqual">GreaterThanOrEqual</a></td><td>
Determines whether *a* is greater than or equal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the >= operator, then with reflection on 'op_GreaterThanOrEqual'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_LessThan">LessThan</a></td><td>
Determines whether *a* is less than *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the < operator, then with reflection on 'op_LessThan'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_LessThanOrEqual">LessThanOrEqual</a></td><td>
Determines whether *a* is less than or equal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the <= operator, then with reflection on 'op_LessThanOrEqual'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_SendMessage">SendMessage</a></td><td>
Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_UnEquals">UnEquals</a></td><td>
Determines whether *a* is unequal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the != operator, then with reflection on 'op_Inequality' and as last resort uses Equals(Object, Object) to determine unequality</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Utils_WmGetMinMaxInfo">WmGetMinMaxInfo</a></td><td>
Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.</td></tr></table>&nbsp;
<a href="#utils-methods">Back to Top</a>

## See Also


#### Reference
<a href="T_Couldron_Utils">Utils Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />
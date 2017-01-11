# ComparerUtils.UnEquals Method 
 _**\[This is preliminary documentation and is subject to change.\]**_


## Overload List
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_UnEquals">UnEquals(Object, Object)</a></td><td>
Determines whether *a* is unequal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the != operator, then with reflection on 'op_Inequality' and as last resort uses Equals(Object, Object) to determine unequality</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_UnEquals__2">UnEquals(T, TValue)(T, T, Func(T, TValue))</a></td><td>
Determines whether *a* is unequal to *b*</td></tr></table>&nbsp;
<a href="#comparerutils.unequals-method">Back to Top</a>

## See Also


#### Reference
<a href="T_Cauldron_Core_ComparerUtils">ComparerUtils Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
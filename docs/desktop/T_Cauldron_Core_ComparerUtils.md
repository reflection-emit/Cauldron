# ComparerUtils Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides methods for comparing objects


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.ComparerUtils<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static class ComparerUtils
```

The ComparerUtils type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_Equals">Equals(Object, Object)</a></td><td>
Determines whether *a* is equal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the == operator, then with reflection on 'op_Equality' and as last resort uses Equals(Object, Object) to determine equality</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_Equals__2">Equals(T, TValue)(T, T, Func(T, TValue))</a></td><td>
Determines whether *a* is equal to *b*</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_GreaterThan">GreaterThan</a></td><td>
Determines whether *a* is greater than *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the > operator, then with reflection on 'op_GreaterThan'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_GreaterThanOrEqual">GreaterThanOrEqual</a></td><td>
Determines whether *a* is greater than or equal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the >= operator, then with reflection on 'op_GreaterThanOrEqual'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_LessThan">LessThan</a></td><td>
Determines whether *a* is less than *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the < operator, then with reflection on 'op_LessThan'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_LessThanOrEqual">LessThanOrEqual</a></td><td>
Determines whether *a* is less than or equal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the <= operator, then with reflection on 'op_LessThanOrEqual'</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_UnEquals">UnEquals(Object, Object)</a></td><td>
Determines whether *a* is unequal to *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the != operator, then with reflection on 'op_Inequality' and as last resort uses Equals(Object, Object) to determine unequality</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_ComparerUtils_UnEquals__2">UnEquals(T, TValue)(T, T, Func(T, TValue))</a></td><td>
Determines whether *a* is unequal to *b*</td></tr></table>&nbsp;
<a href="#comparerutils-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
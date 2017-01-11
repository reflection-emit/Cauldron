# Mathc Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides static methods for common mathematical functions.


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.Mathc<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static class Mathc
```

The Mathc type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_Add">Add</a></td><td>
Adds *a* to *b*

 If *a* and *b* are null then null is returned. If *a* is null then *b* is returned. If *b* is null then *a* is returned. 

 Tries to cast primitiv Type and use the + operator. If the Type is unknown then reflection is used to determin the operator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_Clamp">Clamp(Double, Double, Double)</a></td><td>
Clamps a value between a minimum and maximum value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_Clamp_1">Clamp(Int32, Int32, Int32)</a></td><td>
Clamps a value between a minimum and maximum value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_DegreesToRadians">DegreesToRadians</a></td><td>
Converts degrees to radians</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_Divide">Divide</a></td><td>
Divides *a* with *b*

 If *a* and *b* are null then null is returned. If *a* is null then 0 is returned. If *b* is null then *a* is returned; 

 Tries to cast primitiv Type and use the / operator. If the Type is unknown then reflection is used to determin the operator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_Multiply">Multiply</a></td><td>
Multiplies *a* with *b*

 If *a* and *b* are null then null is returned. If *a* is null then *b* is returned. If *b* is null then *a* is returned. 

 Tries to cast primitiv Type and use the * operator. If the Type is unknown then reflection is used to determin the operator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_RadiansToDegrees">RadiansToDegrees</a></td><td>
Converts radians to degrees</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_Substract">Substract</a></td><td>
Substracts *b* from *a*

 If *a* and *b* are null then null is returned. If *a* is null then 0 is returned. If *b* is null then *a* is returned; 

 Tries to cast primitiv Type and use the - operator. If the Type is unknown then reflection is used to determin the operator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_ValueOf">ValueOf(Double, Double, Double)</a></td><td>
Calculates the percentage relationship of two values</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Mathc_ValueOf_1">ValueOf(Double, Double, Double, Double, Double)</a></td><td>
Calculates the percentage relationship of two values</td></tr></table>&nbsp;
<a href="#mathc-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />
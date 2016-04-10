# Couldron.ValueConverters Namespace
 _**\[This is preliminary documentation and is subject to change.\]**_

## Classes
&nbsp;<table><tr><th></th><th>Class</th><th>Description</th></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_BooleanInvertConverter">BooleanInvertConverter</a></td><td>
Inverts a bool value</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_BooleanToVisibilityConverter">BooleanToVisibilityConverter</a></td><td>
Converts a Boolean to Visibility. If the value is true, the IValueConverter will return either Collapsed or Visible depending on the parameter</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_CollectionHasElementsToBoolConverter">CollectionHasElementsToBoolConverter</a></td><td>
Checks a collection if it has child elements and return true or false depending on the converter parameters 

 Default is return false if the collection has no elements</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_KeyToLocalizedStringConverter">KeyToLocalizedStringConverter</a></td><td>
Tries to get the localized value of the given key</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_NullOrEmptyTovisibilityConverter">NullOrEmptyTovisibilityConverter</a></td><td>
Checks if a string is null or empty. If the string is null or empty, the IValueConverter will return either Collapsed or Visible depending on the parameter</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_ObjectToBooleanConverter">ObjectToBooleanConverter</a></td><td>
Checks if an object is null. If the object is null, the IValueConverter will return either true or false depending on the parameter 

 If the parameter is True then the converter will return true if the object is null, otherwise false</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_ObjectToVisibilityConverter">ObjectToVisibilityConverter</a></td><td>
Checks if an object is null. If the object is null, the IValueConverter will return Collapsed</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_StringFormatConverter">StringFormatConverter</a></td><td>
Converts the value of objects to strings based on the formats specified and inserts them into another string.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ValueConverters_ValueConverterBase_2">ValueConverterBase(TInput, TOutput)</a></td><td>
Provides a base class for Binding value converters.</td></tr></table>&nbsp;

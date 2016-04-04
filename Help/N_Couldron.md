# Couldron Namespace
 _**\[This is preliminary documentation and is subject to change.\]**_

## Classes
&nbsp;<table><tr><th></th><th>Class</th><th>Description</th></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_AssemblyAndResourceNameInfo">AssemblyAndResourceNameInfo</a></td><td>
Represents a resource info of an embedded resource with its corresponding <a href="P_Couldron_AssemblyAndResourceNameInfo_Assembly">Assembly</a></td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_AssemblyUtil">AssemblyUtil</a></td><td>
Contains utilities that helps to manage and gather Assembly information</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_BehaviourInvokationArgs">BehaviourInvokationArgs</a></td><td>
Provides data for the <a href="E_Couldron_INotifyBehaviourInvokation_BehaviourInvoke">BehaviourInvoke</a> event.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_BooleanInvertConverter">BooleanInvertConverter</a></td><td>
Inverts a bool value</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_BooleanToVisibilityConverter">BooleanToVisibilityConverter</a></td><td>
Converts a Boolean to Visibility. If the value is true, the IValueConverter will return either Collapsed or Visible depending on the parameter</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_CollectionHasElementsToBoolConverter">CollectionHasElementsToBoolConverter</a></td><td>
Checks a collection if it has child elements and return true or false depending on the converter parameters 

 Default is return false if the collection has no elements</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_CouldronApplication">CouldronApplication</a></td><td /></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_CouldronTemplateSelector">CouldronTemplateSelector</a></td><td>
Provides a way to choose a DataTemplate based on the data object and the data-bound element.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ErrorCollectionConverter">ErrorCollectionConverter</a></td><td>
Converts a collection of Errors from INotifyDataErrorInfo to a readable string</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ErrorCollectionCountToVisibilityConverter">ErrorCollectionCountToVisibilityConverter</a></td><td>
Converts a collection of Errors from INotifyDataErrorInfo count to Visibility</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a></td><td>
Provides usefull extension methods</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Extensions">Extensions</a></td><td>
Provides usefull extension methods</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ExtensionsFrameworkElement">ExtensionsFrameworkElement</a></td><td>
Provides usefull extension methods extending FrameworkElement</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Factory">Factory</a></td><td>
Provides methods for creating and destroying object instances</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_FactoryAttribute">FactoryAttribute</a></td><td>
Specifies that Type provide a particular export</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_InjectAttribute">InjectAttribute</a></td><td>
Specifies that the property, field or constructor contains a type or parameter that can be supplied by the <a href="T_Couldron_Factory">Factory</a></td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_InjectionFactoryExtension">InjectionFactoryExtension</a></td><td>
Adds injection functionality to the <a href="T_Couldron_Factory">Factory</a></td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_KeyToLocalizedStringConverter">KeyToLocalizedStringConverter</a></td><td>
Tries to get the localized value of the given key</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Localization">Localization</a></td><td>
Provides methods regarding localization</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_MathematicalOperationConverter">MathematicalOperationConverter</a></td><td>
Perform mathematical operation (substraction and addition) on value</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Navigator">Navigator</a></td><td>
Handles creation of a new Window and association of the viewmodel</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_NullOrEmptyTovisibilityConverter">NullOrEmptyTovisibilityConverter</a></td><td>
Checks if a string is null or empty. If the string is null or empty, the IValueConverter will return either Collapsed or Visible depending on the parameter</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ObjectToBooleanConverter">ObjectToBooleanConverter</a></td><td>
Checks if an object is null. If the object is null, the IValueConverter will return return either true or false depending on the parameter 

 If the parameter is True then the converter will return true if the object is null, otherwise false</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ObjectToVisibilityConverter">ObjectToVisibilityConverter</a></td><td>
Checks if an object is null. If the object is null, the IValueConverter will return Collapsed</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_RelayCommand">RelayCommand</a></td><td>
Implements the <a href="T_Couldron_IRelayCommand">IRelayCommand</a> interface</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_RelayCommand_1">RelayCommand(T)</a></td><td>
Implements the <a href="T_Couldron_IRelayCommand">IRelayCommand</a> interface 

<a href="T_Couldron_RelayCommand_1">RelayCommand(T)</a> will pass the EventArgs from the control's event to the action delegate</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_StringFormatConverter">StringFormatConverter</a></td><td>
Converts the value of objects to strings based on the formats specified and inserts them into another string.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Utils">Utils</a></td><td>
Provides a collection of utility methods</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_ViewAttribute">ViewAttribute</a></td><td>
Specifies a view for a the viewmodel</td></tr></table>

## Interfaces
&nbsp;<table><tr><th></th><th>Interface</th><th>Description</th></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Couldron_IFactoryExtension">IFactoryExtension</a></td><td>
Represents an interface for the <a href="T_Couldron_Factory">Factory</a> extension</td></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Couldron_ILocalizationSource">ILocalizationSource</a></td><td>
Represents a source for a localization source</td></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Couldron_INotifyBehaviourInvokation">INotifyBehaviourInvokation</a></td><td>
Notifies the client that a behaviour should be invoked</td></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Couldron_IRelayCommand">IRelayCommand</a></td><td>
Defines a command</td></tr></table>

## Enumerations
&nbsp;<table><tr><th></th><th>Enumeration</th><th>Description</th></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Couldron_FactoryCreationPolicy">FactoryCreationPolicy</a></td><td>
Describes the creation policy of an object through the <a href="T_Couldron_Factory">Factory</a></td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Couldron_WindowsMessages">WindowsMessages</a></td><td>
Windows Messages. 

 Defined in winuser.h from Windows SDK v6.1 

 Documentation pulled from MSDN.</td></tr></table>&nbsp;

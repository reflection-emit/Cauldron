# Locale.Item Property 
 _**\[This is preliminary documentation and is subject to change.\]**_


## Overload List
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Localization_Locale_Item">Item(Object)</a></td><td>
Gets the localized string with an object as a key 

 If the *key* is an enum the returned formatting will be: enum value - enum name 

Int64, Int32, UInt32 and UInt64 are formatted using {0:#,#}. 

Double, Single and Decimal are formatted using {0:#,#.00}. 

 Otherwise its tries to retrieve the localized string using the *key*'s type name as key.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Localization_Locale_Item_1">Item(String)</a></td><td>
Gets the localized string with the specified key 

 Returns null if the key was not found</td></tr></table>&nbsp;
<a href="#locale.item-property">Back to Top</a>

## See Also


#### Reference
<a href="T_Cauldron_Localization_Locale">Locale Class</a><br /><a href="N_Cauldron_Localization">Cauldron.Localization Namespace</a><br />
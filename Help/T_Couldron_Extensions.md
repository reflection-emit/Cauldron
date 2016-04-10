# Extensions Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides usefull extension methods

Provides usefull extension methods


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Couldron.Extensions<br />
**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static class Extensions
```

The Extensions type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Any">Any(IEnumerable)</a></td><td>
Determines whether a sequence contains any elements.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Any_1">Any(IEnumerable, Func(Object, Boolean))</a></td><td>
Determines whether any element of a sequence satisfies a condition.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_CastTo__1">CastTo(T)</a></td><td>
Performs certain types of conversions between compatible reference types or nullable types 

 Returns null if convertion was not successfull</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ChangeAlpha">ChangeAlpha</a></td><td>
Copy and modifies the alpha channel of the SolidColorBrush's Color</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Contains__1">Contains(T)</a></td><td>
Determines whether an element is in the array</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Count">Count</a></td><td>
Gets the number of elements contained in the IEnumerable</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_DisposeAll">DisposeAll</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface. 

 If the object is a FrameworkElement it will try to find known diposable attached properties. 

 It will also dispose the the DataContext content.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ElementAt">ElementAt</a></td><td>
Returns the element at the defined index</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_EnclosedIn">EnclosedIn</a></td><td>
Returns the string enclosed between two defined chars</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_FirstElement">FirstElement</a></td><td>
Returns the first element of a sequence.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetBytes">GetBytes(Byte[], Int32)</a></td><td>
Gets a specified length of bytes. 

 If the specified length *length* is longer than the source array the source array will be returned instead.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetBytes_1">GetBytes(Byte[], Int32, Int32)</a></td><td>
Gets a specified length of bytes</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetMD5HashString">GetMD5HashString</a></td><td>
Hashes a string with MD5</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetMethod">GetMethod</a></td><td>
Gets the methods of the current Type</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetSha256HashBytes">GetSha256HashBytes</a></td><td>
Hashes a string with Sha256</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetSha256HashString">GetSha256HashString</a></td><td>
Hashes a string with Sha256</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetTarget__1">GetTarget(T)</a></td><td>
Retrieves the target object referenced by the current WeakReference(T) object 

 Returns null if the target is not available</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_GetWindowHandle">GetWindowHandle</a></td><td>
Gets the window handle for a Windows Presentation Foundation (WPF) window</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ImplementsInterface__1">ImplementsInterface(T)(TypeInfo)</a></td><td>
Checks if the type has implemented the defined interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ImplementsInterface__1_1">ImplementsInterface(T)(Type)</a></td><td>
Checks if the type has implemented the defined interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_IndexOf">IndexOf(Byte[], Byte[])</a></td><td>
Searches for the specified byte array and returns the zero-based index of the first occurrence within the entire Array</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_IndexOf__1">IndexOf(T)(T[], T)</a></td><td>
Searches for the specified object and returns the zero-based index of the first occurrence within the entire Array</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_IsNotNull__1">IsNotNull(T)</a></td><td>
Checks if the value is null. If not, it will invoke *action*</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_IsNullable">IsNullable</a></td><td>
Gets a value indicating whether the current type is a Nullable(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Left">Left</a></td><td>
Returns a string containing a specified number of characters from the left side of a string.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Move__1">Move(T)</a></td><td>
Moves the specified item to a new location in the collection</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_RandomizeValues">RandomizeValues</a></td><td>
Replaces the values of data in memory with random values. The GC handle will be freed.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ReadToEnd">ReadToEnd</a></td><td>
Reads all characters from the Begin to the end of the stream</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Remove">Remove</a></td><td>
Removes the first occurrence of a specific object from the IEnumerable</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_Right">Right</a></td><td>
Returns a string containing a specified number of characters from the right side of a string.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ToArray__1">ToArray(T)</a></td><td>
Converts a IEnumerable to an array</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ToBase64String">ToBase64String</a></td><td>
Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64 digits.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ToBitmapImage">ToBitmapImage</a></td><td>
Creates a new instance of BitmapImage and assigns the Stream to its StreamSource property 

 Returns null if *stream* is null.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ToBytesAsync">ToBytesAsync</a></td><td>
Converts a Stream to Byte array</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ToInteger">ToInteger</a></td><td>
Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_Extensions_ToSecureString">ToSecureString</a></td><td>
Converts a string to a SecureString</td></tr></table>&nbsp;
<a href="#extensions-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron">Couldron Namespace</a><br />
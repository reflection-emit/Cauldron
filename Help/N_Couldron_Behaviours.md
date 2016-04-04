# Couldron.Behaviours Namespace
 _**\[This is preliminary documentation and is subject to change.\]**_

## Classes
&nbsp;<table><tr><th></th><th>Class</th><th>Description</th></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_ActionBase">ActionBase</a></td><td>
Represents a base class for action behaviours</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_Behaviour_1">Behaviour(T)</a></td><td>
A base class for behaviours</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_BehaviourInvokeAwareBehaviourBase_1">BehaviourInvokeAwareBehaviourBase(T)</a></td><td>
A base class for behaviour invoke aware behaviours</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_BehaviourUsageAttribute">BehaviourUsageAttribute</a></td><td>
Specifies that a behaviour can only be applied once</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_ControlTemplateBinding">ControlTemplateBinding</a></td><td /></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_ControlTemplateCommandFromControlNameBinding">ControlTemplateCommandFromControlNameBinding</a></td><td>
Provides a behaviour that allows Buttons in ControlTemplate to bind to a Command that differs on every control. 

 Usecase example: 

 Two TextBoxex with ControlTemplates. Both TextBoxes shares the ControlTemplate. A Button resides in the ControlTemplate. The Button on each TextBox must invoke two different ICommands.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_EnterKeyToCommand">EnterKeyToCommand</a></td><td>
Provides a behaviour that can invoke a command when the Enter key is pressed</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_EventToCommand">EventToCommand</a></td><td>
Provides a behaviour that can handle events and invokes a binded command</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_EventTrigger">EventTrigger</a></td><td>
Provides a Behaviour that can invoke <a href="T_Couldron_Behaviours_ActionBase">ActionBase</a> behaviours. 

 The <a href="T_Couldron_Behaviours_EventTrigger">EventTrigger</a> is triggered by an event of the associated FrameworkElement</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_Interaction">Interaction</a></td><td>
Defines a <a href="T_Couldron_Collections_BehaviourCollection">BehaviourCollection</a> attached property</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")![Code example](media/CodeExample.png "Code example")</td><td><a href="T_Couldron_Behaviours_Interactivity">Interactivity</a></td><td>
Defines a container that enables to attach behaviours to a Style</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_InvokeMethodAction">InvokeMethodAction</a></td><td>
Represents an action, that can invoke a method (also with parameters) residing on the the <a href="P_Couldron_Behaviours_Behaviour_1_AssociatedObject">AssociatedObject</a> or in the control defined by <a href="P_Couldron_Behaviours_InvokeMethodAction_MethodOwnerType">MethodOwnerType</a>. The method parameters must be properties of the <a href="P_Couldron_Behaviours_Behaviour_1_AssociatedObject">AssociatedObject</a> or it's templatedparent (Not available in UWP)</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_OpenFileDialogAction">OpenFileDialogAction</a></td><td>
Represents an action that opens a OpenFileDialog</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_SetFocus">SetFocus</a></td><td>
Provides a behaviour that can set the focus of a control after Loaded</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_TextBoxInputFilter">TextBoxInputFilter</a></td><td /></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Couldron_Behaviours_WindowConfiguration">WindowConfiguration</a></td><td>
Provides a behaviour to configure the window that will contain the view</td></tr></table>

## Interfaces
&nbsp;<table><tr><th></th><th>Interface</th><th>Description</th></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Couldron_Behaviours_IBehaviour">IBehaviour</a></td><td>
Represents a behaviour</td></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Couldron_Behaviours_IBehaviour_1">IBehaviour(T)</a></td><td>
Represents a behaviour</td></tr></table>&nbsp;

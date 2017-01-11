# ApplicationBase Events
 _**\[This is preliminary documentation and is subject to change.\]**_

The <a href="T_Cauldron_XAML_ApplicationBase">ApplicationBase</a> type exposes the following members.


## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Activated</td><td>
Occurs when an application becomes the foreground application.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Cauldron_XAML_ApplicationBase_BehaviourInvoke">BehaviourInvoke</a></td><td /></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Deactivated</td><td>
Occurs when an application stops being the foreground application.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>DispatcherUnhandledException</td><td>
Occurs when an exception is thrown by an application but not handled.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Exit</td><td>
Occurs just before an application shuts down, and cannot be canceled.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>FragmentNavigation</td><td>
Occurs when a navigator in the application begins navigation to a content fragment, Navigation occurs immediately if the desired fragment is in the current content, or after the source XAML content has been loaded if the desired fragment is in different content.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>LoadCompleted</td><td>
Occurs when content that was navigated to by a navigator in the application has been loaded, parsed, and has begun rendering.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Navigated</td><td>
Occurs when the content that is being navigated to by a navigator in the application has been found, although it may not have completed loading.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Navigating</td><td>
Occurs when a new navigation is requested by a navigator in the application.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationFailed</td><td>
Occurs when an error occurs while a navigator in the application is navigating to the requested content.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationProgress</td><td>
Occurs periodically during a download that is being managed by a navigator in the application to provide navigation progress information.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationStopped</td><td>
Occurs when the StopLoading method of a navigator in the application is called, or when a new navigation is requested by a navigator while a current navigation is in progress.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Cauldron_XAML_ApplicationBase_PropertyChanged">PropertyChanged</a></td><td /></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>SessionEnding</td><td>
Occurs when the user ends the Windows session by logging off or shutting down the operating system.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Startup</td><td>
Occurs when the Run() method of the Application object is called.
 (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#applicationbase-events">Back to Top</a>

## See Also


#### Reference
<a href="T_Cauldron_XAML_ApplicationBase">ApplicationBase Class</a><br /><a href="N_Cauldron_XAML">Cauldron.XAML Namespace</a><br />
using System;
using System.Runtime.CompilerServices;
using Etg.SimpleStubs;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cauldron.Core;
using Windows.Storage;
using Cauldron.XAML.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Windows.Input;
using Cauldron.XAML.Navigation;

namespace System
{
    [CompilerGenerated]
    public class StubICloneable : ICloneable
    {
        private readonly StubContainer<StubICloneable> _stubs = new StubContainer<StubICloneable>();

        object global::System.ICloneable.Clone()
        {
            return _stubs.GetMethodStub<Clone_Delegate>("Clone").Invoke();
        }

        public delegate object Clone_Delegate();

        public StubICloneable Clone(Clone_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Core
{
    [CompilerGenerated]
    public class StubIDisposableObject : IDisposableObject
    {
        private readonly StubContainer<StubIDisposableObject> _stubs = new StubContainer<StubIDisposableObject>();

        bool global::Cauldron.Core.IDisposableObject.IsDisposed
        {
            get
            {
                return _stubs.GetMethodStub<IsDisposed_Get_Delegate>("get_IsDisposed").Invoke();
            }
        }

        public event global::System.EventHandler Disposed;

        protected void On_Disposed(object sender)
        {
            global::System.EventHandler handler = Disposed;
            if (handler != null) { handler(sender, null); }
        }

        public void Disposed_Raise(object sender)
        {
            On_Disposed(sender);
        }

        public delegate bool IsDisposed_Get_Delegate();

        public StubIDisposableObject IsDisposed_Get(IsDisposed_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::System.IDisposable.Dispose()
        {
            _stubs.GetMethodStub<IDisposable_Dispose_Delegate>("Dispose").Invoke();
        }

        public delegate void IDisposable_Dispose_Delegate();

        public StubIDisposableObject Dispose(IDisposable_Dispose_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Activator
{
    [CompilerGenerated]
    public class StubIFactoryExtension : IFactoryExtension
    {
        private readonly StubContainer<StubIFactoryExtension> _stubs = new StubContainer<StubIFactoryExtension>();

        bool global::Cauldron.Activator.IFactoryExtension.CanHandleAmbiguousMatch
        {
            get
            {
                return _stubs.GetMethodStub<CanHandleAmbiguousMatch_Get_Delegate>("get_CanHandleAmbiguousMatch").Invoke();
            }
        }

        public delegate bool CanHandleAmbiguousMatch_Get_Delegate();

        public StubIFactoryExtension CanHandleAmbiguousMatch_Get(CanHandleAmbiguousMatch_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        bool global::Cauldron.Activator.IFactoryExtension.CanModifyArguments(global::System.Reflection.MethodBase method, global::System.Type objectType)
        {
            return _stubs.GetMethodStub<CanModifyArguments_MethodBase_Type_Delegate>("CanModifyArguments").Invoke(method, objectType);
        }

        public delegate bool CanModifyArguments_MethodBase_Type_Delegate(global::System.Reflection.MethodBase method, global::System.Type objectType);

        public StubIFactoryExtension CanModifyArguments(CanModifyArguments_MethodBase_Type_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        object[] global::Cauldron.Activator.IFactoryExtension.ModifyArgument(global::System.Reflection.ParameterInfo[] argumentTypes, object[] arguments)
        {
            return _stubs.GetMethodStub<ModifyArgument_ParameterInfoArray_ObjectArray_Delegate>("ModifyArgument").Invoke(argumentTypes, arguments);
        }

        public delegate object[] ModifyArgument_ParameterInfoArray_ObjectArray_Delegate(global::System.Reflection.ParameterInfo[] argumentTypes, object[] arguments);

        public StubIFactoryExtension ModifyArgument(ModifyArgument_ParameterInfoArray_ObjectArray_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.Activator.IFactoryExtension.OnCreateObject(object context, global::System.Type objectType)
        {
            _stubs.GetMethodStub<OnCreateObject_Object_Type_Delegate>("OnCreateObject").Invoke(context, objectType);
        }

        public delegate void OnCreateObject_Object_Type_Delegate(object context, global::System.Type objectType);

        public StubIFactoryExtension OnCreateObject(OnCreateObject_Object_Type_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.Activator.IFactoryExtension.OnInitialize(global::System.Type type)
        {
            _stubs.GetMethodStub<OnInitialize_Type_Delegate>("OnInitialize").Invoke(type);
        }

        public delegate void OnInitialize_Type_Delegate(global::System.Type type);

        public StubIFactoryExtension OnInitialize(OnInitialize_Type_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Type global::Cauldron.Activator.IFactoryExtension.SelectAmbiguousMatch(global::System.Collections.Generic.IEnumerable<global::System.Type> ambiguousTypes, string contractName)
        {
            return _stubs.GetMethodStub<SelectAmbiguousMatch_IEnumerableOfType_String_Delegate>("SelectAmbiguousMatch").Invoke(ambiguousTypes, contractName);
        }

        public delegate global::System.Type SelectAmbiguousMatch_IEnumerableOfType_String_Delegate(global::System.Collections.Generic.IEnumerable<global::System.Type> ambiguousTypes, string contractName);

        public StubIFactoryExtension SelectAmbiguousMatch(SelectAmbiguousMatch_IEnumerableOfType_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Activator
{
    [CompilerGenerated]
    public class StubIFactoryInitializeComponent : IFactoryInitializeComponent
    {
        private readonly StubContainer<StubIFactoryInitializeComponent> _stubs = new StubContainer<StubIFactoryInitializeComponent>();

        global::System.Threading.Tasks.Task global::Cauldron.Activator.IFactoryInitializeComponent.OnInitializeComponentAsync()
        {
            return _stubs.GetMethodStub<OnInitializeComponentAsync_Delegate>("OnInitializeComponentAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task OnInitializeComponentAsync_Delegate();

        public StubIFactoryInitializeComponent OnInitializeComponentAsync(OnInitializeComponentAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Localization
{
    [CompilerGenerated]
    public class StubILocalizationSource : ILocalizationSource
    {
        private readonly StubContainer<StubILocalizationSource> _stubs = new StubContainer<StubILocalizationSource>();

        bool global::Cauldron.Localization.ILocalizationSource.Contains(string key, string twoLetterISOLanguageName)
        {
            return _stubs.GetMethodStub<Contains_String_String_Delegate>("Contains").Invoke(key, twoLetterISOLanguageName);
        }

        public delegate bool Contains_String_String_Delegate(string key, string twoLetterISOLanguageName);

        public StubILocalizationSource Contains(Contains_String_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        string global::Cauldron.Localization.ILocalizationSource.GetValue(string key, string twoLetterISOLanguageName)
        {
            return _stubs.GetMethodStub<GetValue_String_String_Delegate>("GetValue").Invoke(key, twoLetterISOLanguageName);
        }

        public delegate string GetValue_String_String_Delegate(string key, string twoLetterISOLanguageName);

        public StubILocalizationSource GetValue(GetValue_String_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Potions
{
    [CompilerGenerated]
    public class StubINetwork : INetwork
    {
        private readonly StubContainer<StubINetwork> _stubs = new StubContainer<StubINetwork>();

        global::Cauldron.Core.ConnectionGenerationTypes global::Cauldron.Potions.INetwork.ConnectionType
        {
            get
            {
                return _stubs.GetMethodStub<ConnectionType_Get_Delegate>("get_ConnectionType").Invoke();
            }
        }

        bool global::Cauldron.Potions.INetwork.HasInternetConnection
        {
            get
            {
                return _stubs.GetMethodStub<HasInternetConnection_Get_Delegate>("get_HasInternetConnection").Invoke();
            }
        }

        public delegate global::Cauldron.Core.ConnectionGenerationTypes ConnectionType_Get_Delegate();

        public StubINetwork ConnectionType_Get(ConnectionType_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool HasInternetConnection_Get_Delegate();

        public StubINetwork HasInternetConnection_Get(HasInternetConnection_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<global::Cauldron.Core.PingResults> global::Cauldron.Potions.INetwork.Ping(string hostname, uint port)
        {
            return _stubs.GetMethodStub<Ping_String_UInt32_Delegate>("Ping").Invoke(hostname, port);
        }

        public delegate global::System.Threading.Tasks.Task<global::Cauldron.Core.PingResults> Ping_String_UInt32_Delegate(string hostname, uint port);

        public StubINetwork Ping(Ping_String_UInt32_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Potions
{
    [CompilerGenerated]
    public class StubISerializer : ISerializer
    {
        private readonly StubContainer<StubISerializer> _stubs = new StubContainer<StubISerializer>();

        global::System.Threading.Tasks.Task<object> global::Cauldron.Potions.ISerializer.DeserializeAsync(global::System.Type type, string name)
        {
            return _stubs.GetMethodStub<DeserializeAsync_Type_String_Delegate>("DeserializeAsync").Invoke(type, name);
        }

        public delegate global::System.Threading.Tasks.Task<object> DeserializeAsync_Type_String_Delegate(global::System.Type type, string name);

        public StubISerializer DeserializeAsync(DeserializeAsync_Type_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<object> global::Cauldron.Potions.ISerializer.DeserializeAsync(global::System.Type type, global::Windows.Storage.StorageFolder folder, string name)
        {
            return _stubs.GetMethodStub<DeserializeAsync_Type_StorageFolder_String_Delegate>("DeserializeAsync").Invoke(type, folder, name);
        }

        public delegate global::System.Threading.Tasks.Task<object> DeserializeAsync_Type_StorageFolder_String_Delegate(global::System.Type type, global::Windows.Storage.StorageFolder folder, string name);

        public StubISerializer DeserializeAsync(DeserializeAsync_Type_StorageFolder_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<T> global::Cauldron.Potions.ISerializer.DeserializeAsync<T>(global::Windows.Storage.StorageFolder folder, string name)
        {
            return _stubs.GetMethodStub<DeserializeAsync_StorageFolder_String_Delegate<T>>("DeserializeAsync<T>").Invoke(folder, name);
        }

        public delegate global::System.Threading.Tasks.Task<T> DeserializeAsync_StorageFolder_String_Delegate<T>(global::Windows.Storage.StorageFolder folder, string name) where T : class;

        public StubISerializer DeserializeAsync<T>(DeserializeAsync_StorageFolder_String_Delegate<T> del, int count = Times.Forever, bool overwrite = false) where T : class
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<T> global::Cauldron.Potions.ISerializer.DeserializeAsync<T>(string name)
        {
            return _stubs.GetMethodStub<DeserializeAsync_String_Delegate<T>>("DeserializeAsync<T>").Invoke(name);
        }

        public delegate global::System.Threading.Tasks.Task<T> DeserializeAsync_String_Delegate<T>(string name) where T : class;

        public StubISerializer DeserializeAsync<T>(DeserializeAsync_String_Delegate<T> del, int count = Times.Forever, bool overwrite = false) where T : class
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.Potions.ISerializer.Serialize(object context, global::Windows.Storage.StorageFolder folder, string name)
        {
            _stubs.GetMethodStub<Serialize_Object_StorageFolder_String_Delegate>("Serialize").Invoke(context, folder, name);
        }

        public delegate void Serialize_Object_StorageFolder_String_Delegate(object context, global::Windows.Storage.StorageFolder folder, string name);

        public StubISerializer Serialize(Serialize_Object_StorageFolder_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.Potions.ISerializer.SerializeAsync(object context, string name)
        {
            return _stubs.GetMethodStub<SerializeAsync_Object_String_Delegate>("SerializeAsync").Invoke(context, name);
        }

        public delegate global::System.Threading.Tasks.Task SerializeAsync_Object_String_Delegate(object context, string name);

        public StubISerializer SerializeAsync(SerializeAsync_Object_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.Potions.ISerializer.SerializeAsync(object context, global::Windows.Storage.StorageFolder folder, string name)
        {
            return _stubs.GetMethodStub<SerializeAsync_Object_StorageFolder_String_Delegate>("SerializeAsync").Invoke(context, folder, name);
        }

        public delegate global::System.Threading.Tasks.Task SerializeAsync_Object_StorageFolder_String_Delegate(object context, global::Windows.Storage.StorageFolder folder, string name);

        public StubISerializer SerializeAsync(SerializeAsync_Object_StorageFolder_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Potions
{
    [CompilerGenerated]
    public class StubIUserInformation : IUserInformation
    {
        private readonly StubContainer<StubIUserInformation> _stubs = new StubContainer<StubIUserInformation>();

        global::System.Threading.Tasks.Task<string> global::Cauldron.Potions.IUserInformation.GetDisplayNameAsync()
        {
            return _stubs.GetMethodStub<GetDisplayNameAsync_Delegate>("GetDisplayNameAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<string> GetDisplayNameAsync_Delegate();

        public StubIUserInformation GetDisplayNameAsync(GetDisplayNameAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<string> global::Cauldron.Potions.IUserInformation.GetDomainNameAsync()
        {
            return _stubs.GetMethodStub<GetDomainNameAsync_Delegate>("GetDomainNameAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<string> GetDomainNameAsync_Delegate();

        public StubIUserInformation GetDomainNameAsync(GetDomainNameAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<string> global::Cauldron.Potions.IUserInformation.GetFirstNameAsync()
        {
            return _stubs.GetMethodStub<GetFirstNameAsync_Delegate>("GetFirstNameAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<string> GetFirstNameAsync_Delegate();

        public StubIUserInformation GetFirstNameAsync(GetFirstNameAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<string> global::Cauldron.Potions.IUserInformation.GetLastNameAsync()
        {
            return _stubs.GetMethodStub<GetLastNameAsync_Delegate>("GetLastNameAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<string> GetLastNameAsync_Delegate();

        public StubIUserInformation GetLastNameAsync(GetLastNameAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<string> global::Cauldron.Potions.IUserInformation.GetPrincipalNameAsync()
        {
            return _stubs.GetMethodStub<GetPrincipalNameAsync_Delegate>("GetPrincipalNameAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<string> GetPrincipalNameAsync_Delegate();

        public StubIUserInformation GetPrincipalNameAsync(GetPrincipalNameAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<string> global::Cauldron.Potions.IUserInformation.GetUserNameAsync()
        {
            return _stubs.GetMethodStub<GetUserNameAsync_Delegate>("GetUserNameAsync").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<string> GetUserNameAsync_Delegate();

        public StubIUserInformation GetUserNameAsync(GetUserNameAsync_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.Potions
{
    [CompilerGenerated]
    public class StubIWeb : IWeb
    {
        private readonly StubContainer<StubIWeb> _stubs = new StubContainer<StubIWeb>();

        global::System.Threading.Tasks.Task global::Cauldron.Potions.IWeb.DownloadFile(global::System.Uri uri, global::Windows.Storage.StorageFile resultFile)
        {
            return _stubs.GetMethodStub<DownloadFile_Uri_StorageFile_Delegate>("DownloadFile").Invoke(uri, resultFile);
        }

        public delegate global::System.Threading.Tasks.Task DownloadFile_Uri_StorageFile_Delegate(global::System.Uri uri, global::Windows.Storage.StorageFile resultFile);

        public StubIWeb DownloadFile(DownloadFile_Uri_StorageFile_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.XAML.Interactivity
{
    [CompilerGenerated]
    public class StubIBehaviour : IBehaviour
    {
        private readonly StubContainer<StubIBehaviour> _stubs = new StubContainer<StubIBehaviour>();

        object global::Cauldron.XAML.Interactivity.IBehaviour.AssociatedObject
        {
            get
            {
                return _stubs.GetMethodStub<AssociatedObject_Get_Delegate>("get_AssociatedObject").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<AssociatedObject_Set_Delegate>("set_AssociatedObject").Invoke(value);
            }
        }

        bool global::Cauldron.XAML.Interactivity.IBehaviour.IsAssignedFromTemplate
        {
            get
            {
                return _stubs.GetMethodStub<IsAssignedFromTemplate_Get_Delegate>("get_IsAssignedFromTemplate").Invoke();
            }
        }

        string global::Cauldron.XAML.Interactivity.IBehaviour.Name
        {
            get
            {
                return _stubs.GetMethodStub<Name_Get_Delegate>("get_Name").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<Name_Set_Delegate>("set_Name").Invoke(value);
            }
        }

        public delegate object AssociatedObject_Get_Delegate();

        public StubIBehaviour AssociatedObject_Get(AssociatedObject_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void AssociatedObject_Set_Delegate(object value);

        public StubIBehaviour AssociatedObject_Set(AssociatedObject_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IsAssignedFromTemplate_Get_Delegate();

        public StubIBehaviour IsAssignedFromTemplate_Get(IsAssignedFromTemplate_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate string Name_Get_Delegate();

        public StubIBehaviour Name_Get(Name_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void Name_Set_Delegate(string value);

        public StubIBehaviour Name_Set(Name_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.Interactivity.IBehaviour.Attach()
        {
            _stubs.GetMethodStub<Attach_Delegate>("Attach").Invoke();
        }

        public delegate void Attach_Delegate();

        public StubIBehaviour Attach(Attach_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::Cauldron.XAML.Interactivity.IBehaviour global::Cauldron.XAML.Interactivity.IBehaviour.Copy()
        {
            return _stubs.GetMethodStub<Copy_Delegate>("Copy").Invoke();
        }

        public delegate global::Cauldron.XAML.Interactivity.IBehaviour Copy_Delegate();

        public StubIBehaviour Copy(Copy_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.Interactivity.IBehaviour.DataContextChanged(object newDataContext)
        {
            _stubs.GetMethodStub<DataContextChanged_Object_Delegate>("DataContextChanged").Invoke(newDataContext);
        }

        public delegate void DataContextChanged_Object_Delegate(object newDataContext);

        public StubIBehaviour DataContextChanged(DataContextChanged_Object_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.Interactivity.IBehaviour.DataContextPropertyChanged(string name)
        {
            _stubs.GetMethodStub<DataContextPropertyChanged_String_Delegate>("DataContextPropertyChanged").Invoke(name);
        }

        public delegate void DataContextPropertyChanged_String_Delegate(string name);

        public StubIBehaviour DataContextPropertyChanged(DataContextPropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.Interactivity.IBehaviour.Detach()
        {
            _stubs.GetMethodStub<Detach_Delegate>("Detach").Invoke();
        }

        public delegate void Detach_Delegate();

        public StubIBehaviour Detach(Detach_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.XAML.Validation.ViewModels
{
    [CompilerGenerated]
    public class StubIValidatableViewModel : IValidatableViewModel
    {
        private readonly StubContainer<StubIValidatableViewModel> _stubs = new StubContainer<StubIValidatableViewModel>();

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        bool global::System.ComponentModel.INotifyDataErrorInfo.HasErrors
        {
            get
            {
                return _stubs.GetMethodStub<INotifyDataErrorInfo_HasErrors_Get_Delegate>("get_HasErrors").Invoke();
            }
        }

        bool global::Cauldron.Core.IDisposableObject.IsDisposed
        {
            get
            {
                return _stubs.GetMethodStub<IDisposableObject_IsDisposed_Get_Delegate>("get_IsDisposed").Invoke();
            }
        }

        void global::Cauldron.XAML.Validation.ViewModels.IValidatableViewModel.Validate()
        {
            _stubs.GetMethodStub<Validate_Delegate>("Validate").Invoke();
        }

        public delegate void Validate_Delegate();

        public StubIValidatableViewModel Validate(Validate_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.Validation.ViewModels.IValidatableViewModel.Validate(string propertyName)
        {
            _stubs.GetMethodStub<Validate_String_Delegate>("Validate").Invoke(propertyName);
        }

        public delegate void Validate_String_Delegate(string propertyName);

        public StubIValidatableViewModel Validate(Validate_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.Validation.ViewModels.IValidatableViewModel.Validate(global::System.Reflection.PropertyInfo sender, string propertyName)
        {
            _stubs.GetMethodStub<Validate_PropertyInfo_String_Delegate>("Validate").Invoke(sender, propertyName);
        }

        public delegate void Validate_PropertyInfo_String_Delegate(global::System.Reflection.PropertyInfo sender, string propertyName);

        public StubIValidatableViewModel Validate(Validate_PropertyInfo_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubIValidatableViewModel Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubIValidatableViewModel Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubIValidatableViewModel IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubIValidatableViewModel IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubIValidatableViewModel OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubIValidatableViewModel RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubIValidatableViewModel RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }

        public delegate bool INotifyDataErrorInfo_HasErrors_Get_Delegate();

        public StubIValidatableViewModel HasErrors_Get(INotifyDataErrorInfo_HasErrors_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.EventHandler<global::System.ComponentModel.DataErrorsChangedEventArgs> ErrorsChanged;

        protected void On_ErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            global::System.EventHandler<global::System.ComponentModel.DataErrorsChangedEventArgs> handler = ErrorsChanged;
            if (handler != null) { handler(sender, args); }
        }

        public void ErrorsChanged_Raise(object sender, DataErrorsChangedEventArgs args)
        {
            On_ErrorsChanged(sender, args);
        }

        global::System.Collections.IEnumerable global::System.ComponentModel.INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return _stubs.GetMethodStub<INotifyDataErrorInfo_GetErrors_String_Delegate>("GetErrors").Invoke(propertyName);
        }

        public delegate global::System.Collections.IEnumerable INotifyDataErrorInfo_GetErrors_String_Delegate(string propertyName);

        public StubIValidatableViewModel GetErrors(INotifyDataErrorInfo_GetErrors_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.EventHandler Disposed;

        protected void On_Disposed(object sender)
        {
            global::System.EventHandler handler = Disposed;
            if (handler != null) { handler(sender, null); }
        }

        public void Disposed_Raise(object sender)
        {
            On_Disposed(sender);
        }

        public delegate bool IDisposableObject_IsDisposed_Get_Delegate();

        public StubIValidatableViewModel IsDisposed_Get(IDisposableObject_IsDisposed_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::System.IDisposable.Dispose()
        {
            _stubs.GetMethodStub<IDisposable_Dispose_Delegate>("Dispose").Invoke();
        }

        public delegate void IDisposable_Dispose_Delegate();

        public StubIValidatableViewModel Dispose(IDisposable_Dispose_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.XAML
{
    [CompilerGenerated]
    public class StubIImageManager : IImageManager
    {
        private readonly StubContainer<StubIImageManager> _stubs = new StubContainer<StubIImageManager>();

        global::System.Threading.Tasks.Task<global::Windows.UI.Xaml.Media.Imaging.BitmapImage> global::Cauldron.XAML.IImageManager.GetImageAsync(string resourceInfoName)
        {
            return _stubs.GetMethodStub<GetImageAsync_String_Delegate>("GetImageAsync").Invoke(resourceInfoName);
        }

        public delegate global::System.Threading.Tasks.Task<global::Windows.UI.Xaml.Media.Imaging.BitmapImage> GetImageAsync_String_Delegate(string resourceInfoName);

        public StubIImageManager GetImageAsync(GetImageAsync_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<global::Windows.UI.Xaml.Media.Imaging.BitmapImage> global::Cauldron.XAML.IImageManager.LoadBitmapImageAsync(global::Windows.Storage.StorageFile file)
        {
            return _stubs.GetMethodStub<LoadBitmapImageAsync_StorageFile_Delegate>("LoadBitmapImageAsync").Invoke(file);
        }

        public delegate global::System.Threading.Tasks.Task<global::Windows.UI.Xaml.Media.Imaging.BitmapImage> LoadBitmapImageAsync_StorageFile_Delegate(global::Windows.Storage.StorageFile file);

        public StubIImageManager LoadBitmapImageAsync(LoadBitmapImageAsync_StorageFile_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::System.IDisposable.Dispose()
        {
            _stubs.GetMethodStub<IDisposable_Dispose_Delegate>("Dispose").Invoke();
        }

        public delegate void IDisposable_Dispose_Delegate();

        public StubIImageManager Dispose(IDisposable_Dispose_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.XAML
{
    [CompilerGenerated]
    public class StubIMessageDialog : IMessageDialog
    {
        private readonly StubContainer<StubIMessageDialog> _stubs = new StubContainer<StubIMessageDialog>();

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowAsync(string title, string content, global::Cauldron.XAML.CauldronUICommand command1, global::Cauldron.XAML.CauldronUICommand command2)
        {
            return _stubs.GetMethodStub<ShowAsync_String_String_CauldronUICommand_CauldronUICommand_Delegate>("ShowAsync").Invoke(title, content, command1, command2);
        }

        public delegate global::System.Threading.Tasks.Task ShowAsync_String_String_CauldronUICommand_CauldronUICommand_Delegate(string title, string content, global::Cauldron.XAML.CauldronUICommand command1, global::Cauldron.XAML.CauldronUICommand command2);

        public StubIMessageDialog ShowAsync(ShowAsync_String_String_CauldronUICommand_CauldronUICommand_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, global::Cauldron.XAML.MessageBoxImage messageBoxImage, global::Cauldron.XAML.CauldronUICommandCollection commands)
        {
            return _stubs.GetMethodStub<ShowAsync_String_String_UInt32_UInt32_MessageBoxImage_CauldronUICommandCollection_Delegate>("ShowAsync").Invoke(title, content, defaultCommandIndex, cancelCommandIndex, messageBoxImage, commands);
        }

        public delegate global::System.Threading.Tasks.Task ShowAsync_String_String_UInt32_UInt32_MessageBoxImage_CauldronUICommandCollection_Delegate(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, global::Cauldron.XAML.MessageBoxImage messageBoxImage, global::Cauldron.XAML.CauldronUICommandCollection commands);

        public StubIMessageDialog ShowAsync(ShowAsync_String_String_UInt32_UInt32_MessageBoxImage_CauldronUICommandCollection_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowAsync(string title, string content, global::Cauldron.XAML.MessageBoxImage messageBoxImage, global::Cauldron.XAML.CauldronUICommand command1, global::Cauldron.XAML.CauldronUICommand command2)
        {
            return _stubs.GetMethodStub<ShowAsync_String_String_MessageBoxImage_CauldronUICommand_CauldronUICommand_Delegate>("ShowAsync").Invoke(title, content, messageBoxImage, command1, command2);
        }

        public delegate global::System.Threading.Tasks.Task ShowAsync_String_String_MessageBoxImage_CauldronUICommand_CauldronUICommand_Delegate(string title, string content, global::Cauldron.XAML.MessageBoxImage messageBoxImage, global::Cauldron.XAML.CauldronUICommand command1, global::Cauldron.XAML.CauldronUICommand command2);

        public StubIMessageDialog ShowAsync(ShowAsync_String_String_MessageBoxImage_CauldronUICommand_CauldronUICommand_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, global::Cauldron.XAML.CauldronUICommandCollection commands)
        {
            return _stubs.GetMethodStub<ShowAsync_String_String_UInt32_UInt32_CauldronUICommandCollection_Delegate>("ShowAsync").Invoke(title, content, defaultCommandIndex, cancelCommandIndex, commands);
        }

        public delegate global::System.Threading.Tasks.Task ShowAsync_String_String_UInt32_UInt32_CauldronUICommandCollection_Delegate(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, global::Cauldron.XAML.CauldronUICommandCollection commands);

        public StubIMessageDialog ShowAsync(ShowAsync_String_String_UInt32_UInt32_CauldronUICommandCollection_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowException(global::System.Exception e, string format)
        {
            return _stubs.GetMethodStub<ShowException_Exception_String_Delegate>("ShowException").Invoke(e, format);
        }

        public delegate global::System.Threading.Tasks.Task ShowException_Exception_String_Delegate(global::System.Exception e, string format);

        public StubIMessageDialog ShowException(ShowException_Exception_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowException(global::System.Exception e)
        {
            return _stubs.GetMethodStub<ShowException_Exception_Delegate>("ShowException").Invoke(e);
        }

        public delegate global::System.Threading.Tasks.Task ShowException_Exception_Delegate(global::System.Exception e);

        public StubIMessageDialog ShowException(ShowException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowOKAsync(string title, string content)
        {
            return _stubs.GetMethodStub<ShowOKAsync_String_String_Delegate>("ShowOKAsync").Invoke(title, content);
        }

        public delegate global::System.Threading.Tasks.Task ShowOKAsync_String_String_Delegate(string title, string content);

        public StubIMessageDialog ShowOKAsync(ShowOKAsync_String_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowOKAsync(string title, string content, global::System.Action command)
        {
            return _stubs.GetMethodStub<ShowOKAsync_String_String_Action_Delegate>("ShowOKAsync").Invoke(title, content, command);
        }

        public delegate global::System.Threading.Tasks.Task ShowOKAsync_String_String_Action_Delegate(string title, string content, global::System.Action command);

        public StubIMessageDialog ShowOKAsync(ShowOKAsync_String_String_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowOKAsync(string title, string content, global::Cauldron.XAML.MessageBoxImage messageBoxImage)
        {
            return _stubs.GetMethodStub<ShowOKAsync_String_String_MessageBoxImage_Delegate>("ShowOKAsync").Invoke(title, content, messageBoxImage);
        }

        public delegate global::System.Threading.Tasks.Task ShowOKAsync_String_String_MessageBoxImage_Delegate(string title, string content, global::Cauldron.XAML.MessageBoxImage messageBoxImage);

        public StubIMessageDialog ShowOKAsync(ShowOKAsync_String_String_MessageBoxImage_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowOKAsync(string content)
        {
            return _stubs.GetMethodStub<ShowOKAsync_String_Delegate>("ShowOKAsync").Invoke(content);
        }

        public delegate global::System.Threading.Tasks.Task ShowOKAsync_String_Delegate(string content);

        public StubIMessageDialog ShowOKAsync(ShowOKAsync_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowOKCancelAsync(string title, string content, global::System.Action commandOK)
        {
            return _stubs.GetMethodStub<ShowOKCancelAsync_String_String_Action_Delegate>("ShowOKCancelAsync").Invoke(title, content, commandOK);
        }

        public delegate global::System.Threading.Tasks.Task ShowOKCancelAsync_String_String_Action_Delegate(string title, string content, global::System.Action commandOK);

        public StubIMessageDialog ShowOKCancelAsync(ShowOKCancelAsync_String_String_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowOKCancelAsync(string title, string content, global::System.Action commandOK, global::System.Action commandCancel)
        {
            return _stubs.GetMethodStub<ShowOKCancelAsync_String_String_Action_Action_Delegate>("ShowOKCancelAsync").Invoke(title, content, commandOK, commandCancel);
        }

        public delegate global::System.Threading.Tasks.Task ShowOKCancelAsync_String_String_Action_Action_Delegate(string title, string content, global::System.Action commandOK, global::System.Action commandCancel);

        public StubIMessageDialog ShowOKCancelAsync(ShowOKCancelAsync_String_String_Action_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.IMessageDialog.ShowYesNoAsync(string title, string content)
        {
            return _stubs.GetMethodStub<ShowYesNoAsync_String_String_Delegate>("ShowYesNoAsync").Invoke(title, content);
        }

        public delegate global::System.Threading.Tasks.Task<bool> ShowYesNoAsync_String_String_Delegate(string title, string content);

        public StubIMessageDialog ShowYesNoAsync(ShowYesNoAsync_String_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowYesNoAsync(string title, string content, global::System.Action commandYes)
        {
            return _stubs.GetMethodStub<ShowYesNoAsync_String_String_Action_Delegate>("ShowYesNoAsync").Invoke(title, content, commandYes);
        }

        public delegate global::System.Threading.Tasks.Task ShowYesNoAsync_String_String_Action_Delegate(string title, string content, global::System.Action commandYes);

        public StubIMessageDialog ShowYesNoAsync(ShowYesNoAsync_String_String_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowYesNoAsync(string title, string content, global::System.Action commandYes, global::System.Action commandNo)
        {
            return _stubs.GetMethodStub<ShowYesNoAsync_String_String_Action_Action_Delegate>("ShowYesNoAsync").Invoke(title, content, commandYes, commandNo);
        }

        public delegate global::System.Threading.Tasks.Task ShowYesNoAsync_String_String_Action_Action_Delegate(string title, string content, global::System.Action commandYes, global::System.Action commandNo);

        public StubIMessageDialog ShowYesNoAsync(ShowYesNoAsync_String_String_Action_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowYesNoCancelAsync(string title, string content, global::System.Action commandYes, global::System.Action commandNo, global::System.Action commandCancel)
        {
            return _stubs.GetMethodStub<ShowYesNoCancelAsync_String_String_Action_Action_Action_Delegate>("ShowYesNoCancelAsync").Invoke(title, content, commandYes, commandNo, commandCancel);
        }

        public delegate global::System.Threading.Tasks.Task ShowYesNoCancelAsync_String_String_Action_Action_Action_Delegate(string title, string content, global::System.Action commandYes, global::System.Action commandNo, global::System.Action commandCancel);

        public StubIMessageDialog ShowYesNoCancelAsync(ShowYesNoCancelAsync_String_String_Action_Action_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.IMessageDialog.ShowYesNoCancelAsync(string title, string content, global::System.Action commandYes, global::System.Action commandNo)
        {
            return _stubs.GetMethodStub<ShowYesNoCancelAsync_String_String_Action_Action_Delegate>("ShowYesNoCancelAsync").Invoke(title, content, commandYes, commandNo);
        }

        public delegate global::System.Threading.Tasks.Task ShowYesNoCancelAsync_String_String_Action_Action_Delegate(string title, string content, global::System.Action commandYes, global::System.Action commandNo);

        public StubIMessageDialog ShowYesNoCancelAsync(ShowYesNoCancelAsync_String_String_Action_Action_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.XAML
{
    [CompilerGenerated]
    public class StubIRelayCommand : IRelayCommand
    {
        private readonly StubContainer<StubIRelayCommand> _stubs = new StubContainer<StubIRelayCommand>();

        void global::Cauldron.XAML.IRelayCommand.RefreshCanExecute()
        {
            _stubs.GetMethodStub<RefreshCanExecute_Delegate>("RefreshCanExecute").Invoke();
        }

        public delegate void RefreshCanExecute_Delegate();

        public StubIRelayCommand RefreshCanExecute(RefreshCanExecute_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.EventHandler CanExecuteChanged;

        protected void On_CanExecuteChanged(object sender)
        {
            global::System.EventHandler handler = CanExecuteChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void CanExecuteChanged_Raise(object sender)
        {
            On_CanExecuteChanged(sender);
        }

        bool global::System.Windows.Input.ICommand.CanExecute(object parameter)
        {
            return _stubs.GetMethodStub<ICommand_CanExecute_Object_Delegate>("CanExecute").Invoke(parameter);
        }

        public delegate bool ICommand_CanExecute_Object_Delegate(object parameter);

        public StubIRelayCommand CanExecute(ICommand_CanExecute_Object_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::System.Windows.Input.ICommand.Execute(object parameter)
        {
            _stubs.GetMethodStub<ICommand_Execute_Object_Delegate>("Execute").Invoke(parameter);
        }

        public delegate void ICommand_Execute_Object_Delegate(object parameter);

        public StubIRelayCommand Execute(ICommand_Execute_Object_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubIDialogViewModel<TResult> : IDialogViewModel<TResult>
    {
        private readonly StubContainer<StubIDialogViewModel<TResult>> _stubs = new StubContainer<StubIDialogViewModel<TResult>>();

        TResult global::Cauldron.XAML.ViewModels.IDialogViewModel<TResult>.Result
        {
            get
            {
                return _stubs.GetMethodStub<Result_Get_Delegate>("get_Result").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<Result_Set_Delegate>("set_Result").Invoke(value);
            }
        }

        string global::Cauldron.XAML.ViewModels.IDialogViewModel.Title
        {
            get
            {
                return _stubs.GetMethodStub<IDialogViewModel_Title_Get_Delegate>("get_Title").Invoke();
            }
        }

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        public delegate TResult Result_Get_Delegate();

        public StubIDialogViewModel<TResult> Result_Get(Result_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void Result_Set_Delegate(TResult value);

        public StubIDialogViewModel<TResult> Result_Set(Result_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate string IDialogViewModel_Title_Get_Delegate();

        public StubIDialogViewModel<TResult> Title_Get(IDialogViewModel_Title_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubIDialogViewModel<TResult> Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubIDialogViewModel<TResult> Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubIDialogViewModel<TResult> IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubIDialogViewModel<TResult> IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubIDialogViewModel<TResult> OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubIDialogViewModel<TResult> RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubIDialogViewModel<TResult> RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubIDialogViewModel : IDialogViewModel
    {
        private readonly StubContainer<StubIDialogViewModel> _stubs = new StubContainer<StubIDialogViewModel>();

        string global::Cauldron.XAML.ViewModels.IDialogViewModel.Title
        {
            get
            {
                return _stubs.GetMethodStub<Title_Get_Delegate>("get_Title").Invoke();
            }
        }

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        public delegate string Title_Get_Delegate();

        public StubIDialogViewModel Title_Get(Title_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubIDialogViewModel Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubIDialogViewModel Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubIDialogViewModel IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubIDialogViewModel IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubIDialogViewModel OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubIDialogViewModel RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubIDialogViewModel RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubIFrameAware : IFrameAware
    {
        private readonly StubContainer<StubIFrameAware> _stubs = new StubContainer<StubIFrameAware>();

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        void global::Cauldron.XAML.ViewModels.IFrameAware.Activated()
        {
            _stubs.GetMethodStub<Activated_Delegate>("Activated").Invoke();
        }

        public delegate void Activated_Delegate();

        public StubIFrameAware Activated(Activated_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        bool global::Cauldron.XAML.ViewModels.IFrameAware.CanClose()
        {
            return _stubs.GetMethodStub<CanClose_Delegate>("CanClose").Invoke();
        }

        public delegate bool CanClose_Delegate();

        public StubIFrameAware CanClose(CanClose_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IFrameAware.Deactivated()
        {
            _stubs.GetMethodStub<Deactivated_Delegate>("Deactivated").Invoke();
        }

        public delegate void Deactivated_Delegate();

        public StubIFrameAware Deactivated(Deactivated_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubIFrameAware Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubIFrameAware Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubIFrameAware IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubIFrameAware IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubIFrameAware OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubIFrameAware RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubIFrameAware RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubINavigable : INavigable
    {
        private readonly StubContainer<StubINavigable> _stubs = new StubContainer<StubINavigable>();

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.ViewModels.INavigable.OnNavigatedFrom(global::Cauldron.XAML.Navigation.NavigationInfo args)
        {
            return _stubs.GetMethodStub<OnNavigatedFrom_NavigationInfo_Delegate>("OnNavigatedFrom").Invoke(args);
        }

        public delegate global::System.Threading.Tasks.Task OnNavigatedFrom_NavigationInfo_Delegate(global::Cauldron.XAML.Navigation.NavigationInfo args);

        public StubINavigable OnNavigatedFrom(OnNavigatedFrom_NavigationInfo_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.ViewModels.INavigable.OnNavigatedTo(global::Cauldron.XAML.Navigation.NavigationInfo args)
        {
            return _stubs.GetMethodStub<OnNavigatedTo_NavigationInfo_Delegate>("OnNavigatedTo").Invoke(args);
        }

        public delegate global::System.Threading.Tasks.Task OnNavigatedTo_NavigationInfo_Delegate(global::Cauldron.XAML.Navigation.NavigationInfo args);

        public StubINavigable OnNavigatedTo(OnNavigatedTo_NavigationInfo_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.ViewModels.INavigable.OnNavigatingFrom(global::Cauldron.XAML.Navigation.NavigatingInfo args)
        {
            return _stubs.GetMethodStub<OnNavigatingFrom_NavigatingInfo_Delegate>("OnNavigatingFrom").Invoke(args);
        }

        public delegate global::System.Threading.Tasks.Task OnNavigatingFrom_NavigatingInfo_Delegate(global::Cauldron.XAML.Navigation.NavigatingInfo args);

        public StubINavigable OnNavigatingFrom(OnNavigatingFrom_NavigatingInfo_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubINavigable Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubINavigable Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubINavigable IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubINavigable IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubINavigable OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubINavigable RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubINavigable RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.Navigation
{
    [CompilerGenerated]
    public class StubINavigator : INavigator
    {
        private readonly StubContainer<StubINavigator> _stubs = new StubContainer<StubINavigator>();

        bool global::Cauldron.XAML.Navigation.INavigator.CanGoBack
        {
            get
            {
                return _stubs.GetMethodStub<CanGoBack_Get_Delegate>("get_CanGoBack").Invoke();
            }
        }

        bool global::Cauldron.XAML.Navigation.INavigator.CanGoForward
        {
            get
            {
                return _stubs.GetMethodStub<CanGoForward_Get_Delegate>("get_CanGoForward").Invoke();
            }
        }

        public delegate bool CanGoBack_Get_Delegate();

        public StubINavigator CanGoBack_Get(CanGoBack_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool CanGoForward_Get_Delegate();

        public StubINavigator CanGoForward_Get(CanGoForward_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.Navigation.INavigator.GoBack()
        {
            return _stubs.GetMethodStub<GoBack_Delegate>("GoBack").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<bool> GoBack_Delegate();

        public StubINavigator GoBack(GoBack_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.Navigation.INavigator.GoForward()
        {
            return _stubs.GetMethodStub<GoForward_Delegate>("GoForward").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<bool> GoForward_Delegate();

        public StubINavigator GoForward(GoForward_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.Navigation.INavigator.NavigateAsync(global::System.Type viewModelType)
        {
            return _stubs.GetMethodStub<NavigateAsync_Type_Delegate>("NavigateAsync").Invoke(viewModelType);
        }

        public delegate global::System.Threading.Tasks.Task<bool> NavigateAsync_Type_Delegate(global::System.Type viewModelType);

        public StubINavigator NavigateAsync(NavigateAsync_Type_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.Navigation.INavigator.NavigateAsync(global::System.Type viewModelType, object[] parameters)
        {
            return _stubs.GetMethodStub<NavigateAsync_Type_ObjectArray_Delegate>("NavigateAsync").Invoke(viewModelType, parameters);
        }

        public delegate global::System.Threading.Tasks.Task<bool> NavigateAsync_Type_ObjectArray_Delegate(global::System.Type viewModelType, object[] parameters);

        public StubINavigator NavigateAsync(NavigateAsync_Type_ObjectArray_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.Navigation.INavigator.NavigateAsync<T>()
        {
            return _stubs.GetMethodStub<NavigateAsync_Delegate<T>>("NavigateAsync<T>").Invoke();
        }

        public delegate global::System.Threading.Tasks.Task<bool> NavigateAsync_Delegate<T>() where T : IViewModel;

        public StubINavigator NavigateAsync<T>(NavigateAsync_Delegate<T> del, int count = Times.Forever, bool overwrite = false) where T : IViewModel
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.Navigation.INavigator.NavigateAsync<T,TResult>(global::System.Func<TResult, global::System.Threading.Tasks.Task> callback)
        {
            return _stubs.GetMethodStub<NavigateAsync_FuncOfTResultTask_Delegate<T, TResult>>("NavigateAsync<T,TResult>").Invoke(callback);
        }

        public delegate global::System.Threading.Tasks.Task NavigateAsync_FuncOfTResultTask_Delegate<T,TResult>(global::System.Func<TResult, global::System.Threading.Tasks.Task> callback) where T : class, IDialogViewModel;

        public StubINavigator NavigateAsync<T,TResult>(NavigateAsync_FuncOfTResultTask_Delegate<T, TResult> del, int count = Times.Forever, bool overwrite = false) where T : class, IDialogViewModel
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.Navigation.INavigator.NavigateAsync<T>(global::System.Func<global::System.Threading.Tasks.Task> callback)
        {
            return _stubs.GetMethodStub<NavigateAsync_FuncOfTask_Delegate<T>>("NavigateAsync<T>").Invoke(callback);
        }

        public delegate global::System.Threading.Tasks.Task NavigateAsync_FuncOfTask_Delegate<T>(global::System.Func<global::System.Threading.Tasks.Task> callback) where T : class, IDialogViewModel;

        public StubINavigator NavigateAsync<T>(NavigateAsync_FuncOfTask_Delegate<T> del, int count = Times.Forever, bool overwrite = false) where T : class, IDialogViewModel
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task<bool> global::Cauldron.XAML.Navigation.INavigator.NavigateAsync<T>(object[] parameters)
        {
            return _stubs.GetMethodStub<NavigateAsync_ObjectArray_Delegate<T>>("NavigateAsync<T>").Invoke(parameters);
        }

        public delegate global::System.Threading.Tasks.Task<bool> NavigateAsync_ObjectArray_Delegate<T>(object[] parameters) where T : IViewModel;

        public StubINavigator NavigateAsync<T>(NavigateAsync_ObjectArray_Delegate<T> del, int count = Times.Forever, bool overwrite = false) where T : IViewModel
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.Navigation.INavigator.NavigateAsync<T,TResult>(global::System.Func<TResult, global::System.Threading.Tasks.Task> callback, object[] parameters)
        {
            return _stubs.GetMethodStub<NavigateAsync_FuncOfTResultTask_ObjectArray_Delegate<T, TResult>>("NavigateAsync<T,TResult>").Invoke(callback, parameters);
        }

        public delegate global::System.Threading.Tasks.Task NavigateAsync_FuncOfTResultTask_ObjectArray_Delegate<T,TResult>(global::System.Func<TResult, global::System.Threading.Tasks.Task> callback, object[] parameters) where T : class, IDialogViewModel;

        public StubINavigator NavigateAsync<T,TResult>(NavigateAsync_FuncOfTResultTask_ObjectArray_Delegate<T, TResult> del, int count = Times.Forever, bool overwrite = false) where T : class, IDialogViewModel
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        global::System.Threading.Tasks.Task global::Cauldron.XAML.Navigation.INavigator.NavigateAsync<T>(global::System.Func<global::System.Threading.Tasks.Task> callback, object[] parameters)
        {
            return _stubs.GetMethodStub<NavigateAsync_FuncOfTask_ObjectArray_Delegate<T>>("NavigateAsync<T>").Invoke(callback, parameters);
        }

        public delegate global::System.Threading.Tasks.Task NavigateAsync_FuncOfTask_ObjectArray_Delegate<T>(global::System.Func<global::System.Threading.Tasks.Task> callback, object[] parameters) where T : class, IDialogViewModel;

        public StubINavigator NavigateAsync<T>(NavigateAsync_FuncOfTask_ObjectArray_Delegate<T> del, int count = Times.Forever, bool overwrite = false) where T : class, IDialogViewModel
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        bool global::Cauldron.XAML.Navigation.INavigator.TryClose(global::Cauldron.XAML.ViewModels.IViewModel viewModel)
        {
            return _stubs.GetMethodStub<TryClose_IViewModel_Delegate>("TryClose").Invoke(viewModel);
        }

        public delegate bool TryClose_IViewModel_Delegate(global::Cauldron.XAML.ViewModels.IViewModel viewModel);

        public StubINavigator TryClose(TryClose_IViewModel_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }
    }
}

namespace Cauldron.XAML
{
    [CompilerGenerated]
    public class StubINotifyBehaviourInvocation : INotifyBehaviourInvocation
    {
        private readonly StubContainer<StubINotifyBehaviourInvocation> _stubs = new StubContainer<StubINotifyBehaviourInvocation>();

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubIPrelaunchAware : IPrelaunchAware
    {
        private readonly StubContainer<StubIPrelaunchAware> _stubs = new StubContainer<StubIPrelaunchAware>();

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        void global::Cauldron.XAML.ViewModels.IPrelaunchAware.AppIsVisible()
        {
            _stubs.GetMethodStub<AppIsVisible_Delegate>("AppIsVisible").Invoke();
        }

        public delegate void AppIsVisible_Delegate();

        public StubIPrelaunchAware AppIsVisible(AppIsVisible_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubIPrelaunchAware Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubIPrelaunchAware Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubIPrelaunchAware IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubIPrelaunchAware IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubIPrelaunchAware OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubIPrelaunchAware RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubIPrelaunchAware RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubISizeAware : ISizeAware
    {
        private readonly StubContainer<StubISizeAware> _stubs = new StubContainer<StubISizeAware>();

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IViewModel_IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IViewModel_IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        void global::Cauldron.XAML.ViewModels.ISizeAware.SizeChanged(double width, double height)
        {
            _stubs.GetMethodStub<SizeChanged_Double_Double_Delegate>("SizeChanged").Invoke(width, height);
        }

        public delegate void SizeChanged_Double_Double_Delegate(double width, double height);

        public StubISizeAware SizeChanged(SizeChanged_Double_Double_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::Cauldron.Core.DispatcherEx IViewModel_Dispatcher_Get_Delegate();

        public StubISizeAware Dispatcher_Get(IViewModel_Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid IViewModel_Id_Get_Delegate();

        public StubISizeAware Id_Get(IViewModel_Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IViewModel_IsLoading_Get_Delegate();

        public StubISizeAware IsLoading_Get(IViewModel_IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IViewModel_IsLoading_Set_Delegate(bool value);

        public StubISizeAware IsLoading_Set(IViewModel_IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<IViewModel_OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void IViewModel_OnException_Exception_Delegate(global::System.Exception e);

        public StubISizeAware OnException(IViewModel_OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubISizeAware RaiseNotifyBehaviourInvoke(IViewModel_RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<IViewModel_RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void IViewModel_RaisePropertyChanged_String_Delegate(string propertyName);

        public StubISizeAware RaisePropertyChanged(IViewModel_RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}

namespace Cauldron.XAML.ViewModels
{
    [CompilerGenerated]
    public class StubIViewModel : IViewModel
    {
        private readonly StubContainer<StubIViewModel> _stubs = new StubContainer<StubIViewModel>();

        global::Cauldron.Core.DispatcherEx global::Cauldron.XAML.ViewModels.IViewModel.Dispatcher
        {
            get
            {
                return _stubs.GetMethodStub<Dispatcher_Get_Delegate>("get_Dispatcher").Invoke();
            }
        }

        global::System.Guid global::Cauldron.XAML.ViewModels.IViewModel.Id
        {
            get
            {
                return _stubs.GetMethodStub<Id_Get_Delegate>("get_Id").Invoke();
            }
        }

        bool global::Cauldron.XAML.ViewModels.IViewModel.IsLoading
        {
            get
            {
                return _stubs.GetMethodStub<IsLoading_Get_Delegate>("get_IsLoading").Invoke();
            }

            set
            {
                _stubs.GetMethodStub<IsLoading_Set_Delegate>("set_IsLoading").Invoke(value);
            }
        }

        public delegate global::Cauldron.Core.DispatcherEx Dispatcher_Get_Delegate();

        public StubIViewModel Dispatcher_Get(Dispatcher_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate global::System.Guid Id_Get_Delegate();

        public StubIViewModel Id_Get(Id_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate bool IsLoading_Get_Delegate();

        public StubIViewModel IsLoading_Get(IsLoading_Get_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public delegate void IsLoading_Set_Delegate(bool value);

        public StubIViewModel IsLoading_Set(IsLoading_Set_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.OnException(global::System.Exception e)
        {
            _stubs.GetMethodStub<OnException_Exception_Delegate>("OnException").Invoke(e);
        }

        public delegate void OnException_Exception_Delegate(global::System.Exception e);

        public StubIViewModel OnException(OnException_Exception_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            _stubs.GetMethodStub<RaiseNotifyBehaviourInvoke_String_Delegate>("RaiseNotifyBehaviourInvoke").Invoke(behaviourName);
        }

        public delegate void RaiseNotifyBehaviourInvoke_String_Delegate(string behaviourName);

        public StubIViewModel RaiseNotifyBehaviourInvoke(RaiseNotifyBehaviourInvoke_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        void global::Cauldron.XAML.ViewModels.IViewModel.RaisePropertyChanged(string propertyName)
        {
            _stubs.GetMethodStub<RaisePropertyChanged_String_Delegate>("RaisePropertyChanged").Invoke(propertyName);
        }

        public delegate void RaisePropertyChanged_String_Delegate(string propertyName);

        public StubIViewModel RaisePropertyChanged(RaisePropertyChanged_String_Delegate del, int count = Times.Forever, bool overwrite = false)
        {
            _stubs.SetMethodStub(del, count, overwrite);
            return this;
        }

        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged(object sender)
        {
            global::System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(sender, null); }
        }

        public void PropertyChanged_Raise(object sender)
        {
            On_PropertyChanged(sender);
        }

        public event global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> BehaviourInvoke;

        protected void On_BehaviourInvoke(object sender, BehaviourInvocationArgs args)
        {
            global::System.EventHandler<global::Cauldron.XAML.BehaviourInvocationArgs> handler = BehaviourInvoke;
            if (handler != null) { handler(sender, args); }
        }

        public void BehaviourInvoke_Raise(object sender, BehaviourInvocationArgs args)
        {
            On_BehaviourInvoke(sender, args);
        }
    }
}
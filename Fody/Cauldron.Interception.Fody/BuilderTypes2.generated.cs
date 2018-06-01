
/*
	Generated :)
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using Mono.Cecil;
using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody
{    	
    /// <summary>
    /// Provides predifined types for Cecilator
    /// </summary>
	public static class BuilderTypes2
    {
			
				
		#region Application
        private static BuilderTypeApplication _application;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="System.Windows.Application"/>. 
        /// </summary>
        public static BuilderTypeApplication Application
        {
            get
            {
                if (_application == null) _application = new BuilderTypeApplication(Builder.Current.GetType("System.Windows.Application").Import());
                return _application;
            }
        }

		#endregion
				
		#region ComponentAttribute
        private static BuilderTypeComponentAttribute _componentattribute;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.ComponentAttribute"/>. 
        /// </summary>
        public static BuilderTypeComponentAttribute ComponentAttribute
        {
            get
            {
                if (_componentattribute == null) _componentattribute = new BuilderTypeComponentAttribute(Builder.Current.GetType("Cauldron.Activator.ComponentAttribute").Import());
                return _componentattribute;
            }
        }

		#endregion
				
		#region ComponentConstructorAttribute
        private static BuilderTypeComponentConstructorAttribute _componentconstructorattribute;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.ComponentConstructorAttribute"/>. 
        /// </summary>
        public static BuilderTypeComponentConstructorAttribute ComponentConstructorAttribute
        {
            get
            {
                if (_componentconstructorattribute == null) _componentconstructorattribute = new BuilderTypeComponentConstructorAttribute(Builder.Current.GetType("Cauldron.Activator.ComponentConstructorAttribute").Import());
                return _componentconstructorattribute;
            }
        }

		#endregion
				
		#region Extensions
        private static BuilderTypeExtensions _extensions;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.Extensions"/>. 
        /// </summary>
        public static BuilderTypeExtensions Extensions
        {
            get
            {
                if (_extensions == null) _extensions = new BuilderTypeExtensions(Builder.Current.GetType("Cauldron.Interception.Extensions").Import());
                return _extensions;
            }
        }

		#endregion
				
		#region ExtensionsReflection
        private static BuilderTypeExtensionsReflection _extensionsreflection;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.ExtensionsReflection"/>. 
        /// </summary>
        public static BuilderTypeExtensionsReflection ExtensionsReflection
        {
            get
            {
                if (_extensionsreflection == null) _extensionsreflection = new BuilderTypeExtensionsReflection(Builder.Current.GetType("Cauldron.ExtensionsReflection").Import());
                return _extensionsreflection;
            }
        }

		#endregion
				
		#region Factory
        private static BuilderTypeFactory _factory;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.Factory"/>. 
        /// </summary>
        public static BuilderTypeFactory Factory
        {
            get
            {
                if (_factory == null) _factory = new BuilderTypeFactory(Builder.Current.GetType("Cauldron.Activator.Factory").Import());
                return _factory;
            }
        }

		#endregion
				
		#region Factory`1
        private static BuilderTypeFactory1 _factory_1;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.Factory{T}"/>. 
        /// </summary>
        public static BuilderTypeFactory1 Factory1
        {
            get
            {
                if (_factory_1 == null) _factory_1 = new BuilderTypeFactory1(Builder.Current.GetType("Cauldron.Activator.Factory`1").Import());
                return _factory_1;
            }
        }

		#endregion
				
		#region GenericComponentAttribute
        private static BuilderTypeGenericComponentAttribute _genericcomponentattribute;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.GenericComponentAttribute"/>. 
        /// </summary>
        public static BuilderTypeGenericComponentAttribute GenericComponentAttribute
        {
            get
            {
                if (_genericcomponentattribute == null) _genericcomponentattribute = new BuilderTypeGenericComponentAttribute(Builder.Current.GetType("Cauldron.Activator.GenericComponentAttribute").Import());
                return _genericcomponentattribute;
            }
        }

		#endregion
				
		#region IConstructorInterceptor
        private static BuilderTypeIConstructorInterceptor _iconstructorinterceptor;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.IConstructorInterceptor"/>. 
        /// </summary>
        public static BuilderTypeIConstructorInterceptor IConstructorInterceptor
        {
            get
            {
                if (_iconstructorinterceptor == null) _iconstructorinterceptor = new BuilderTypeIConstructorInterceptor(Builder.Current.GetType("Cauldron.Interception.IConstructorInterceptor").Import());
                return _iconstructorinterceptor;
            }
        }

		#endregion
				
		#region IDisposableObject
        private static BuilderTypeIDisposableObject _idisposableobject;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Core.IDisposableObject"/>. 
        /// </summary>
        public static BuilderTypeIDisposableObject IDisposableObject
        {
            get
            {
                if (_idisposableobject == null) _idisposableobject = new BuilderTypeIDisposableObject(Builder.Current.GetType("Cauldron.Core.IDisposableObject").Import());
                return _idisposableobject;
            }
        }

		#endregion
				
		#region IFactoryExtension
        private static BuilderTypeIFactoryExtension _ifactoryextension;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.IFactoryExtension"/>. 
        /// </summary>
        public static BuilderTypeIFactoryExtension IFactoryExtension
        {
            get
            {
                if (_ifactoryextension == null) _ifactoryextension = new BuilderTypeIFactoryExtension(Builder.Current.GetType("Cauldron.Activator.IFactoryExtension").Import());
                return _ifactoryextension;
            }
        }

		#endregion
				
		#region IFactoryTypeInfo
        private static BuilderTypeIFactoryTypeInfo _ifactorytypeinfo;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Activator.IFactoryTypeInfo"/>. 
        /// </summary>
        public static BuilderTypeIFactoryTypeInfo IFactoryTypeInfo
        {
            get
            {
                if (_ifactorytypeinfo == null) _ifactorytypeinfo = new BuilderTypeIFactoryTypeInfo(Builder.Current.GetType("Cauldron.Activator.IFactoryTypeInfo").Import());
                return _ifactorytypeinfo;
            }
        }

		#endregion
				
		#region IMethodInterceptor
        private static BuilderTypeIMethodInterceptor _imethodinterceptor;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.IMethodInterceptor"/>. 
        /// </summary>
        public static BuilderTypeIMethodInterceptor IMethodInterceptor
        {
            get
            {
                if (_imethodinterceptor == null) _imethodinterceptor = new BuilderTypeIMethodInterceptor(Builder.Current.GetType("Cauldron.Interception.IMethodInterceptor").Import());
                return _imethodinterceptor;
            }
        }

		#endregion
				
		#region InterceptionRuleAttribute
        private static BuilderTypeInterceptionRuleAttribute _interceptionruleattribute;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.InterceptionRuleAttribute"/>. 
        /// </summary>
        public static BuilderTypeInterceptionRuleAttribute InterceptionRuleAttribute
        {
            get
            {
                if (_interceptionruleattribute == null) _interceptionruleattribute = new BuilderTypeInterceptionRuleAttribute(Builder.Current.GetType("Cauldron.Interception.InterceptionRuleAttribute").Import());
                return _interceptionruleattribute;
            }
        }

		#endregion
				
		#region InterceptorOptionsAttribute
        private static BuilderTypeInterceptorOptionsAttribute _interceptoroptionsattribute;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.InterceptorOptionsAttribute"/>. 
        /// </summary>
        public static BuilderTypeInterceptorOptionsAttribute InterceptorOptionsAttribute
        {
            get
            {
                if (_interceptoroptionsattribute == null) _interceptoroptionsattribute = new BuilderTypeInterceptorOptionsAttribute(Builder.Current.GetType("Cauldron.Interception.InterceptorOptionsAttribute").Import());
                return _interceptoroptionsattribute;
            }
        }

		#endregion
				
		#region IPropertyGetterInterceptor
        private static BuilderTypeIPropertyGetterInterceptor _ipropertygetterinterceptor;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.IPropertyGetterInterceptor"/>. 
        /// </summary>
        public static BuilderTypeIPropertyGetterInterceptor IPropertyGetterInterceptor
        {
            get
            {
                if (_ipropertygetterinterceptor == null) _ipropertygetterinterceptor = new BuilderTypeIPropertyGetterInterceptor(Builder.Current.GetType("Cauldron.Interception.IPropertyGetterInterceptor").Import());
                return _ipropertygetterinterceptor;
            }
        }

		#endregion
				
		#region IPropertyInterceptorInitialize
        private static BuilderTypeIPropertyInterceptorInitialize _ipropertyinterceptorinitialize;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.IPropertyInterceptorInitialize"/>. 
        /// </summary>
        public static BuilderTypeIPropertyInterceptorInitialize IPropertyInterceptorInitialize
        {
            get
            {
                if (_ipropertyinterceptorinitialize == null) _ipropertyinterceptorinitialize = new BuilderTypeIPropertyInterceptorInitialize(Builder.Current.GetType("Cauldron.Interception.IPropertyInterceptorInitialize").Import());
                return _ipropertyinterceptorinitialize;
            }
        }

		#endregion
				
		#region IPropertySetterInterceptor
        private static BuilderTypeIPropertySetterInterceptor _ipropertysetterinterceptor;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.IPropertySetterInterceptor"/>. 
        /// </summary>
        public static BuilderTypeIPropertySetterInterceptor IPropertySetterInterceptor
        {
            get
            {
                if (_ipropertysetterinterceptor == null) _ipropertysetterinterceptor = new BuilderTypeIPropertySetterInterceptor(Builder.Current.GetType("Cauldron.Interception.IPropertySetterInterceptor").Import());
                return _ipropertysetterinterceptor;
            }
        }

		#endregion
				
		#region ISimpleMethodInterceptor
        private static BuilderTypeISimpleMethodInterceptor _isimplemethodinterceptor;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.ISimpleMethodInterceptor"/>. 
        /// </summary>
        public static BuilderTypeISimpleMethodInterceptor ISimpleMethodInterceptor
        {
            get
            {
                if (_isimplemethodinterceptor == null) _isimplemethodinterceptor = new BuilderTypeISimpleMethodInterceptor(Builder.Current.GetType("Cauldron.Interception.ISimpleMethodInterceptor").Import());
                return _isimplemethodinterceptor;
            }
        }

		#endregion
				
		#region ISyncRoot
        private static BuilderTypeISyncRoot _isyncroot;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.ISyncRoot"/>. 
        /// </summary>
        public static BuilderTypeISyncRoot ISyncRoot
        {
            get
            {
                if (_isyncroot == null) _isyncroot = new BuilderTypeISyncRoot(Builder.Current.GetType("Cauldron.Interception.ISyncRoot").Import());
                return _isyncroot;
            }
        }

		#endregion
				
		#region PropertyInterceptionInfo
        private static BuilderTypePropertyInterceptionInfo _propertyinterceptioninfo;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="Cauldron.Interception.PropertyInterceptionInfo"/>. 
        /// </summary>
        public static BuilderTypePropertyInterceptionInfo PropertyInterceptionInfo
        {
            get
            {
                if (_propertyinterceptioninfo == null) _propertyinterceptioninfo = new BuilderTypePropertyInterceptionInfo(Builder.Current.GetType("Cauldron.Interception.PropertyInterceptionInfo").Import());
                return _propertyinterceptioninfo;
            }
        }

		#endregion
				
		#region ResourceDictionary
        private static BuilderTypeResourceDictionary _resourcedictionary;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing <see cref="System.Windows.ResourceDictionary"/>. 
        /// </summary>
        public static BuilderTypeResourceDictionary ResourceDictionary
        {
            get
            {
                if (_resourcedictionary == null) _resourcedictionary = new BuilderTypeResourceDictionary(Builder.Current.GetType("System.Windows.ResourceDictionary").Import());
                return _resourcedictionary;
            }
        }

		#endregion
		
	}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="System.Windows.Application"/>
    /// </summary>
    public partial class BuilderTypeApplication : TypeSystemExBase
	{
        internal BuilderTypeApplication(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeApplication value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeApplication value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_run_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 Run()<para/>
		/// </summary>
		public Method GetMethod_Run()
		{
						
			if(this.var_run_0_0 == null)
				this.var_run_0_0 = this.builderType.GetMethod("Run", true);

			return this.var_run_0_0.Import();
						
						
		}
						
		private Method var_run_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 Run(System.Windows.Window)<para/>
		/// </summary>
		public Method GetMethod_Run(TypeReference pwindow)
		{
						
						
			if(this.var_run_0_1 == null)
				this.var_run_0_1 = this.builderType.GetMethod("Run", true, pwindow);
			
			return this.var_run_0_1.Import();
						
		}
						
		private Method var_shutdown_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Shutdown()<para/>
		/// </summary>
		public Method GetMethod_Shutdown()
		{
						
			if(this.var_shutdown_0_0 == null)
				this.var_shutdown_0_0 = this.builderType.GetMethod("Shutdown", true);

			return this.var_shutdown_0_0.Import();
						
						
		}
						
		private Method var_shutdown_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Shutdown(Int32)<para/>
		/// </summary>
		public Method GetMethod_Shutdown(TypeReference pexitCode)
		{
						
						
			if(this.var_shutdown_0_1 == null)
				this.var_shutdown_0_1 = this.builderType.GetMethod("Shutdown", true, pexitCode);
			
			return this.var_shutdown_0_1.Import();
						
		}
						
		private Method var_findresource_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object FindResource(System.Object)<para/>
		/// </summary>
		public Method GetMethod_FindResource()
		{
			if(this.var_findresource_0_1 == null)
				this.var_findresource_0_1 = this.builderType.GetMethod("FindResource", 1, true);

			return this.var_findresource_0_1.Import();
		}
						
		private Method var_tryfindresource_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object TryFindResource(System.Object)<para/>
		/// </summary>
		public Method GetMethod_TryFindResource()
		{
			if(this.var_tryfindresource_0_1 == null)
				this.var_tryfindresource_0_1 = this.builderType.GetMethod("TryFindResource", 1, true);

			return this.var_tryfindresource_0_1.Import();
		}
						
		private Method var_loadcomponent_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void LoadComponent(System.Object, System.Uri)<para/>
		/// </summary>
		public Method GetMethod_LoadComponent(TypeReference pcomponent, TypeReference presourceLocator)
		{
						
						
			if(this.var_loadcomponent_0_2 == null)
				this.var_loadcomponent_0_2 = this.builderType.GetMethod("LoadComponent", true, pcomponent, presourceLocator);
			
			return this.var_loadcomponent_0_2.Import();
						
		}
						
		private Method var_loadcomponent_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object LoadComponent(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_LoadComponent(TypeReference presourceLocator)
		{
						
						
			if(this.var_loadcomponent_0_1 == null)
				this.var_loadcomponent_0_1 = this.builderType.GetMethod("LoadComponent", true, presourceLocator);
			
			return this.var_loadcomponent_0_1.Import();
						
		}
						
		private Method var_getresourcestream_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Resources.StreamResourceInfo GetResourceStream(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetResourceStream()
		{
			if(this.var_getresourcestream_0_1 == null)
				this.var_getresourcestream_0_1 = this.builderType.GetMethod("GetResourceStream", 1, true);

			return this.var_getresourcestream_0_1.Import();
		}
						
		private Method var_getcontentstream_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Resources.StreamResourceInfo GetContentStream(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetContentStream()
		{
			if(this.var_getcontentstream_0_1 == null)
				this.var_getcontentstream_0_1 = this.builderType.GetMethod("GetContentStream", 1, true);

			return this.var_getcontentstream_0_1.Import();
		}
						
		private Method var_getremotestream_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Resources.StreamResourceInfo GetRemoteStream(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetRemoteStream()
		{
			if(this.var_getremotestream_0_1 == null)
				this.var_getremotestream_0_1 = this.builderType.GetMethod("GetRemoteStream", 1, true);

			return this.var_getremotestream_0_1.Import();
		}
						
		private Method var_getcookie_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String GetCookie(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetCookie()
		{
			if(this.var_getcookie_0_1 == null)
				this.var_getcookie_0_1 = this.builderType.GetMethod("GetCookie", 1, true);

			return this.var_getcookie_0_1.Import();
		}
						
		private Method var_setcookie_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void SetCookie(System.Uri, System.String)<para/>
		/// </summary>
		public Method GetMethod_SetCookie()
		{
			if(this.var_setcookie_0_2 == null)
				this.var_setcookie_0_2 = this.builderType.GetMethod("SetCookie", 2, true);

			return this.var_setcookie_0_2.Import();
		}
						
		private Method var_get_current_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Application get_Current()<para/>
		/// </summary>
		public Method GetMethod_get_Current()
		{
			if(this.var_get_current_0_0 == null)
				this.var_get_current_0_0 = this.builderType.GetMethod("get_Current", 0, true);

			return this.var_get_current_0_0.Import();
		}
						
		private Method var_get_windows_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.WindowCollection get_Windows()<para/>
		/// </summary>
		public Method GetMethod_get_Windows()
		{
			if(this.var_get_windows_0_0 == null)
				this.var_get_windows_0_0 = this.builderType.GetMethod("get_Windows", 0, true);

			return this.var_get_windows_0_0.Import();
		}
						
		private Method var_get_mainwindow_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Window get_MainWindow()<para/>
		/// </summary>
		public Method GetMethod_get_MainWindow()
		{
			if(this.var_get_mainwindow_0_0 == null)
				this.var_get_mainwindow_0_0 = this.builderType.GetMethod("get_MainWindow", 0, true);

			return this.var_get_mainwindow_0_0.Import();
		}
						
		private Method var_set_mainwindow_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_MainWindow(System.Windows.Window)<para/>
		/// </summary>
		public Method GetMethod_set_MainWindow()
		{
			if(this.var_set_mainwindow_0_1 == null)
				this.var_set_mainwindow_0_1 = this.builderType.GetMethod("set_MainWindow", 1, true);

			return this.var_set_mainwindow_0_1.Import();
		}
						
		private Method var_get_shutdownmode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.ShutdownMode get_ShutdownMode()<para/>
		/// </summary>
		public Method GetMethod_get_ShutdownMode()
		{
			if(this.var_get_shutdownmode_0_0 == null)
				this.var_get_shutdownmode_0_0 = this.builderType.GetMethod("get_ShutdownMode", 0, true);

			return this.var_get_shutdownmode_0_0.Import();
		}
						
		private Method var_set_shutdownmode_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_ShutdownMode(System.Windows.ShutdownMode)<para/>
		/// </summary>
		public Method GetMethod_set_ShutdownMode()
		{
			if(this.var_set_shutdownmode_0_1 == null)
				this.var_set_shutdownmode_0_1 = this.builderType.GetMethod("set_ShutdownMode", 1, true);

			return this.var_set_shutdownmode_0_1.Import();
		}
						
		private Method var_get_resources_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.ResourceDictionary get_Resources()<para/>
		/// </summary>
		public Method GetMethod_get_Resources()
		{
			if(this.var_get_resources_0_0 == null)
				this.var_get_resources_0_0 = this.builderType.GetMethod("get_Resources", 0, true);

			return this.var_get_resources_0_0.Import();
		}
						
		private Method var_set_resources_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_Resources(System.Windows.ResourceDictionary)<para/>
		/// </summary>
		public Method GetMethod_set_Resources()
		{
			if(this.var_set_resources_0_1 == null)
				this.var_set_resources_0_1 = this.builderType.GetMethod("set_Resources", 1, true);

			return this.var_set_resources_0_1.Import();
		}
						
		private Method var_get_startupuri_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Uri get_StartupUri()<para/>
		/// </summary>
		public Method GetMethod_get_StartupUri()
		{
			if(this.var_get_startupuri_0_0 == null)
				this.var_get_startupuri_0_0 = this.builderType.GetMethod("get_StartupUri", 0, true);

			return this.var_get_startupuri_0_0.Import();
		}
						
		private Method var_set_startupuri_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_StartupUri(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_set_StartupUri()
		{
			if(this.var_set_startupuri_0_1 == null)
				this.var_set_startupuri_0_1 = this.builderType.GetMethod("set_StartupUri", 1, true);

			return this.var_set_startupuri_0_1.Import();
		}
						
		private Method var_get_properties_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.IDictionary get_Properties()<para/>
		/// </summary>
		public Method GetMethod_get_Properties()
		{
			if(this.var_get_properties_0_0 == null)
				this.var_get_properties_0_0 = this.builderType.GetMethod("get_Properties", 0, true);

			return this.var_get_properties_0_0.Import();
		}
						
		private Method var_get_resourceassembly_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.Assembly get_ResourceAssembly()<para/>
		/// </summary>
		public Method GetMethod_get_ResourceAssembly()
		{
			if(this.var_get_resourceassembly_0_0 == null)
				this.var_get_resourceassembly_0_0 = this.builderType.GetMethod("get_ResourceAssembly", 0, true);

			return this.var_get_resourceassembly_0_0.Import();
		}
						
		private Method var_set_resourceassembly_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_ResourceAssembly(System.Reflection.Assembly)<para/>
		/// </summary>
		public Method GetMethod_set_ResourceAssembly()
		{
			if(this.var_set_resourceassembly_0_1 == null)
				this.var_set_resourceassembly_0_1 = this.builderType.GetMethod("set_ResourceAssembly", 1, true);

			return this.var_set_resourceassembly_0_1.Import();
		}
						
		private Method var_add_startup_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Startup(System.Windows.StartupEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Startup()
		{
			if(this.var_add_startup_0_1 == null)
				this.var_add_startup_0_1 = this.builderType.GetMethod("add_Startup", 1, true);

			return this.var_add_startup_0_1.Import();
		}
						
		private Method var_remove_startup_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Startup(System.Windows.StartupEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Startup()
		{
			if(this.var_remove_startup_0_1 == null)
				this.var_remove_startup_0_1 = this.builderType.GetMethod("remove_Startup", 1, true);

			return this.var_remove_startup_0_1.Import();
		}
						
		private Method var_add_exit_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Exit(System.Windows.ExitEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Exit()
		{
			if(this.var_add_exit_0_1 == null)
				this.var_add_exit_0_1 = this.builderType.GetMethod("add_Exit", 1, true);

			return this.var_add_exit_0_1.Import();
		}
						
		private Method var_remove_exit_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Exit(System.Windows.ExitEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Exit()
		{
			if(this.var_remove_exit_0_1 == null)
				this.var_remove_exit_0_1 = this.builderType.GetMethod("remove_Exit", 1, true);

			return this.var_remove_exit_0_1.Import();
		}
						
		private Method var_add_activated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Activated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Activated()
		{
			if(this.var_add_activated_0_1 == null)
				this.var_add_activated_0_1 = this.builderType.GetMethod("add_Activated", 1, true);

			return this.var_add_activated_0_1.Import();
		}
						
		private Method var_remove_activated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Activated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Activated()
		{
			if(this.var_remove_activated_0_1 == null)
				this.var_remove_activated_0_1 = this.builderType.GetMethod("remove_Activated", 1, true);

			return this.var_remove_activated_0_1.Import();
		}
						
		private Method var_add_deactivated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Deactivated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Deactivated()
		{
			if(this.var_add_deactivated_0_1 == null)
				this.var_add_deactivated_0_1 = this.builderType.GetMethod("add_Deactivated", 1, true);

			return this.var_add_deactivated_0_1.Import();
		}
						
		private Method var_remove_deactivated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Deactivated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Deactivated()
		{
			if(this.var_remove_deactivated_0_1 == null)
				this.var_remove_deactivated_0_1 = this.builderType.GetMethod("remove_Deactivated", 1, true);

			return this.var_remove_deactivated_0_1.Import();
		}
						
		private Method var_add_sessionending_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_SessionEnding(System.Windows.SessionEndingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_SessionEnding()
		{
			if(this.var_add_sessionending_0_1 == null)
				this.var_add_sessionending_0_1 = this.builderType.GetMethod("add_SessionEnding", 1, true);

			return this.var_add_sessionending_0_1.Import();
		}
						
		private Method var_remove_sessionending_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_SessionEnding(System.Windows.SessionEndingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_SessionEnding()
		{
			if(this.var_remove_sessionending_0_1 == null)
				this.var_remove_sessionending_0_1 = this.builderType.GetMethod("remove_SessionEnding", 1, true);

			return this.var_remove_sessionending_0_1.Import();
		}
						
		private Method var_add_dispatcherunhandledexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_DispatcherUnhandledException(System.Windows.Threading.DispatcherUnhandledExceptionEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_DispatcherUnhandledException()
		{
			if(this.var_add_dispatcherunhandledexception_0_1 == null)
				this.var_add_dispatcherunhandledexception_0_1 = this.builderType.GetMethod("add_DispatcherUnhandledException", 1, true);

			return this.var_add_dispatcherunhandledexception_0_1.Import();
		}
						
		private Method var_remove_dispatcherunhandledexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_DispatcherUnhandledException(System.Windows.Threading.DispatcherUnhandledExceptionEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_DispatcherUnhandledException()
		{
			if(this.var_remove_dispatcherunhandledexception_0_1 == null)
				this.var_remove_dispatcherunhandledexception_0_1 = this.builderType.GetMethod("remove_DispatcherUnhandledException", 1, true);

			return this.var_remove_dispatcherunhandledexception_0_1.Import();
		}
						
		private Method var_add_navigating_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Navigating(System.Windows.Navigation.NavigatingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Navigating()
		{
			if(this.var_add_navigating_0_1 == null)
				this.var_add_navigating_0_1 = this.builderType.GetMethod("add_Navigating", 1, true);

			return this.var_add_navigating_0_1.Import();
		}
						
		private Method var_remove_navigating_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Navigating(System.Windows.Navigation.NavigatingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Navigating()
		{
			if(this.var_remove_navigating_0_1 == null)
				this.var_remove_navigating_0_1 = this.builderType.GetMethod("remove_Navigating", 1, true);

			return this.var_remove_navigating_0_1.Import();
		}
						
		private Method var_add_navigated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Navigated(System.Windows.Navigation.NavigatedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Navigated()
		{
			if(this.var_add_navigated_0_1 == null)
				this.var_add_navigated_0_1 = this.builderType.GetMethod("add_Navigated", 1, true);

			return this.var_add_navigated_0_1.Import();
		}
						
		private Method var_remove_navigated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Navigated(System.Windows.Navigation.NavigatedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Navigated()
		{
			if(this.var_remove_navigated_0_1 == null)
				this.var_remove_navigated_0_1 = this.builderType.GetMethod("remove_Navigated", 1, true);

			return this.var_remove_navigated_0_1.Import();
		}
						
		private Method var_add_navigationprogress_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_NavigationProgress(System.Windows.Navigation.NavigationProgressEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_NavigationProgress()
		{
			if(this.var_add_navigationprogress_0_1 == null)
				this.var_add_navigationprogress_0_1 = this.builderType.GetMethod("add_NavigationProgress", 1, true);

			return this.var_add_navigationprogress_0_1.Import();
		}
						
		private Method var_remove_navigationprogress_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_NavigationProgress(System.Windows.Navigation.NavigationProgressEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_NavigationProgress()
		{
			if(this.var_remove_navigationprogress_0_1 == null)
				this.var_remove_navigationprogress_0_1 = this.builderType.GetMethod("remove_NavigationProgress", 1, true);

			return this.var_remove_navigationprogress_0_1.Import();
		}
						
		private Method var_add_navigationfailed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_NavigationFailed(System.Windows.Navigation.NavigationFailedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_NavigationFailed()
		{
			if(this.var_add_navigationfailed_0_1 == null)
				this.var_add_navigationfailed_0_1 = this.builderType.GetMethod("add_NavigationFailed", 1, true);

			return this.var_add_navigationfailed_0_1.Import();
		}
						
		private Method var_remove_navigationfailed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_NavigationFailed(System.Windows.Navigation.NavigationFailedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_NavigationFailed()
		{
			if(this.var_remove_navigationfailed_0_1 == null)
				this.var_remove_navigationfailed_0_1 = this.builderType.GetMethod("remove_NavigationFailed", 1, true);

			return this.var_remove_navigationfailed_0_1.Import();
		}
						
		private Method var_add_loadcompleted_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_LoadCompleted(System.Windows.Navigation.LoadCompletedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_LoadCompleted()
		{
			if(this.var_add_loadcompleted_0_1 == null)
				this.var_add_loadcompleted_0_1 = this.builderType.GetMethod("add_LoadCompleted", 1, true);

			return this.var_add_loadcompleted_0_1.Import();
		}
						
		private Method var_remove_loadcompleted_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_LoadCompleted(System.Windows.Navigation.LoadCompletedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_LoadCompleted()
		{
			if(this.var_remove_loadcompleted_0_1 == null)
				this.var_remove_loadcompleted_0_1 = this.builderType.GetMethod("remove_LoadCompleted", 1, true);

			return this.var_remove_loadcompleted_0_1.Import();
		}
						
		private Method var_add_navigationstopped_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_NavigationStopped(System.Windows.Navigation.NavigationStoppedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_NavigationStopped()
		{
			if(this.var_add_navigationstopped_0_1 == null)
				this.var_add_navigationstopped_0_1 = this.builderType.GetMethod("add_NavigationStopped", 1, true);

			return this.var_add_navigationstopped_0_1.Import();
		}
						
		private Method var_remove_navigationstopped_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_NavigationStopped(System.Windows.Navigation.NavigationStoppedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_NavigationStopped()
		{
			if(this.var_remove_navigationstopped_0_1 == null)
				this.var_remove_navigationstopped_0_1 = this.builderType.GetMethod("remove_NavigationStopped", 1, true);

			return this.var_remove_navigationstopped_0_1.Import();
		}
						
		private Method var_add_fragmentnavigation_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_FragmentNavigation(System.Windows.Navigation.FragmentNavigationEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_FragmentNavigation()
		{
			if(this.var_add_fragmentnavigation_0_1 == null)
				this.var_add_fragmentnavigation_0_1 = this.builderType.GetMethod("add_FragmentNavigation", 1, true);

			return this.var_add_fragmentnavigation_0_1.Import();
		}
						
		private Method var_remove_fragmentnavigation_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_FragmentNavigation(System.Windows.Navigation.FragmentNavigationEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_FragmentNavigation()
		{
			if(this.var_remove_fragmentnavigation_0_1 == null)
				this.var_remove_fragmentnavigation_0_1 = this.builderType.GetMethod("remove_FragmentNavigation", 1, true);

			return this.var_remove_fragmentnavigation_0_1.Import();
		}
						
		private Method var_get_dispatcher_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Threading.Dispatcher get_Dispatcher()<para/>
		/// </summary>
		public Method GetMethod_get_Dispatcher()
		{
			if(this.var_get_dispatcher_0_0 == null)
				this.var_get_dispatcher_0_0 = this.builderType.GetMethod("get_Dispatcher", 0, true);

			return this.var_get_dispatcher_0_0.Import();
		}
						
		private Method var_checkaccess_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean CheckAccess()<para/>
		/// </summary>
		public Method GetMethod_CheckAccess()
		{
			if(this.var_checkaccess_0_0 == null)
				this.var_checkaccess_0_0 = this.builderType.GetMethod("CheckAccess", 0, true);

			return this.var_checkaccess_0_0.Import();
		}
						
		private Method var_verifyaccess_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void VerifyAccess()<para/>
		/// </summary>
		public Method GetMethod_VerifyAccess()
		{
			if(this.var_verifyaccess_0_0 == null)
				this.var_verifyaccess_0_0 = this.builderType.GetMethod("VerifyAccess", 0, true);

			return this.var_verifyaccess_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor()<para/>
		/// </summary>
		public Method GetMethod_ctor()
		{
			if(this.var_ctor_0_0 == null)
				this.var_ctor_0_0 = this.builderType.GetMethod(".ctor", 0, true);

			return this.var_ctor_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.ComponentAttribute"/>
    /// </summary>
    public partial class BuilderTypeComponentAttribute : TypeSystemExBase
	{
        internal BuilderTypeComponentAttribute(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeComponentAttribute value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeComponentAttribute value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_get_contractname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_ContractName()<para/>
		/// </summary>
		public Method GetMethod_get_ContractName()
		{
			if(this.var_get_contractname_0_0 == null)
				this.var_get_contractname_0_0 = this.builderType.GetMethod("get_ContractName", 0, true);

			return this.var_get_contractname_0_0.Import();
		}
						
		private Method var_get_invokeonobjectcreationevent_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_InvokeOnObjectCreationEvent()<para/>
		/// </summary>
		public Method GetMethod_get_InvokeOnObjectCreationEvent()
		{
			if(this.var_get_invokeonobjectcreationevent_0_0 == null)
				this.var_get_invokeonobjectcreationevent_0_0 = this.builderType.GetMethod("get_InvokeOnObjectCreationEvent", 0, true);

			return this.var_get_invokeonobjectcreationevent_0_0.Import();
		}
						
		private Method var_set_invokeonobjectcreationevent_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_InvokeOnObjectCreationEvent(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_InvokeOnObjectCreationEvent()
		{
			if(this.var_set_invokeonobjectcreationevent_0_1 == null)
				this.var_set_invokeonobjectcreationevent_0_1 = this.builderType.GetMethod("set_InvokeOnObjectCreationEvent", 1, true);

			return this.var_set_invokeonobjectcreationevent_0_1.Import();
		}
						
		private Method var_get_policy_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.FactoryCreationPolicy get_Policy()<para/>
		/// </summary>
		public Method GetMethod_get_Policy()
		{
			if(this.var_get_policy_0_0 == null)
				this.var_get_policy_0_0 = this.builderType.GetMethod("get_Policy", 0, true);

			return this.var_get_policy_0_0.Import();
		}
						
		private Method var_get_priority_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// UInt32 get_Priority()<para/>
		/// </summary>
		public Method GetMethod_get_Priority()
		{
			if(this.var_get_priority_0_0 == null)
				this.var_get_priority_0_0 = this.builderType.GetMethod("get_Priority", 0, true);

			return this.var_get_priority_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_get_typeid_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_TypeId()<para/>
		/// </summary>
		public Method GetMethod_get_TypeId()
		{
			if(this.var_get_typeid_0_0 == null)
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", 0, true);

			return this.var_get_typeid_0_0.Import();
		}
						
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match()
		{
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", 1, true);

			return this.var_match_0_1.Import();
		}
						
		private Method var_isdefaultattribute_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsDefaultAttribute()<para/>
		/// </summary>
		public Method GetMethod_IsDefaultAttribute()
		{
			if(this.var_isdefaultattribute_0_0 == null)
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", 0, true);

			return this.var_isdefaultattribute_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_2;
				
		private Method var_ctor_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor(System.String, UInt32)<para/>
		/// Void .ctor(System.Type, UInt32)<para/>
		/// </summary>
		public Method GetMethod_ctor(TypeReference pcontractName, TypeReference ppriority)
		{
						
						
			if(typeof(System.String).AreEqual(pcontractName) && typeof(System.UInt32).AreEqual(ppriority))
			{
				if(this.var_ctor_0_2 == null)
					this.var_ctor_0_2 = this.builderType.GetMethod(".ctor", true, pcontractName, ppriority);
			
				return this.var_ctor_0_2.Import();
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.UInt32).AreEqual(ppriority))
			{
				if(this.var_ctor_1_2 == null)
					this.var_ctor_1_2 = this.builderType.GetMethod(".ctor", true, pcontractName, ppriority);
			
				return this.var_ctor_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_ctor_0_1;
				
		private Method var_ctor_1_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor(System.String)<para/>
		/// Void .ctor(System.Type)<para/>
		/// </summary>
		public Method GetMethod_ctor(TypeReference pcontractName)
		{
						
						
			if(typeof(System.String).AreEqual(pcontractName))
			{
				if(this.var_ctor_0_1 == null)
					this.var_ctor_0_1 = this.builderType.GetMethod(".ctor", true, pcontractName);
			
				return this.var_ctor_0_1.Import();
			}
			
			if(typeof(System.Type).AreEqual(pcontractName))
			{
				if(this.var_ctor_1_1 == null)
					this.var_ctor_1_1 = this.builderType.GetMethod(".ctor", true, pcontractName);
			
				return this.var_ctor_1_1.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.ComponentConstructorAttribute"/>
    /// </summary>
    public partial class BuilderTypeComponentConstructorAttribute : TypeSystemExBase
	{
        internal BuilderTypeComponentConstructorAttribute(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeComponentConstructorAttribute value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeComponentConstructorAttribute value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_get_typeid_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_TypeId()<para/>
		/// </summary>
		public Method GetMethod_get_TypeId()
		{
			if(this.var_get_typeid_0_0 == null)
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", 0, true);

			return this.var_get_typeid_0_0.Import();
		}
						
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match()
		{
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", 1, true);

			return this.var_match_0_1.Import();
		}
						
		private Method var_isdefaultattribute_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsDefaultAttribute()<para/>
		/// </summary>
		public Method GetMethod_IsDefaultAttribute()
		{
			if(this.var_isdefaultattribute_0_0 == null)
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", 0, true);

			return this.var_isdefaultattribute_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor()<para/>
		/// </summary>
		public Method GetMethod_ctor()
		{
			if(this.var_ctor_0_0 == null)
				this.var_ctor_0_0 = this.builderType.GetMethod(".ctor", 0, true);

			return this.var_ctor_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.Extensions"/>
    /// </summary>
    public partial class BuilderTypeExtensions : TypeSystemExBase
	{
        internal BuilderTypeExtensions(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeExtensions value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeExtensions value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_getsha256hash_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String GetSHA256Hash(System.String)<para/>
		/// </summary>
		public Method GetMethod_GetSHA256Hash()
		{
			if(this.var_getsha256hash_0_1 == null)
				this.var_getsha256hash_0_1 = this.builderType.GetMethod("GetSHA256Hash", 1, true);

			return this.var_getsha256hash_0_1.Import();
		}
						
		private Method var_trydisposeinternal_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void TryDisposeInternal(System.Object)<para/>
		/// </summary>
		public Method GetMethod_TryDisposeInternal()
		{
			if(this.var_trydisposeinternal_0_1 == null)
				this.var_trydisposeinternal_0_1 = this.builderType.GetMethod("TryDisposeInternal", 1, true);

			return this.var_trydisposeinternal_0_1.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.ExtensionsReflection"/>
    /// </summary>
    public partial class BuilderTypeExtensionsReflection : TypeSystemExBase
	{
        internal BuilderTypeExtensionsReflection(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeExtensionsReflection value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeExtensionsReflection value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_arereferenceassignable_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean AreReferenceAssignable(System.Type, System.Type)<para/>
		/// </summary>
		public Method GetMethod_AreReferenceAssignable()
		{
			if(this.var_arereferenceassignable_0_2 == null)
				this.var_arereferenceassignable_0_2 = this.builderType.GetMethod("AreReferenceAssignable", 2, true);

			return this.var_arereferenceassignable_0_2.Import();
		}
						
		private Method var_createinstance_0_2;
				
		private Method var_createinstance_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object CreateInstance(System.Type, System.Object[])<para/>
		/// System.Object CreateInstance(System.Reflection.ConstructorInfo, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateInstance(TypeReference ptype, TypeReference pargs)
		{
						
						
			if(typeof(System.Type).AreEqual(ptype) && typeof(System.Object[]).AreEqual(pargs))
			{
				if(this.var_createinstance_0_2 == null)
					this.var_createinstance_0_2 = this.builderType.GetMethod("CreateInstance", true, ptype, pargs);
			
				return this.var_createinstance_0_2.Import();
			}
			
			if(typeof(System.Reflection.ConstructorInfo).AreEqual(ptype) && typeof(System.Object[]).AreEqual(pargs))
			{
				if(this.var_createinstance_1_2 == null)
					this.var_createinstance_1_2 = this.builderType.GetMethod("CreateInstance", true, ptype, pargs);
			
				return this.var_createinstance_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_getchildrentype_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetChildrenType(System.Type)<para/>
		/// </summary>
		public Method GetMethod_GetChildrenType()
		{
			if(this.var_getchildrentype_0_1 == null)
				this.var_getchildrentype_0_1 = this.builderType.GetMethod("GetChildrenType", 1, true);

			return this.var_getchildrentype_0_1.Import();
		}
						
		private Method var_getdefaultinstance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object GetDefaultInstance(System.Type)<para/>
		/// </summary>
		public Method GetMethod_GetDefaultInstance()
		{
			if(this.var_getdefaultinstance_0_1 == null)
				this.var_getdefaultinstance_0_1 = this.builderType.GetMethod("GetDefaultInstance", 1, true);

			return this.var_getdefaultinstance_0_1.Import();
		}
						
		private Method var_getdictionarykeyvaluetypes_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type[] GetDictionaryKeyValueTypes(System.Type)<para/>
		/// </summary>
		public Method GetMethod_GetDictionaryKeyValueTypes()
		{
			if(this.var_getdictionarykeyvaluetypes_0_1 == null)
				this.var_getdictionarykeyvaluetypes_0_1 = this.builderType.GetMethod("GetDictionaryKeyValueTypes", 1, true);

			return this.var_getdictionarykeyvaluetypes_0_1.Import();
		}
						
		private Method var_getfieldsex_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[System.Reflection.FieldInfo] GetFieldsEx(System.Type, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetFieldsEx()
		{
			if(this.var_getfieldsex_0_2 == null)
				this.var_getfieldsex_0_2 = this.builderType.GetMethod("GetFieldsEx", 2, true);

			return this.var_getfieldsex_0_2.Import();
		}
						
		private Method var_getmethod_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.MethodInfo GetMethod(System.Type, System.String, System.Type[], System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetMethod()
		{
			if(this.var_getmethod_0_4 == null)
				this.var_getmethod_0_4 = this.builderType.GetMethod("GetMethod", 4, true);

			return this.var_getmethod_0_4.Import();
		}
						
		private Method var_getmethodex_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.MethodInfo GetMethodEx(System.Type, System.String, System.Type[], System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetMethodEx()
		{
			if(this.var_getmethodex_0_4 == null)
				this.var_getmethodex_0_4 = this.builderType.GetMethod("GetMethodEx", 4, true);

			return this.var_getmethodex_0_4.Import();
		}
						
		private Method var_getmethodsex_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[System.Reflection.MethodInfo] GetMethodsEx(System.Type, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetMethodsEx()
		{
			if(this.var_getmethodsex_0_2 == null)
				this.var_getmethodsex_0_2 = this.builderType.GetMethod("GetMethodsEx", 2, true);

			return this.var_getmethodsex_0_2.Import();
		}
						
		private Method var_getpropertiesex_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[System.Reflection.PropertyInfo] GetPropertiesEx(System.Type, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertiesEx()
		{
			if(this.var_getpropertiesex_0_2 == null)
				this.var_getpropertiesex_0_2 = this.builderType.GetMethod("GetPropertiesEx", 2, true);

			return this.var_getpropertiesex_0_2.Import();
		}
						
		private Method var_getpropertyex_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.PropertyInfo GetPropertyEx(System.Type, System.String, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertyEx()
		{
			if(this.var_getpropertyex_0_3 == null)
				this.var_getpropertyex_0_3 = this.builderType.GetMethod("GetPropertyEx", 3, true);

			return this.var_getpropertyex_0_3.Import();
		}
						
		private Method var_getpropertyfrompath_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.PropertyInfo GetPropertyFromPath(System.Type, System.String, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertyFromPath()
		{
			if(this.var_getpropertyfrompath_0_3 == null)
				this.var_getpropertyfrompath_0_3 = this.builderType.GetMethod("GetPropertyFromPath", 3, true);

			return this.var_getpropertyfrompath_0_3.Import();
		}
						
		private Method var_getpropertyvalue_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object GetPropertyValue(System.Object, System.String, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertyValue(TypeReference pobj, TypeReference ppropertyName, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getpropertyvalue_0_3 == null)
				this.var_getpropertyvalue_0_3 = this.builderType.GetMethod("GetPropertyValue", true, pobj, ppropertyName, pbindingFlags);
			
			return this.var_getpropertyvalue_0_3.Import();
						
		}
						
		private Method var_implementsinterface_0_2;
				
		private Method var_implementsinterface_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean ImplementsInterface(System.Type, System.Type)<para/>
		/// Boolean ImplementsInterface(System.Reflection.TypeInfo, System.Type)<para/>
		/// </summary>
		public Method GetMethod_ImplementsInterface(TypeReference ptype, TypeReference ptypeOfInterface)
		{
						
						
			if(typeof(System.Type).AreEqual(ptype) && typeof(System.Type).AreEqual(ptypeOfInterface))
			{
				if(this.var_implementsinterface_0_2 == null)
					this.var_implementsinterface_0_2 = this.builderType.GetMethod("ImplementsInterface", true, ptype, ptypeOfInterface);
			
				return this.var_implementsinterface_0_2.Import();
			}
			
			if(typeof(System.Reflection.TypeInfo).AreEqual(ptype) && typeof(System.Type).AreEqual(ptypeOfInterface))
			{
				if(this.var_implementsinterface_1_2 == null)
					this.var_implementsinterface_1_2 = this.builderType.GetMethod("ImplementsInterface", true, ptype, ptypeOfInterface);
			
				return this.var_implementsinterface_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_iscollectionorlist_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsCollectionOrList(System.Type)<para/>
		/// </summary>
		public Method GetMethod_IsCollectionOrList()
		{
			if(this.var_iscollectionorlist_0_1 == null)
				this.var_iscollectionorlist_0_1 = this.builderType.GetMethod("IsCollectionOrList", 1, true);

			return this.var_iscollectionorlist_0_1.Import();
		}
						
		private Method var_isnullable_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsNullable(System.Type)<para/>
		/// </summary>
		public Method GetMethod_IsNullable()
		{
			if(this.var_isnullable_0_1 == null)
				this.var_isnullable_0_1 = this.builderType.GetMethod("IsNullable", 1, true);

			return this.var_isnullable_0_1.Import();
		}
						
		private Method var_matchesargumenttypes_0_2;
				
		private Method var_matchesargumenttypes_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean MatchesArgumentTypes(System.Reflection.MethodBase, System.Type[])<para/>
		/// Boolean MatchesArgumentTypes(System.Reflection.ParameterInfo[], System.Type[])<para/>
		/// </summary>
		public Method GetMethod_MatchesArgumentTypes(TypeReference pmethod, TypeReference pargumentTypes)
		{
						
						
			if(typeof(System.Reflection.MethodBase).AreEqual(pmethod) && typeof(System.Type[]).AreEqual(pargumentTypes))
			{
				if(this.var_matchesargumenttypes_0_2 == null)
					this.var_matchesargumenttypes_0_2 = this.builderType.GetMethod("MatchesArgumentTypes", true, pmethod, pargumentTypes);
			
				return this.var_matchesargumenttypes_0_2.Import();
			}
			
			if(typeof(System.Reflection.ParameterInfo[]).AreEqual(pmethod) && typeof(System.Type[]).AreEqual(pargumentTypes))
			{
				if(this.var_matchesargumenttypes_1_2 == null)
					this.var_matchesargumenttypes_1_2 = this.builderType.GetMethod("MatchesArgumentTypes", true, pmethod, pargumentTypes);
			
				return this.var_matchesargumenttypes_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.Factory"/>
    /// </summary>
    public partial class BuilderTypeFactory : TypeSystemExBase
	{
        internal BuilderTypeFactory(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeFactory value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeFactory value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_add_objectcreated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_ObjectCreated(System.EventHandler`1[Cauldron.Activator.FactoryObjectCreatedEventArgs])<para/>
		/// </summary>
		public Method GetMethod_add_ObjectCreated()
		{
			if(this.var_add_objectcreated_0_1 == null)
				this.var_add_objectcreated_0_1 = this.builderType.GetMethod("add_ObjectCreated", 1, true);

			return this.var_add_objectcreated_0_1.Import();
		}
						
		private Method var_remove_objectcreated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_ObjectCreated(System.EventHandler`1[Cauldron.Activator.FactoryObjectCreatedEventArgs])<para/>
		/// </summary>
		public Method GetMethod_remove_ObjectCreated()
		{
			if(this.var_remove_objectcreated_0_1 == null)
				this.var_remove_objectcreated_0_1 = this.builderType.GetMethod("remove_ObjectCreated", 1, true);

			return this.var_remove_objectcreated_0_1.Import();
		}
						
		private Method var_add_rebuilt_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Rebuilt(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Rebuilt()
		{
			if(this.var_add_rebuilt_0_1 == null)
				this.var_add_rebuilt_0_1 = this.builderType.GetMethod("add_Rebuilt", 1, true);

			return this.var_add_rebuilt_0_1.Import();
		}
						
		private Method var_remove_rebuilt_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Rebuilt(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Rebuilt()
		{
			if(this.var_remove_rebuilt_0_1 == null)
				this.var_remove_rebuilt_0_1 = this.builderType.GetMethod("remove_Rebuilt", 1, true);

			return this.var_remove_rebuilt_0_1.Import();
		}
						
		private Method var_get_canraiseexceptions_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_CanRaiseExceptions()<para/>
		/// </summary>
		public Method GetMethod_get_CanRaiseExceptions()
		{
			if(this.var_get_canraiseexceptions_0_0 == null)
				this.var_get_canraiseexceptions_0_0 = this.builderType.GetMethod("get_CanRaiseExceptions", 0, true);

			return this.var_get_canraiseexceptions_0_0.Import();
		}
						
		private Method var_set_canraiseexceptions_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_CanRaiseExceptions(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_CanRaiseExceptions()
		{
			if(this.var_set_canraiseexceptions_0_1 == null)
				this.var_set_canraiseexceptions_0_1 = this.builderType.GetMethod("set_CanRaiseExceptions", 1, true);

			return this.var_set_canraiseexceptions_0_1.Import();
		}
						
		private Method var_get_factorytypes_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[Cauldron.Activator.IFactoryTypeInfo] get_FactoryTypes()<para/>
		/// </summary>
		public Method GetMethod_get_FactoryTypes()
		{
			if(this.var_get_factorytypes_0_0 == null)
				this.var_get_factorytypes_0_0 = this.builderType.GetMethod("get_FactoryTypes", 0, true);

			return this.var_get_factorytypes_0_0.Import();
		}
						
		private Method var_get_registeredtypes_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[Cauldron.Activator.IFactoryTypeInfo] get_RegisteredTypes()<para/>
		/// </summary>
		public Method GetMethod_get_RegisteredTypes()
		{
			if(this.var_get_registeredtypes_0_0 == null)
				this.var_get_registeredtypes_0_0 = this.builderType.GetMethod("get_RegisteredTypes", 0, true);

			return this.var_get_registeredtypes_0_0.Import();
		}
						
		private Method var_get_resolvers_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.FactoryResolver get_Resolvers()<para/>
		/// </summary>
		public Method GetMethod_get_Resolvers()
		{
			if(this.var_get_resolvers_0_0 == null)
				this.var_get_resolvers_0_0 = this.builderType.GetMethod("get_Resolvers", 0, true);

			return this.var_get_resolvers_0_0.Import();
		}
						
		private Method var_create_0_2;
				
		private Method var_create_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object Create(System.String, System.Object[])<para/>
		/// System.Object Create(System.Type, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_Create(TypeReference pcontractName, TypeReference pparameters)
		{
						
						
			if(typeof(System.String).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_create_0_2 == null)
					this.var_create_0_2 = this.builderType.GetMethod("Create", true, pcontractName, pparameters);
			
				return this.var_create_0_2.Import();
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_create_1_2 == null)
					this.var_create_1_2 = this.builderType.GetMethod("Create", true, pcontractName, pparameters);
			
				return this.var_create_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_createfirst_0_2;
				
		private Method var_createfirst_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object CreateFirst(System.Type, System.Object[])<para/>
		/// System.Object CreateFirst(System.String, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateFirst(TypeReference pcontractType, TypeReference pparameters)
		{
						
						
			if(typeof(System.Type).AreEqual(pcontractType) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createfirst_0_2 == null)
					this.var_createfirst_0_2 = this.builderType.GetMethod("CreateFirst", true, pcontractType, pparameters);
			
				return this.var_createfirst_0_2.Import();
			}
			
			if(typeof(System.String).AreEqual(pcontractType) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createfirst_1_2 == null)
					this.var_createfirst_1_2 = this.builderType.GetMethod("CreateFirst", true, pcontractType, pparameters);
			
				return this.var_createfirst_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_createinstance_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object CreateInstance(System.Type, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateInstance()
		{
			if(this.var_createinstance_0_2 == null)
				this.var_createinstance_0_2 = this.builderType.GetMethod("CreateInstance", 2, true);

			return this.var_createinstance_0_2.Import();
		}
						
		private Method var_createmany_0_2;
				
		private Method var_createmany_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.IEnumerable CreateMany(System.String, System.Object[])<para/>
		/// System.Collections.IEnumerable CreateMany(System.Type, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateMany(TypeReference pcontractName, TypeReference pparameters)
		{
						
						
			if(typeof(System.String).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createmany_0_2 == null)
					this.var_createmany_0_2 = this.builderType.GetMethod("CreateMany", true, pcontractName, pparameters);
			
				return this.var_createmany_0_2.Import();
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createmany_1_2 == null)
					this.var_createmany_1_2 = this.builderType.GetMethod("CreateMany", true, pcontractName, pparameters);
			
				return this.var_createmany_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_createmanyordered_0_2;
				
		private Method var_createmanyordered_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.IEnumerable CreateManyOrdered(System.String, System.Object[])<para/>
		/// System.Collections.IEnumerable CreateManyOrdered(System.Type, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateManyOrdered(TypeReference pcontractName, TypeReference pparameters)
		{
						
						
			if(typeof(System.String).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createmanyordered_0_2 == null)
					this.var_createmanyordered_0_2 = this.builderType.GetMethod("CreateManyOrdered", true, pcontractName, pparameters);
			
				return this.var_createmanyordered_0_2.Import();
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createmanyordered_1_2 == null)
					this.var_createmanyordered_1_2 = this.builderType.GetMethod("CreateManyOrdered", true, pcontractName, pparameters);
			
				return this.var_createmanyordered_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_destroy_0_1;
				
		private Method var_destroy_1_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Destroy(System.Type)<para/>
		/// Void Destroy(System.String)<para/>
		/// </summary>
		public Method GetMethod_Destroy(TypeReference pcontractType)
		{
						
						
			if(typeof(System.Type).AreEqual(pcontractType))
			{
				if(this.var_destroy_0_1 == null)
					this.var_destroy_0_1 = this.builderType.GetMethod("Destroy", true, pcontractType);
			
				return this.var_destroy_0_1.Import();
			}
			
			if(typeof(System.String).AreEqual(pcontractType))
			{
				if(this.var_destroy_1_1 == null)
					this.var_destroy_1_1 = this.builderType.GetMethod("Destroy", true, pcontractType);
			
				return this.var_destroy_1_1.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_destroy_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Destroy()<para/>
		/// </summary>
		public Method GetMethod_Destroy()
		{
						
			if(this.var_destroy_0_0 == null)
				this.var_destroy_0_0 = this.builderType.GetMethod("Destroy", true);

			return this.var_destroy_0_0.Import();
						
						
		}
						
		private Method var_getfactorytypeinfo_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.IFactoryTypeInfo GetFactoryTypeInfo(System.String)<para/>
		/// </summary>
		public Method GetMethod_GetFactoryTypeInfo()
		{
			if(this.var_getfactorytypeinfo_0_1 == null)
				this.var_getfactorytypeinfo_0_1 = this.builderType.GetMethod("GetFactoryTypeInfo", 1, true);

			return this.var_getfactorytypeinfo_0_1.Import();
		}
						
		private Method var_getfactorytypeinfofirst_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.IFactoryTypeInfo GetFactoryTypeInfoFirst(System.String)<para/>
		/// </summary>
		public Method GetMethod_GetFactoryTypeInfoFirst()
		{
			if(this.var_getfactorytypeinfofirst_0_1 == null)
				this.var_getfactorytypeinfofirst_0_1 = this.builderType.GetMethod("GetFactoryTypeInfoFirst", 1, true);

			return this.var_getfactorytypeinfofirst_0_1.Import();
		}
						
		private Method var_hascontract_0_1;
				
		private Method var_hascontract_1_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean HasContract(System.String)<para/>
		/// Boolean HasContract(System.Type)<para/>
		/// </summary>
		public Method GetMethod_HasContract(TypeReference pcontractName)
		{
						
						
			if(typeof(System.String).AreEqual(pcontractName))
			{
				if(this.var_hascontract_0_1 == null)
					this.var_hascontract_0_1 = this.builderType.GetMethod("HasContract", true, pcontractName);
			
				return this.var_hascontract_0_1.Import();
			}
			
			if(typeof(System.Type).AreEqual(pcontractName))
			{
				if(this.var_hascontract_1_1 == null)
					this.var_hascontract_1_1 = this.builderType.GetMethod("HasContract", true, pcontractName);
			
				return this.var_hascontract_1_1.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_onobjectcreation_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnObjectCreation(System.Object, Cauldron.Activator.IFactoryTypeInfo)<para/>
		/// </summary>
		public Method GetMethod_OnObjectCreation()
		{
			if(this.var_onobjectcreation_0_2 == null)
				this.var_onobjectcreation_0_2 = this.builderType.GetMethod("OnObjectCreation", 2, true);

			return this.var_onobjectcreation_0_2.Import();
		}
						
		private Method var_removetype_0_2;
				
		private Method var_removetype_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void RemoveType(System.Type, System.Type)<para/>
		/// Void RemoveType(System.String, System.Type)<para/>
		/// </summary>
		public Method GetMethod_RemoveType(TypeReference pcontractType, TypeReference ptype)
		{
						
						
			if(typeof(System.Type).AreEqual(pcontractType) && typeof(System.Type).AreEqual(ptype))
			{
				if(this.var_removetype_0_2 == null)
					this.var_removetype_0_2 = this.builderType.GetMethod("RemoveType", true, pcontractType, ptype);
			
				return this.var_removetype_0_2.Import();
			}
			
			if(typeof(System.String).AreEqual(pcontractType) && typeof(System.Type).AreEqual(ptype))
			{
				if(this.var_removetype_1_2 == null)
					this.var_removetype_1_2 = this.builderType.GetMethod("RemoveType", true, pcontractType, ptype);
			
				return this.var_removetype_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor()<para/>
		/// </summary>
		public Method GetMethod_ctor()
		{
			if(this.var_ctor_0_0 == null)
				this.var_ctor_0_0 = this.builderType.GetMethod(".ctor", 0, true);

			return this.var_ctor_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.Factory{T}"/>
    /// </summary>
    public partial class BuilderTypeFactory1 : TypeSystemExBase
	{
        internal BuilderTypeFactory1(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeFactory1 value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeFactory1 value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.GenericComponentAttribute"/>
    /// </summary>
    public partial class BuilderTypeGenericComponentAttribute : TypeSystemExBase
	{
        internal BuilderTypeGenericComponentAttribute(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeGenericComponentAttribute value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeGenericComponentAttribute value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_get_contractname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_ContractName()<para/>
		/// </summary>
		public Method GetMethod_get_ContractName()
		{
			if(this.var_get_contractname_0_0 == null)
				this.var_get_contractname_0_0 = this.builderType.GetMethod("get_ContractName", 0, true);

			return this.var_get_contractname_0_0.Import();
		}
						
		private Method var_get_invokeonobjectcreationevent_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_InvokeOnObjectCreationEvent()<para/>
		/// </summary>
		public Method GetMethod_get_InvokeOnObjectCreationEvent()
		{
			if(this.var_get_invokeonobjectcreationevent_0_0 == null)
				this.var_get_invokeonobjectcreationevent_0_0 = this.builderType.GetMethod("get_InvokeOnObjectCreationEvent", 0, true);

			return this.var_get_invokeonobjectcreationevent_0_0.Import();
		}
						
		private Method var_set_invokeonobjectcreationevent_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_InvokeOnObjectCreationEvent(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_InvokeOnObjectCreationEvent()
		{
			if(this.var_set_invokeonobjectcreationevent_0_1 == null)
				this.var_set_invokeonobjectcreationevent_0_1 = this.builderType.GetMethod("set_InvokeOnObjectCreationEvent", 1, true);

			return this.var_set_invokeonobjectcreationevent_0_1.Import();
		}
						
		private Method var_get_policy_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.FactoryCreationPolicy get_Policy()<para/>
		/// </summary>
		public Method GetMethod_get_Policy()
		{
			if(this.var_get_policy_0_0 == null)
				this.var_get_policy_0_0 = this.builderType.GetMethod("get_Policy", 0, true);

			return this.var_get_policy_0_0.Import();
		}
						
		private Method var_get_priority_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// UInt32 get_Priority()<para/>
		/// </summary>
		public Method GetMethod_get_Priority()
		{
			if(this.var_get_priority_0_0 == null)
				this.var_get_priority_0_0 = this.builderType.GetMethod("get_Priority", 0, true);

			return this.var_get_priority_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_get_typeid_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_TypeId()<para/>
		/// </summary>
		public Method GetMethod_get_TypeId()
		{
			if(this.var_get_typeid_0_0 == null)
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", 0, true);

			return this.var_get_typeid_0_0.Import();
		}
						
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match()
		{
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", 1, true);

			return this.var_match_0_1.Import();
		}
						
		private Method var_isdefaultattribute_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsDefaultAttribute()<para/>
		/// </summary>
		public Method GetMethod_IsDefaultAttribute()
		{
			if(this.var_isdefaultattribute_0_0 == null)
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", 0, true);

			return this.var_isdefaultattribute_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_2;
				
		private Method var_ctor_1_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor(System.Type, System.Type)<para/>
		/// Void .ctor(System.Type, System.String)<para/>
		/// </summary>
		public Method GetMethod_ctor(TypeReference ptype, TypeReference pcontractType)
		{
						
						
			if(typeof(System.Type).AreEqual(ptype) && typeof(System.Type).AreEqual(pcontractType))
			{
				if(this.var_ctor_0_2 == null)
					this.var_ctor_0_2 = this.builderType.GetMethod(".ctor", true, ptype, pcontractType);
			
				return this.var_ctor_0_2.Import();
			}
			
			if(typeof(System.Type).AreEqual(ptype) && typeof(System.String).AreEqual(pcontractType))
			{
				if(this.var_ctor_1_2 == null)
					this.var_ctor_1_2 = this.builderType.GetMethod(".ctor", true, ptype, pcontractType);
			
				return this.var_ctor_1_2.Import();
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.IConstructorInterceptor"/>
    /// </summary>
    public partial class BuilderTypeIConstructorInterceptor : TypeSystemExBase
	{
        internal BuilderTypeIConstructorInterceptor(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIConstructorInterceptor value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIConstructorInterceptor value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_onbeforeinitialization_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnBeforeInitialization(System.Type, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnBeforeInitialization()
		{
			if(this.var_onbeforeinitialization_0_3 == null)
				this.var_onbeforeinitialization_0_3 = this.builderType.GetMethod("OnBeforeInitialization", 3, true);

			return this.var_onbeforeinitialization_0_3.Import();
		}
						
		private Method var_onenter_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnEnter(System.Type, System.Object, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnEnter()
		{
			if(this.var_onenter_0_4 == null)
				this.var_onenter_0_4 = this.builderType.GetMethod("OnEnter", 4, true);

			return this.var_onenter_0_4.Import();
		}
						
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException()
		{
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", 1, true);

			return this.var_onexception_0_1.Import();
		}
						
		private Method var_onexit_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnExit()<para/>
		/// </summary>
		public Method GetMethod_OnExit()
		{
			if(this.var_onexit_0_0 == null)
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", 0, true);

			return this.var_onexit_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Core.IDisposableObject"/>
    /// </summary>
    public partial class BuilderTypeIDisposableObject : TypeSystemExBase
	{
        internal BuilderTypeIDisposableObject(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIDisposableObject value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIDisposableObject value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_add_disposed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Disposed(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Disposed()
		{
			if(this.var_add_disposed_0_1 == null)
				this.var_add_disposed_0_1 = this.builderType.GetMethod("add_Disposed", 1, true);

			return this.var_add_disposed_0_1.Import();
		}
						
		private Method var_remove_disposed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Disposed(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Disposed()
		{
			if(this.var_remove_disposed_0_1 == null)
				this.var_remove_disposed_0_1 = this.builderType.GetMethod("remove_Disposed", 1, true);

			return this.var_remove_disposed_0_1.Import();
		}
						
		private Method var_get_isdisposed_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_IsDisposed()<para/>
		/// </summary>
		public Method GetMethod_get_IsDisposed()
		{
			if(this.var_get_isdisposed_0_0 == null)
				this.var_get_isdisposed_0_0 = this.builderType.GetMethod("get_IsDisposed", 0, true);

			return this.var_get_isdisposed_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.IFactoryExtension"/>
    /// </summary>
    public partial class BuilderTypeIFactoryExtension : TypeSystemExBase
	{
        internal BuilderTypeIFactoryExtension(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIFactoryExtension value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIFactoryExtension value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_initialize_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Initialize(System.Collections.Generic.IEnumerable`1[Cauldron.Activator.IFactoryTypeInfo])<para/>
		/// </summary>
		public Method GetMethod_Initialize()
		{
			if(this.var_initialize_0_1 == null)
				this.var_initialize_0_1 = this.builderType.GetMethod("Initialize", 1, true);

			return this.var_initialize_0_1.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Activator.IFactoryTypeInfo"/>
    /// </summary>
    public partial class BuilderTypeIFactoryTypeInfo : TypeSystemExBase
	{
        internal BuilderTypeIFactoryTypeInfo(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIFactoryTypeInfo value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIFactoryTypeInfo value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_get_contractname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_ContractName()<para/>
		/// </summary>
		public Method GetMethod_get_ContractName()
		{
			if(this.var_get_contractname_0_0 == null)
				this.var_get_contractname_0_0 = this.builderType.GetMethod("get_ContractName", 0, true);

			return this.var_get_contractname_0_0.Import();
		}
						
		private Method var_get_contracttype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type get_ContractType()<para/>
		/// </summary>
		public Method GetMethod_get_ContractType()
		{
			if(this.var_get_contracttype_0_0 == null)
				this.var_get_contracttype_0_0 = this.builderType.GetMethod("get_ContractType", 0, true);

			return this.var_get_contracttype_0_0.Import();
		}
						
		private Method var_get_creationpolicy_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.FactoryCreationPolicy get_CreationPolicy()<para/>
		/// </summary>
		public Method GetMethod_get_CreationPolicy()
		{
			if(this.var_get_creationpolicy_0_0 == null)
				this.var_get_creationpolicy_0_0 = this.builderType.GetMethod("get_CreationPolicy", 0, true);

			return this.var_get_creationpolicy_0_0.Import();
		}
						
		private Method var_get_instance_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_Instance()<para/>
		/// </summary>
		public Method GetMethod_get_Instance()
		{
			if(this.var_get_instance_0_0 == null)
				this.var_get_instance_0_0 = this.builderType.GetMethod("get_Instance", 0, true);

			return this.var_get_instance_0_0.Import();
		}
						
		private Method var_set_instance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_Instance(System.Object)<para/>
		/// </summary>
		public Method GetMethod_set_Instance()
		{
			if(this.var_set_instance_0_1 == null)
				this.var_set_instance_0_1 = this.builderType.GetMethod("set_Instance", 1, true);

			return this.var_set_instance_0_1.Import();
		}
						
		private Method var_get_priority_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// UInt32 get_Priority()<para/>
		/// </summary>
		public Method GetMethod_get_Priority()
		{
			if(this.var_get_priority_0_0 == null)
				this.var_get_priority_0_0 = this.builderType.GetMethod("get_Priority", 0, true);

			return this.var_get_priority_0_0.Import();
		}
						
		private Method var_get_type_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type get_Type()<para/>
		/// </summary>
		public Method GetMethod_get_Type()
		{
			if(this.var_get_type_0_0 == null)
				this.var_get_type_0_0 = this.builderType.GetMethod("get_Type", 0, true);

			return this.var_get_type_0_0.Import();
		}
						
		private Method var_createinstance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object CreateInstance(System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateInstance(TypeReference parguments)
		{
						
						
			if(this.var_createinstance_0_1 == null)
				this.var_createinstance_0_1 = this.builderType.GetMethod("CreateInstance", true, parguments);
			
			return this.var_createinstance_0_1.Import();
						
		}
						
		private Method var_createinstance_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object CreateInstance()<para/>
		/// </summary>
		public Method GetMethod_CreateInstance()
		{
						
			if(this.var_createinstance_0_0 == null)
				this.var_createinstance_0_0 = this.builderType.GetMethod("CreateInstance", true);

			return this.var_createinstance_0_0.Import();
						
						
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.IMethodInterceptor"/>
    /// </summary>
    public partial class BuilderTypeIMethodInterceptor : TypeSystemExBase
	{
        internal BuilderTypeIMethodInterceptor(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIMethodInterceptor value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIMethodInterceptor value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_onenter_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnEnter(System.Type, System.Object, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnEnter()
		{
			if(this.var_onenter_0_4 == null)
				this.var_onenter_0_4 = this.builderType.GetMethod("OnEnter", 4, true);

			return this.var_onenter_0_4.Import();
		}
						
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException()
		{
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", 1, true);

			return this.var_onexception_0_1.Import();
		}
						
		private Method var_onexit_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnExit()<para/>
		/// </summary>
		public Method GetMethod_OnExit()
		{
			if(this.var_onexit_0_0 == null)
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", 0, true);

			return this.var_onexit_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.InterceptionRuleAttribute"/>
    /// </summary>
    public partial class BuilderTypeInterceptionRuleAttribute : TypeSystemExBase
	{
        internal BuilderTypeInterceptionRuleAttribute(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeInterceptionRuleAttribute value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeInterceptionRuleAttribute value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_get_typeid_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_TypeId()<para/>
		/// </summary>
		public Method GetMethod_get_TypeId()
		{
			if(this.var_get_typeid_0_0 == null)
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", 0, true);

			return this.var_get_typeid_0_0.Import();
		}
						
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match()
		{
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", 1, true);

			return this.var_match_0_1.Import();
		}
						
		private Method var_isdefaultattribute_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsDefaultAttribute()<para/>
		/// </summary>
		public Method GetMethod_IsDefaultAttribute()
		{
			if(this.var_isdefaultattribute_0_0 == null)
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", 0, true);

			return this.var_isdefaultattribute_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.InterceptorOptionsAttribute"/>
    /// </summary>
    public partial class BuilderTypeInterceptorOptionsAttribute : TypeSystemExBase
	{
        internal BuilderTypeInterceptorOptionsAttribute(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeInterceptorOptionsAttribute value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeInterceptorOptionsAttribute value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_get_alwayscreatenewinstance_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_AlwaysCreateNewInstance()<para/>
		/// </summary>
		public Method GetMethod_get_AlwaysCreateNewInstance()
		{
			if(this.var_get_alwayscreatenewinstance_0_0 == null)
				this.var_get_alwayscreatenewinstance_0_0 = this.builderType.GetMethod("get_AlwaysCreateNewInstance", 0, true);

			return this.var_get_alwayscreatenewinstance_0_0.Import();
		}
						
		private Method var_set_alwayscreatenewinstance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_AlwaysCreateNewInstance(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_AlwaysCreateNewInstance()
		{
			if(this.var_set_alwayscreatenewinstance_0_1 == null)
				this.var_set_alwayscreatenewinstance_0_1 = this.builderType.GetMethod("set_AlwaysCreateNewInstance", 1, true);

			return this.var_set_alwayscreatenewinstance_0_1.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_get_typeid_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_TypeId()<para/>
		/// </summary>
		public Method GetMethod_get_TypeId()
		{
			if(this.var_get_typeid_0_0 == null)
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", 0, true);

			return this.var_get_typeid_0_0.Import();
		}
						
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match()
		{
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", 1, true);

			return this.var_match_0_1.Import();
		}
						
		private Method var_isdefaultattribute_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsDefaultAttribute()<para/>
		/// </summary>
		public Method GetMethod_IsDefaultAttribute()
		{
			if(this.var_isdefaultattribute_0_0 == null)
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", 0, true);

			return this.var_isdefaultattribute_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor()<para/>
		/// </summary>
		public Method GetMethod_ctor()
		{
			if(this.var_ctor_0_0 == null)
				this.var_ctor_0_0 = this.builderType.GetMethod(".ctor", 0, true);

			return this.var_ctor_0_0.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.IPropertyGetterInterceptor"/>
    /// </summary>
    public partial class BuilderTypeIPropertyGetterInterceptor : TypeSystemExBase
	{
        internal BuilderTypeIPropertyGetterInterceptor(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIPropertyGetterInterceptor value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIPropertyGetterInterceptor value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException()
		{
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", 1, true);

			return this.var_onexception_0_1.Import();
		}
						
		private Method var_onexit_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnExit()<para/>
		/// </summary>
		public Method GetMethod_OnExit()
		{
			if(this.var_onexit_0_0 == null)
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", 0, true);

			return this.var_onexit_0_0.Import();
		}
						
		private Method var_onget_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnGet(Cauldron.Interception.PropertyInterceptionInfo, System.Object)<para/>
		/// </summary>
		public Method GetMethod_OnGet()
		{
			if(this.var_onget_0_2 == null)
				this.var_onget_0_2 = this.builderType.GetMethod("OnGet", 2, true);

			return this.var_onget_0_2.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.IPropertyInterceptorInitialize"/>
    /// </summary>
    public partial class BuilderTypeIPropertyInterceptorInitialize : TypeSystemExBase
	{
        internal BuilderTypeIPropertyInterceptorInitialize(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIPropertyInterceptorInitialize value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIPropertyInterceptorInitialize value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_oninitialize_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnInitialize(Cauldron.Interception.PropertyInterceptionInfo, System.Object)<para/>
		/// </summary>
		public Method GetMethod_OnInitialize()
		{
			if(this.var_oninitialize_0_2 == null)
				this.var_oninitialize_0_2 = this.builderType.GetMethod("OnInitialize", 2, true);

			return this.var_oninitialize_0_2.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.IPropertySetterInterceptor"/>
    /// </summary>
    public partial class BuilderTypeIPropertySetterInterceptor : TypeSystemExBase
	{
        internal BuilderTypeIPropertySetterInterceptor(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeIPropertySetterInterceptor value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIPropertySetterInterceptor value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException()
		{
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", 1, true);

			return this.var_onexception_0_1.Import();
		}
						
		private Method var_onexit_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnExit()<para/>
		/// </summary>
		public Method GetMethod_OnExit()
		{
			if(this.var_onexit_0_0 == null)
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", 0, true);

			return this.var_onexit_0_0.Import();
		}
						
		private Method var_onset_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnSet(Cauldron.Interception.PropertyInterceptionInfo, System.Object, System.Object)<para/>
		/// </summary>
		public Method GetMethod_OnSet()
		{
			if(this.var_onset_0_3 == null)
				this.var_onset_0_3 = this.builderType.GetMethod("OnSet", 3, true);

			return this.var_onset_0_3.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.ISimpleMethodInterceptor"/>
    /// </summary>
    public partial class BuilderTypeISimpleMethodInterceptor : TypeSystemExBase
	{
        internal BuilderTypeISimpleMethodInterceptor(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeISimpleMethodInterceptor value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeISimpleMethodInterceptor value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_onenter_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnEnter(System.Type, System.Object, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnEnter()
		{
			if(this.var_onenter_0_4 == null)
				this.var_onenter_0_4 = this.builderType.GetMethod("OnEnter", 4, true);

			return this.var_onenter_0_4.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.ISyncRoot"/>
    /// </summary>
    public partial class BuilderTypeISyncRoot : TypeSystemExBase
	{
        internal BuilderTypeISyncRoot(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeISyncRoot value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeISyncRoot value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_get_syncroot_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_SyncRoot()<para/>
		/// </summary>
		public Method GetMethod_get_SyncRoot()
		{
			if(this.var_get_syncroot_0_0 == null)
				this.var_get_syncroot_0_0 = this.builderType.GetMethod("get_SyncRoot", 0, true);

			return this.var_get_syncroot_0_0.Import();
		}
						
		private Method var_set_syncroot_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_SyncRoot(System.Object)<para/>
		/// </summary>
		public Method GetMethod_set_SyncRoot()
		{
			if(this.var_set_syncroot_0_1 == null)
				this.var_set_syncroot_0_1 = this.builderType.GetMethod("set_SyncRoot", 1, true);

			return this.var_set_syncroot_0_1.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="Cauldron.Interception.PropertyInterceptionInfo"/>
    /// </summary>
    public partial class BuilderTypePropertyInterceptionInfo : TypeSystemExBase
	{
        internal BuilderTypePropertyInterceptionInfo(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypePropertyInterceptionInfo value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypePropertyInterceptionInfo value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_get_childtype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type get_ChildType()<para/>
		/// </summary>
		public Method GetMethod_get_ChildType()
		{
			if(this.var_get_childtype_0_0 == null)
				this.var_get_childtype_0_0 = this.builderType.GetMethod("get_ChildType", 0, true);

			return this.var_get_childtype_0_0.Import();
		}
						
		private Method var_get_declaringtype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type get_DeclaringType()<para/>
		/// </summary>
		public Method GetMethod_get_DeclaringType()
		{
			if(this.var_get_declaringtype_0_0 == null)
				this.var_get_declaringtype_0_0 = this.builderType.GetMethod("get_DeclaringType", 0, true);

			return this.var_get_declaringtype_0_0.Import();
		}
						
		private Method var_get_getmethod_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.MethodBase get_GetMethod()<para/>
		/// </summary>
		public Method GetMethod_get_GetMethod()
		{
			if(this.var_get_getmethod_0_0 == null)
				this.var_get_getmethod_0_0 = this.builderType.GetMethod("get_GetMethod", 0, true);

			return this.var_get_getmethod_0_0.Import();
		}
						
		private Method var_get_instance_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_Instance()<para/>
		/// </summary>
		public Method GetMethod_get_Instance()
		{
			if(this.var_get_instance_0_0 == null)
				this.var_get_instance_0_0 = this.builderType.GetMethod("get_Instance", 0, true);

			return this.var_get_instance_0_0.Import();
		}
						
		private Method var_get_propertyname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_PropertyName()<para/>
		/// </summary>
		public Method GetMethod_get_PropertyName()
		{
			if(this.var_get_propertyname_0_0 == null)
				this.var_get_propertyname_0_0 = this.builderType.GetMethod("get_PropertyName", 0, true);

			return this.var_get_propertyname_0_0.Import();
		}
						
		private Method var_get_propertytype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type get_PropertyType()<para/>
		/// </summary>
		public Method GetMethod_get_PropertyType()
		{
			if(this.var_get_propertytype_0_0 == null)
				this.var_get_propertytype_0_0 = this.builderType.GetMethod("get_PropertyType", 0, true);

			return this.var_get_propertytype_0_0.Import();
		}
						
		private Method var_get_setmethod_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.MethodBase get_SetMethod()<para/>
		/// </summary>
		public Method GetMethod_get_SetMethod()
		{
			if(this.var_get_setmethod_0_0 == null)
				this.var_get_setmethod_0_0 = this.builderType.GetMethod("get_SetMethod", 0, true);

			return this.var_get_setmethod_0_0.Import();
		}
						
		private Method var_setvalue_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void SetValue(System.Object)<para/>
		/// </summary>
		public Method GetMethod_SetValue()
		{
			if(this.var_setvalue_0_1 == null)
				this.var_setvalue_0_1 = this.builderType.GetMethod("SetValue", 1, true);

			return this.var_setvalue_0_1.Import();
		}
						
		private Method var_topropertyinfo_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.PropertyInfo ToPropertyInfo()<para/>
		/// </summary>
		public Method GetMethod_ToPropertyInfo()
		{
			if(this.var_topropertyinfo_0_0 == null)
				this.var_topropertyinfo_0_0 = this.builderType.GetMethod("ToPropertyInfo", 0, true);

			return this.var_topropertyinfo_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_7;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor(System.Reflection.MethodBase, System.Reflection.MethodBase, System.String, System.Type, System.Object, System.Type, System.Action`1[System.Object])<para/>
		/// </summary>
		public Method GetMethod_ctor()
		{
			if(this.var_ctor_0_7 == null)
				this.var_ctor_0_7 = this.builderType.GetMethod(".ctor", 7, true);

			return this.var_ctor_0_7.Import();
		}
					}

			
    /// <summary>
    /// Provides a wrapper class for <see cref="System.Windows.ResourceDictionary"/>
    /// </summary>
    public partial class BuilderTypeResourceDictionary : TypeSystemExBase
	{
        internal BuilderTypeResourceDictionary(BuilderType builderType) : base(builderType)
		{
		}

		/// <exclude />
		public static implicit operator BuilderType(BuilderTypeResourceDictionary value) => value.builderType.Import();
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeResourceDictionary value) => Builder.Current.Import((TypeReference)value.builderType);
						
				
		private Method var_copyto_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void CopyTo(System.Collections.DictionaryEntry[], Int32)<para/>
		/// </summary>
		public Method GetMethod_CopyTo()
		{
			if(this.var_copyto_0_2 == null)
				this.var_copyto_0_2 = this.builderType.GetMethod("CopyTo", 2, true);

			return this.var_copyto_0_2.Import();
		}
						
		private Method var_get_mergeddictionaries_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.ObjectModel.Collection`1[System.Windows.ResourceDictionary] get_MergedDictionaries()<para/>
		/// </summary>
		public Method GetMethod_get_MergedDictionaries()
		{
			if(this.var_get_mergeddictionaries_0_0 == null)
				this.var_get_mergeddictionaries_0_0 = this.builderType.GetMethod("get_MergedDictionaries", 0, true);

			return this.var_get_mergeddictionaries_0_0.Import();
		}
						
		private Method var_get_source_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Uri get_Source()<para/>
		/// </summary>
		public Method GetMethod_get_Source()
		{
			if(this.var_get_source_0_0 == null)
				this.var_get_source_0_0 = this.builderType.GetMethod("get_Source", 0, true);

			return this.var_get_source_0_0.Import();
		}
						
		private Method var_set_source_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_Source(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_set_Source()
		{
			if(this.var_set_source_0_1 == null)
				this.var_set_source_0_1 = this.builderType.GetMethod("set_Source", 1, true);

			return this.var_set_source_0_1.Import();
		}
						
		private Method var_registername_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void RegisterName(System.String, System.Object)<para/>
		/// </summary>
		public Method GetMethod_RegisterName()
		{
			if(this.var_registername_0_2 == null)
				this.var_registername_0_2 = this.builderType.GetMethod("RegisterName", 2, true);

			return this.var_registername_0_2.Import();
		}
						
		private Method var_unregistername_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void UnregisterName(System.String)<para/>
		/// </summary>
		public Method GetMethod_UnregisterName()
		{
			if(this.var_unregistername_0_1 == null)
				this.var_unregistername_0_1 = this.builderType.GetMethod("UnregisterName", 1, true);

			return this.var_unregistername_0_1.Import();
		}
						
		private Method var_findname_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object FindName(System.String)<para/>
		/// </summary>
		public Method GetMethod_FindName()
		{
			if(this.var_findname_0_1 == null)
				this.var_findname_0_1 = this.builderType.GetMethod("FindName", 1, true);

			return this.var_findname_0_1.Import();
		}
						
		private Method var_get_isfixedsize_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_IsFixedSize()<para/>
		/// </summary>
		public Method GetMethod_get_IsFixedSize()
		{
			if(this.var_get_isfixedsize_0_0 == null)
				this.var_get_isfixedsize_0_0 = this.builderType.GetMethod("get_IsFixedSize", 0, true);

			return this.var_get_isfixedsize_0_0.Import();
		}
						
		private Method var_get_isreadonly_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_IsReadOnly()<para/>
		/// </summary>
		public Method GetMethod_get_IsReadOnly()
		{
			if(this.var_get_isreadonly_0_0 == null)
				this.var_get_isreadonly_0_0 = this.builderType.GetMethod("get_IsReadOnly", 0, true);

			return this.var_get_isreadonly_0_0.Import();
		}
						
		private Method var_get_invalidatesimplicitdatatemplateresources_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_InvalidatesImplicitDataTemplateResources()<para/>
		/// </summary>
		public Method GetMethod_get_InvalidatesImplicitDataTemplateResources()
		{
			if(this.var_get_invalidatesimplicitdatatemplateresources_0_0 == null)
				this.var_get_invalidatesimplicitdatatemplateresources_0_0 = this.builderType.GetMethod("get_InvalidatesImplicitDataTemplateResources", 0, true);

			return this.var_get_invalidatesimplicitdatatemplateresources_0_0.Import();
		}
						
		private Method var_set_invalidatesimplicitdatatemplateresources_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_InvalidatesImplicitDataTemplateResources(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_InvalidatesImplicitDataTemplateResources()
		{
			if(this.var_set_invalidatesimplicitdatatemplateresources_0_1 == null)
				this.var_set_invalidatesimplicitdatatemplateresources_0_1 = this.builderType.GetMethod("set_InvalidatesImplicitDataTemplateResources", 1, true);

			return this.var_set_invalidatesimplicitdatatemplateresources_0_1.Import();
		}
						
		private Method var_get_item_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_Item(System.Object)<para/>
		/// </summary>
		public Method GetMethod_get_Item()
		{
			if(this.var_get_item_0_1 == null)
				this.var_get_item_0_1 = this.builderType.GetMethod("get_Item", 1, true);

			return this.var_get_item_0_1.Import();
		}
						
		private Method var_set_item_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_Item(System.Object, System.Object)<para/>
		/// </summary>
		public Method GetMethod_set_Item()
		{
			if(this.var_set_item_0_2 == null)
				this.var_set_item_0_2 = this.builderType.GetMethod("set_Item", 2, true);

			return this.var_set_item_0_2.Import();
		}
						
		private Method var_get_deferrablecontent_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.DeferrableContent get_DeferrableContent()<para/>
		/// </summary>
		public Method GetMethod_get_DeferrableContent()
		{
			if(this.var_get_deferrablecontent_0_0 == null)
				this.var_get_deferrablecontent_0_0 = this.builderType.GetMethod("get_DeferrableContent", 0, true);

			return this.var_get_deferrablecontent_0_0.Import();
		}
						
		private Method var_set_deferrablecontent_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_DeferrableContent(System.Windows.DeferrableContent)<para/>
		/// </summary>
		public Method GetMethod_set_DeferrableContent()
		{
			if(this.var_set_deferrablecontent_0_1 == null)
				this.var_set_deferrablecontent_0_1 = this.builderType.GetMethod("set_DeferrableContent", 1, true);

			return this.var_set_deferrablecontent_0_1.Import();
		}
						
		private Method var_get_keys_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.ICollection get_Keys()<para/>
		/// </summary>
		public Method GetMethod_get_Keys()
		{
			if(this.var_get_keys_0_0 == null)
				this.var_get_keys_0_0 = this.builderType.GetMethod("get_Keys", 0, true);

			return this.var_get_keys_0_0.Import();
		}
						
		private Method var_get_values_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.ICollection get_Values()<para/>
		/// </summary>
		public Method GetMethod_get_Values()
		{
			if(this.var_get_values_0_0 == null)
				this.var_get_values_0_0 = this.builderType.GetMethod("get_Values", 0, true);

			return this.var_get_values_0_0.Import();
		}
						
		private Method var_add_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Add(System.Object, System.Object)<para/>
		/// </summary>
		public Method GetMethod_Add()
		{
			if(this.var_add_0_2 == null)
				this.var_add_0_2 = this.builderType.GetMethod("Add", 2, true);

			return this.var_add_0_2.Import();
		}
						
		private Method var_clear_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Clear()<para/>
		/// </summary>
		public Method GetMethod_Clear()
		{
			if(this.var_clear_0_0 == null)
				this.var_clear_0_0 = this.builderType.GetMethod("Clear", 0, true);

			return this.var_clear_0_0.Import();
		}
						
		private Method var_contains_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Contains(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Contains()
		{
			if(this.var_contains_0_1 == null)
				this.var_contains_0_1 = this.builderType.GetMethod("Contains", 1, true);

			return this.var_contains_0_1.Import();
		}
						
		private Method var_getenumerator_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.IDictionaryEnumerator GetEnumerator()<para/>
		/// </summary>
		public Method GetMethod_GetEnumerator()
		{
			if(this.var_getenumerator_0_0 == null)
				this.var_getenumerator_0_0 = this.builderType.GetMethod("GetEnumerator", 0, true);

			return this.var_getenumerator_0_0.Import();
		}
						
		private Method var_remove_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Remove(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Remove()
		{
			if(this.var_remove_0_1 == null)
				this.var_remove_0_1 = this.builderType.GetMethod("Remove", 1, true);

			return this.var_remove_0_1.Import();
		}
						
		private Method var_get_count_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 get_Count()<para/>
		/// </summary>
		public Method GetMethod_get_Count()
		{
			if(this.var_get_count_0_0 == null)
				this.var_get_count_0_0 = this.builderType.GetMethod("get_Count", 0, true);

			return this.var_get_count_0_0.Import();
		}
						
		private Method var_begininit_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void BeginInit()<para/>
		/// </summary>
		public Method GetMethod_BeginInit()
		{
			if(this.var_begininit_0_0 == null)
				this.var_begininit_0_0 = this.builderType.GetMethod("BeginInit", 0, true);

			return this.var_begininit_0_0.Import();
		}
						
		private Method var_endinit_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void EndInit()<para/>
		/// </summary>
		public Method GetMethod_EndInit()
		{
			if(this.var_endinit_0_0 == null)
				this.var_endinit_0_0 = this.builderType.GetMethod("EndInit", 0, true);

			return this.var_endinit_0_0.Import();
		}
						
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", 0, true);

			return this.var_tostring_0_0.Import();
		}
						
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals()
		{
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", 1, true);

			return this.var_equals_0_1.Import();
		}
						
		private Method var_gethashcode_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Int32 GetHashCode()<para/>
		/// </summary>
		public Method GetMethod_GetHashCode()
		{
			if(this.var_gethashcode_0_0 == null)
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", 0, true);

			return this.var_gethashcode_0_0.Import();
		}
						
		private Method var_gettype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetType()<para/>
		/// </summary>
		public Method GetMethod_GetType()
		{
			if(this.var_gettype_0_0 == null)
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", 0, true);

			return this.var_gettype_0_0.Import();
		}
						
		private Method var_ctor_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void .ctor()<para/>
		/// </summary>
		public Method GetMethod_ctor()
		{
			if(this.var_ctor_0_0 == null)
				this.var_ctor_0_0 = this.builderType.GetMethod(".ctor", 0, true);

			return this.var_ctor_0_0.Import();
		}
					}

	}


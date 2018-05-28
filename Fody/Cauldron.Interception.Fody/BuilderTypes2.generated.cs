
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
		
	}

	/// <exclude />
	public class TypeSystemExBase 
	{
		/// <exclude />
		internal BuilderType builderType;

        internal TypeSystemExBase(BuilderType builderType)
		{
			this.builderType = builderType;
		}
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
		public static implicit operator BuilderType(BuilderTypeApplication value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeApplication value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
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

			return this.var_run_0_0;
						
						
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
				this.var_run_0_1 = this.builderType.GetMethod("Run", true, pwindow).Import();
			
			return this.var_run_0_1;
						
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

			return this.var_shutdown_0_0;
						
						
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
				this.var_shutdown_0_1 = this.builderType.GetMethod("Shutdown", true, pexitCode).Import();
			
			return this.var_shutdown_0_1;
						
		}
				
		private Method var_findresource_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object FindResource(System.Object)<para/>
		/// </summary>
		public Method GetMethod_FindResource(TypeReference presourceKey)
		{
						
						
			if(this.var_findresource_0_1 == null)
				this.var_findresource_0_1 = this.builderType.GetMethod("FindResource", true, presourceKey).Import();
			
			return this.var_findresource_0_1;
						
		}
				
		private Method var_tryfindresource_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object TryFindResource(System.Object)<para/>
		/// </summary>
		public Method GetMethod_TryFindResource(TypeReference presourceKey)
		{
						
						
			if(this.var_tryfindresource_0_1 == null)
				this.var_tryfindresource_0_1 = this.builderType.GetMethod("TryFindResource", true, presourceKey).Import();
			
			return this.var_tryfindresource_0_1;
						
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
				this.var_loadcomponent_0_2 = this.builderType.GetMethod("LoadComponent", true, pcomponent, presourceLocator).Import();
			
			return this.var_loadcomponent_0_2;
						
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
				this.var_loadcomponent_0_1 = this.builderType.GetMethod("LoadComponent", true, presourceLocator).Import();
			
			return this.var_loadcomponent_0_1;
						
		}
				
		private Method var_getresourcestream_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Resources.StreamResourceInfo GetResourceStream(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetResourceStream(TypeReference puriResource)
		{
						
						
			if(this.var_getresourcestream_0_1 == null)
				this.var_getresourcestream_0_1 = this.builderType.GetMethod("GetResourceStream", true, puriResource).Import();
			
			return this.var_getresourcestream_0_1;
						
		}
				
		private Method var_getcontentstream_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Resources.StreamResourceInfo GetContentStream(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetContentStream(TypeReference puriContent)
		{
						
						
			if(this.var_getcontentstream_0_1 == null)
				this.var_getcontentstream_0_1 = this.builderType.GetMethod("GetContentStream", true, puriContent).Import();
			
			return this.var_getcontentstream_0_1;
						
		}
				
		private Method var_getremotestream_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Windows.Resources.StreamResourceInfo GetRemoteStream(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetRemoteStream(TypeReference puriRemote)
		{
						
						
			if(this.var_getremotestream_0_1 == null)
				this.var_getremotestream_0_1 = this.builderType.GetMethod("GetRemoteStream", true, puriRemote).Import();
			
			return this.var_getremotestream_0_1;
						
		}
				
		private Method var_getcookie_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String GetCookie(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_GetCookie(TypeReference puri)
		{
						
						
			if(this.var_getcookie_0_1 == null)
				this.var_getcookie_0_1 = this.builderType.GetMethod("GetCookie", true, puri).Import();
			
			return this.var_getcookie_0_1;
						
		}
				
		private Method var_setcookie_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void SetCookie(System.Uri, System.String)<para/>
		/// </summary>
		public Method GetMethod_SetCookie(TypeReference puri, TypeReference pvalue)
		{
						
						
			if(this.var_setcookie_0_2 == null)
				this.var_setcookie_0_2 = this.builderType.GetMethod("SetCookie", true, puri, pvalue).Import();
			
			return this.var_setcookie_0_2;
						
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
				this.var_get_current_0_0 = this.builderType.GetMethod("get_Current", true);

			return this.var_get_current_0_0;
						
						
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
				this.var_get_windows_0_0 = this.builderType.GetMethod("get_Windows", true);

			return this.var_get_windows_0_0;
						
						
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
				this.var_get_mainwindow_0_0 = this.builderType.GetMethod("get_MainWindow", true);

			return this.var_get_mainwindow_0_0;
						
						
		}
				
		private Method var_set_mainwindow_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_MainWindow(System.Windows.Window)<para/>
		/// </summary>
		public Method GetMethod_set_MainWindow(TypeReference pvalue)
		{
						
						
			if(this.var_set_mainwindow_0_1 == null)
				this.var_set_mainwindow_0_1 = this.builderType.GetMethod("set_MainWindow", true, pvalue).Import();
			
			return this.var_set_mainwindow_0_1;
						
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
				this.var_get_shutdownmode_0_0 = this.builderType.GetMethod("get_ShutdownMode", true);

			return this.var_get_shutdownmode_0_0;
						
						
		}
				
		private Method var_set_shutdownmode_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_ShutdownMode(System.Windows.ShutdownMode)<para/>
		/// </summary>
		public Method GetMethod_set_ShutdownMode(TypeReference pvalue)
		{
						
						
			if(this.var_set_shutdownmode_0_1 == null)
				this.var_set_shutdownmode_0_1 = this.builderType.GetMethod("set_ShutdownMode", true, pvalue).Import();
			
			return this.var_set_shutdownmode_0_1;
						
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
				this.var_get_resources_0_0 = this.builderType.GetMethod("get_Resources", true);

			return this.var_get_resources_0_0;
						
						
		}
				
		private Method var_set_resources_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_Resources(System.Windows.ResourceDictionary)<para/>
		/// </summary>
		public Method GetMethod_set_Resources(TypeReference pvalue)
		{
						
						
			if(this.var_set_resources_0_1 == null)
				this.var_set_resources_0_1 = this.builderType.GetMethod("set_Resources", true, pvalue).Import();
			
			return this.var_set_resources_0_1;
						
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
				this.var_get_startupuri_0_0 = this.builderType.GetMethod("get_StartupUri", true);

			return this.var_get_startupuri_0_0;
						
						
		}
				
		private Method var_set_startupuri_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_StartupUri(System.Uri)<para/>
		/// </summary>
		public Method GetMethod_set_StartupUri(TypeReference pvalue)
		{
						
						
			if(this.var_set_startupuri_0_1 == null)
				this.var_set_startupuri_0_1 = this.builderType.GetMethod("set_StartupUri", true, pvalue).Import();
			
			return this.var_set_startupuri_0_1;
						
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
				this.var_get_properties_0_0 = this.builderType.GetMethod("get_Properties", true);

			return this.var_get_properties_0_0;
						
						
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
				this.var_get_resourceassembly_0_0 = this.builderType.GetMethod("get_ResourceAssembly", true);

			return this.var_get_resourceassembly_0_0;
						
						
		}
				
		private Method var_set_resourceassembly_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_ResourceAssembly(System.Reflection.Assembly)<para/>
		/// </summary>
		public Method GetMethod_set_ResourceAssembly(TypeReference pvalue)
		{
						
						
			if(this.var_set_resourceassembly_0_1 == null)
				this.var_set_resourceassembly_0_1 = this.builderType.GetMethod("set_ResourceAssembly", true, pvalue).Import();
			
			return this.var_set_resourceassembly_0_1;
						
		}
				
		private Method var_add_startup_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Startup(System.Windows.StartupEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Startup(TypeReference pvalue)
		{
						
						
			if(this.var_add_startup_0_1 == null)
				this.var_add_startup_0_1 = this.builderType.GetMethod("add_Startup", true, pvalue).Import();
			
			return this.var_add_startup_0_1;
						
		}
				
		private Method var_remove_startup_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Startup(System.Windows.StartupEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Startup(TypeReference pvalue)
		{
						
						
			if(this.var_remove_startup_0_1 == null)
				this.var_remove_startup_0_1 = this.builderType.GetMethod("remove_Startup", true, pvalue).Import();
			
			return this.var_remove_startup_0_1;
						
		}
				
		private Method var_add_exit_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Exit(System.Windows.ExitEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Exit(TypeReference pvalue)
		{
						
						
			if(this.var_add_exit_0_1 == null)
				this.var_add_exit_0_1 = this.builderType.GetMethod("add_Exit", true, pvalue).Import();
			
			return this.var_add_exit_0_1;
						
		}
				
		private Method var_remove_exit_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Exit(System.Windows.ExitEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Exit(TypeReference pvalue)
		{
						
						
			if(this.var_remove_exit_0_1 == null)
				this.var_remove_exit_0_1 = this.builderType.GetMethod("remove_Exit", true, pvalue).Import();
			
			return this.var_remove_exit_0_1;
						
		}
				
		private Method var_add_activated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Activated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Activated(TypeReference pvalue)
		{
						
						
			if(this.var_add_activated_0_1 == null)
				this.var_add_activated_0_1 = this.builderType.GetMethod("add_Activated", true, pvalue).Import();
			
			return this.var_add_activated_0_1;
						
		}
				
		private Method var_remove_activated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Activated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Activated(TypeReference pvalue)
		{
						
						
			if(this.var_remove_activated_0_1 == null)
				this.var_remove_activated_0_1 = this.builderType.GetMethod("remove_Activated", true, pvalue).Import();
			
			return this.var_remove_activated_0_1;
						
		}
				
		private Method var_add_deactivated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Deactivated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Deactivated(TypeReference pvalue)
		{
						
						
			if(this.var_add_deactivated_0_1 == null)
				this.var_add_deactivated_0_1 = this.builderType.GetMethod("add_Deactivated", true, pvalue).Import();
			
			return this.var_add_deactivated_0_1;
						
		}
				
		private Method var_remove_deactivated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Deactivated(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Deactivated(TypeReference pvalue)
		{
						
						
			if(this.var_remove_deactivated_0_1 == null)
				this.var_remove_deactivated_0_1 = this.builderType.GetMethod("remove_Deactivated", true, pvalue).Import();
			
			return this.var_remove_deactivated_0_1;
						
		}
				
		private Method var_add_sessionending_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_SessionEnding(System.Windows.SessionEndingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_SessionEnding(TypeReference pvalue)
		{
						
						
			if(this.var_add_sessionending_0_1 == null)
				this.var_add_sessionending_0_1 = this.builderType.GetMethod("add_SessionEnding", true, pvalue).Import();
			
			return this.var_add_sessionending_0_1;
						
		}
				
		private Method var_remove_sessionending_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_SessionEnding(System.Windows.SessionEndingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_SessionEnding(TypeReference pvalue)
		{
						
						
			if(this.var_remove_sessionending_0_1 == null)
				this.var_remove_sessionending_0_1 = this.builderType.GetMethod("remove_SessionEnding", true, pvalue).Import();
			
			return this.var_remove_sessionending_0_1;
						
		}
				
		private Method var_add_dispatcherunhandledexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_DispatcherUnhandledException(System.Windows.Threading.DispatcherUnhandledExceptionEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_DispatcherUnhandledException(TypeReference pvalue)
		{
						
						
			if(this.var_add_dispatcherunhandledexception_0_1 == null)
				this.var_add_dispatcherunhandledexception_0_1 = this.builderType.GetMethod("add_DispatcherUnhandledException", true, pvalue).Import();
			
			return this.var_add_dispatcherunhandledexception_0_1;
						
		}
				
		private Method var_remove_dispatcherunhandledexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_DispatcherUnhandledException(System.Windows.Threading.DispatcherUnhandledExceptionEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_DispatcherUnhandledException(TypeReference pvalue)
		{
						
						
			if(this.var_remove_dispatcherunhandledexception_0_1 == null)
				this.var_remove_dispatcherunhandledexception_0_1 = this.builderType.GetMethod("remove_DispatcherUnhandledException", true, pvalue).Import();
			
			return this.var_remove_dispatcherunhandledexception_0_1;
						
		}
				
		private Method var_add_navigating_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Navigating(System.Windows.Navigation.NavigatingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Navigating(TypeReference pvalue)
		{
						
						
			if(this.var_add_navigating_0_1 == null)
				this.var_add_navigating_0_1 = this.builderType.GetMethod("add_Navigating", true, pvalue).Import();
			
			return this.var_add_navigating_0_1;
						
		}
				
		private Method var_remove_navigating_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Navigating(System.Windows.Navigation.NavigatingCancelEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Navigating(TypeReference pvalue)
		{
						
						
			if(this.var_remove_navigating_0_1 == null)
				this.var_remove_navigating_0_1 = this.builderType.GetMethod("remove_Navigating", true, pvalue).Import();
			
			return this.var_remove_navigating_0_1;
						
		}
				
		private Method var_add_navigated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Navigated(System.Windows.Navigation.NavigatedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Navigated(TypeReference pvalue)
		{
						
						
			if(this.var_add_navigated_0_1 == null)
				this.var_add_navigated_0_1 = this.builderType.GetMethod("add_Navigated", true, pvalue).Import();
			
			return this.var_add_navigated_0_1;
						
		}
				
		private Method var_remove_navigated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Navigated(System.Windows.Navigation.NavigatedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Navigated(TypeReference pvalue)
		{
						
						
			if(this.var_remove_navigated_0_1 == null)
				this.var_remove_navigated_0_1 = this.builderType.GetMethod("remove_Navigated", true, pvalue).Import();
			
			return this.var_remove_navigated_0_1;
						
		}
				
		private Method var_add_navigationprogress_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_NavigationProgress(System.Windows.Navigation.NavigationProgressEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_NavigationProgress(TypeReference pvalue)
		{
						
						
			if(this.var_add_navigationprogress_0_1 == null)
				this.var_add_navigationprogress_0_1 = this.builderType.GetMethod("add_NavigationProgress", true, pvalue).Import();
			
			return this.var_add_navigationprogress_0_1;
						
		}
				
		private Method var_remove_navigationprogress_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_NavigationProgress(System.Windows.Navigation.NavigationProgressEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_NavigationProgress(TypeReference pvalue)
		{
						
						
			if(this.var_remove_navigationprogress_0_1 == null)
				this.var_remove_navigationprogress_0_1 = this.builderType.GetMethod("remove_NavigationProgress", true, pvalue).Import();
			
			return this.var_remove_navigationprogress_0_1;
						
		}
				
		private Method var_add_navigationfailed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_NavigationFailed(System.Windows.Navigation.NavigationFailedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_NavigationFailed(TypeReference pvalue)
		{
						
						
			if(this.var_add_navigationfailed_0_1 == null)
				this.var_add_navigationfailed_0_1 = this.builderType.GetMethod("add_NavigationFailed", true, pvalue).Import();
			
			return this.var_add_navigationfailed_0_1;
						
		}
				
		private Method var_remove_navigationfailed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_NavigationFailed(System.Windows.Navigation.NavigationFailedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_NavigationFailed(TypeReference pvalue)
		{
						
						
			if(this.var_remove_navigationfailed_0_1 == null)
				this.var_remove_navigationfailed_0_1 = this.builderType.GetMethod("remove_NavigationFailed", true, pvalue).Import();
			
			return this.var_remove_navigationfailed_0_1;
						
		}
				
		private Method var_add_loadcompleted_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_LoadCompleted(System.Windows.Navigation.LoadCompletedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_LoadCompleted(TypeReference pvalue)
		{
						
						
			if(this.var_add_loadcompleted_0_1 == null)
				this.var_add_loadcompleted_0_1 = this.builderType.GetMethod("add_LoadCompleted", true, pvalue).Import();
			
			return this.var_add_loadcompleted_0_1;
						
		}
				
		private Method var_remove_loadcompleted_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_LoadCompleted(System.Windows.Navigation.LoadCompletedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_LoadCompleted(TypeReference pvalue)
		{
						
						
			if(this.var_remove_loadcompleted_0_1 == null)
				this.var_remove_loadcompleted_0_1 = this.builderType.GetMethod("remove_LoadCompleted", true, pvalue).Import();
			
			return this.var_remove_loadcompleted_0_1;
						
		}
				
		private Method var_add_navigationstopped_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_NavigationStopped(System.Windows.Navigation.NavigationStoppedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_NavigationStopped(TypeReference pvalue)
		{
						
						
			if(this.var_add_navigationstopped_0_1 == null)
				this.var_add_navigationstopped_0_1 = this.builderType.GetMethod("add_NavigationStopped", true, pvalue).Import();
			
			return this.var_add_navigationstopped_0_1;
						
		}
				
		private Method var_remove_navigationstopped_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_NavigationStopped(System.Windows.Navigation.NavigationStoppedEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_NavigationStopped(TypeReference pvalue)
		{
						
						
			if(this.var_remove_navigationstopped_0_1 == null)
				this.var_remove_navigationstopped_0_1 = this.builderType.GetMethod("remove_NavigationStopped", true, pvalue).Import();
			
			return this.var_remove_navigationstopped_0_1;
						
		}
				
		private Method var_add_fragmentnavigation_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_FragmentNavigation(System.Windows.Navigation.FragmentNavigationEventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_FragmentNavigation(TypeReference pvalue)
		{
						
						
			if(this.var_add_fragmentnavigation_0_1 == null)
				this.var_add_fragmentnavigation_0_1 = this.builderType.GetMethod("add_FragmentNavigation", true, pvalue).Import();
			
			return this.var_add_fragmentnavigation_0_1;
						
		}
				
		private Method var_remove_fragmentnavigation_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_FragmentNavigation(System.Windows.Navigation.FragmentNavigationEventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_FragmentNavigation(TypeReference pvalue)
		{
						
						
			if(this.var_remove_fragmentnavigation_0_1 == null)
				this.var_remove_fragmentnavigation_0_1 = this.builderType.GetMethod("remove_FragmentNavigation", true, pvalue).Import();
			
			return this.var_remove_fragmentnavigation_0_1;
						
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
				this.var_get_dispatcher_0_0 = this.builderType.GetMethod("get_Dispatcher", true);

			return this.var_get_dispatcher_0_0;
						
						
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
				this.var_checkaccess_0_0 = this.builderType.GetMethod("CheckAccess", true);

			return this.var_checkaccess_0_0;
						
						
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
				this.var_verifyaccess_0_0 = this.builderType.GetMethod("VerifyAccess", true);

			return this.var_verifyaccess_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeComponentAttribute value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeComponentAttribute value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_get_contractname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_ContractName()<para/>
		/// </summary>
		public Method GetMethod_get_ContractName()
		{
						
			if(this.var_get_contractname_0_0 == null)
				this.var_get_contractname_0_0 = this.builderType.GetMethod("get_ContractName", true);

			return this.var_get_contractname_0_0;
						
						
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
				this.var_get_invokeonobjectcreationevent_0_0 = this.builderType.GetMethod("get_InvokeOnObjectCreationEvent", true);

			return this.var_get_invokeonobjectcreationevent_0_0;
						
						
		}
				
		private Method var_set_invokeonobjectcreationevent_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_InvokeOnObjectCreationEvent(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_InvokeOnObjectCreationEvent(TypeReference pvalue)
		{
						
						
			if(this.var_set_invokeonobjectcreationevent_0_1 == null)
				this.var_set_invokeonobjectcreationevent_0_1 = this.builderType.GetMethod("set_InvokeOnObjectCreationEvent", true, pvalue).Import();
			
			return this.var_set_invokeonobjectcreationevent_0_1;
						
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
				this.var_get_policy_0_0 = this.builderType.GetMethod("get_Policy", true);

			return this.var_get_policy_0_0;
						
						
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
				this.var_get_priority_0_0 = this.builderType.GetMethod("get_Priority", true);

			return this.var_get_priority_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", true);

			return this.var_get_typeid_0_0;
						
						
		}
				
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match(TypeReference pobj)
		{
						
						
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", true, pobj).Import();
			
			return this.var_match_0_1;
						
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
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", true);

			return this.var_isdefaultattribute_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeComponentConstructorAttribute value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeComponentConstructorAttribute value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", true);

			return this.var_get_typeid_0_0;
						
						
		}
				
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match(TypeReference pobj)
		{
						
						
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", true, pobj).Import();
			
			return this.var_match_0_1;
						
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
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", true);

			return this.var_isdefaultattribute_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeExtensions value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeExtensions value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_getsha256hash_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String GetSHA256Hash(System.String)<para/>
		/// </summary>
		public Method GetMethod_GetSHA256Hash(TypeReference pvalue)
		{
						
						
			if(this.var_getsha256hash_0_1 == null)
				this.var_getsha256hash_0_1 = this.builderType.GetMethod("GetSHA256Hash", true, pvalue).Import();
			
			return this.var_getsha256hash_0_1;
						
		}
				
		private Method var_trydisposeinternal_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void TryDisposeInternal(System.Object)<para/>
		/// </summary>
		public Method GetMethod_TryDisposeInternal(TypeReference pcontext)
		{
						
						
			if(this.var_trydisposeinternal_0_1 == null)
				this.var_trydisposeinternal_0_1 = this.builderType.GetMethod("TryDisposeInternal", true, pcontext).Import();
			
			return this.var_trydisposeinternal_0_1;
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeExtensionsReflection value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeExtensionsReflection value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_arereferenceassignable_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean AreReferenceAssignable(System.Type, System.Type)<para/>
		/// </summary>
		public Method GetMethod_AreReferenceAssignable(TypeReference ptype, TypeReference ptoBeAssigned)
		{
						
						
			if(this.var_arereferenceassignable_0_2 == null)
				this.var_arereferenceassignable_0_2 = this.builderType.GetMethod("AreReferenceAssignable", true, ptype, ptoBeAssigned).Import();
			
			return this.var_arereferenceassignable_0_2;
						
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
					this.var_createinstance_0_2 = this.builderType.GetMethod("CreateInstance", true, ptype, pargs).Import();
			
				return this.var_createinstance_0_2;
			}
			
			if(typeof(System.Reflection.ConstructorInfo).AreEqual(ptype) && typeof(System.Object[]).AreEqual(pargs))
			{
				if(this.var_createinstance_1_2 == null)
					this.var_createinstance_1_2 = this.builderType.GetMethod("CreateInstance", true, ptype, pargs).Import();
			
				return this.var_createinstance_1_2;
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
				
		private Method var_getchildrentype_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type GetChildrenType(System.Type)<para/>
		/// </summary>
		public Method GetMethod_GetChildrenType(TypeReference ptype)
		{
						
						
			if(this.var_getchildrentype_0_1 == null)
				this.var_getchildrentype_0_1 = this.builderType.GetMethod("GetChildrenType", true, ptype).Import();
			
			return this.var_getchildrentype_0_1;
						
		}
				
		private Method var_getdefaultinstance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object GetDefaultInstance(System.Type)<para/>
		/// </summary>
		public Method GetMethod_GetDefaultInstance(TypeReference ptype)
		{
						
						
			if(this.var_getdefaultinstance_0_1 == null)
				this.var_getdefaultinstance_0_1 = this.builderType.GetMethod("GetDefaultInstance", true, ptype).Import();
			
			return this.var_getdefaultinstance_0_1;
						
		}
				
		private Method var_getdictionarykeyvaluetypes_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type[] GetDictionaryKeyValueTypes(System.Type)<para/>
		/// </summary>
		public Method GetMethod_GetDictionaryKeyValueTypes(TypeReference ptype)
		{
						
						
			if(this.var_getdictionarykeyvaluetypes_0_1 == null)
				this.var_getdictionarykeyvaluetypes_0_1 = this.builderType.GetMethod("GetDictionaryKeyValueTypes", true, ptype).Import();
			
			return this.var_getdictionarykeyvaluetypes_0_1;
						
		}
				
		private Method var_getfieldsex_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[System.Reflection.FieldInfo] GetFieldsEx(System.Type, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetFieldsEx(TypeReference ptype, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getfieldsex_0_2 == null)
				this.var_getfieldsex_0_2 = this.builderType.GetMethod("GetFieldsEx", true, ptype, pbindingFlags).Import();
			
			return this.var_getfieldsex_0_2;
						
		}
				
		private Method var_getmethod_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.MethodInfo GetMethod(System.Type, System.String, System.Type[], System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetMethod(TypeReference ptype, TypeReference pmethodName, TypeReference pparameterTypes, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getmethod_0_4 == null)
				this.var_getmethod_0_4 = this.builderType.GetMethod("GetMethod", true, ptype, pmethodName, pparameterTypes, pbindingFlags).Import();
			
			return this.var_getmethod_0_4;
						
		}
				
		private Method var_getmethodex_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.MethodInfo GetMethodEx(System.Type, System.String, System.Type[], System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetMethodEx(TypeReference ptype, TypeReference pmethodName, TypeReference pparameterTypes, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getmethodex_0_4 == null)
				this.var_getmethodex_0_4 = this.builderType.GetMethod("GetMethodEx", true, ptype, pmethodName, pparameterTypes, pbindingFlags).Import();
			
			return this.var_getmethodex_0_4;
						
		}
				
		private Method var_getmethodsex_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[System.Reflection.MethodInfo] GetMethodsEx(System.Type, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetMethodsEx(TypeReference ptype, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getmethodsex_0_2 == null)
				this.var_getmethodsex_0_2 = this.builderType.GetMethod("GetMethodsEx", true, ptype, pbindingFlags).Import();
			
			return this.var_getmethodsex_0_2;
						
		}
				
		private Method var_getpropertiesex_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Collections.Generic.IEnumerable`1[System.Reflection.PropertyInfo] GetPropertiesEx(System.Type, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertiesEx(TypeReference ptype, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getpropertiesex_0_2 == null)
				this.var_getpropertiesex_0_2 = this.builderType.GetMethod("GetPropertiesEx", true, ptype, pbindingFlags).Import();
			
			return this.var_getpropertiesex_0_2;
						
		}
				
		private Method var_getpropertyex_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.PropertyInfo GetPropertyEx(System.Type, System.String, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertyEx(TypeReference ptype, TypeReference ppropertyName, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getpropertyex_0_3 == null)
				this.var_getpropertyex_0_3 = this.builderType.GetMethod("GetPropertyEx", true, ptype, ppropertyName, pbindingFlags).Import();
			
			return this.var_getpropertyex_0_3;
						
		}
				
		private Method var_getpropertyfrompath_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Reflection.PropertyInfo GetPropertyFromPath(System.Type, System.String, System.Reflection.BindingFlags)<para/>
		/// </summary>
		public Method GetMethod_GetPropertyFromPath(TypeReference ptype, TypeReference ppath, TypeReference pbindingFlags)
		{
						
						
			if(this.var_getpropertyfrompath_0_3 == null)
				this.var_getpropertyfrompath_0_3 = this.builderType.GetMethod("GetPropertyFromPath", true, ptype, ppath, pbindingFlags).Import();
			
			return this.var_getpropertyfrompath_0_3;
						
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
				this.var_getpropertyvalue_0_3 = this.builderType.GetMethod("GetPropertyValue", true, pobj, ppropertyName, pbindingFlags).Import();
			
			return this.var_getpropertyvalue_0_3;
						
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
					this.var_implementsinterface_0_2 = this.builderType.GetMethod("ImplementsInterface", true, ptype, ptypeOfInterface).Import();
			
				return this.var_implementsinterface_0_2;
			}
			
			if(typeof(System.Reflection.TypeInfo).AreEqual(ptype) && typeof(System.Type).AreEqual(ptypeOfInterface))
			{
				if(this.var_implementsinterface_1_2 == null)
					this.var_implementsinterface_1_2 = this.builderType.GetMethod("ImplementsInterface", true, ptype, ptypeOfInterface).Import();
			
				return this.var_implementsinterface_1_2;
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
				
		private Method var_iscollectionorlist_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsCollectionOrList(System.Type)<para/>
		/// </summary>
		public Method GetMethod_IsCollectionOrList(TypeReference ptype)
		{
						
						
			if(this.var_iscollectionorlist_0_1 == null)
				this.var_iscollectionorlist_0_1 = this.builderType.GetMethod("IsCollectionOrList", true, ptype).Import();
			
			return this.var_iscollectionorlist_0_1;
						
		}
				
		private Method var_isnullable_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean IsNullable(System.Type)<para/>
		/// </summary>
		public Method GetMethod_IsNullable(TypeReference ptarget)
		{
						
						
			if(this.var_isnullable_0_1 == null)
				this.var_isnullable_0_1 = this.builderType.GetMethod("IsNullable", true, ptarget).Import();
			
			return this.var_isnullable_0_1;
						
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
					this.var_matchesargumenttypes_0_2 = this.builderType.GetMethod("MatchesArgumentTypes", true, pmethod, pargumentTypes).Import();
			
				return this.var_matchesargumenttypes_0_2;
			}
			
			if(typeof(System.Reflection.ParameterInfo[]).AreEqual(pmethod) && typeof(System.Type[]).AreEqual(pargumentTypes))
			{
				if(this.var_matchesargumenttypes_1_2 == null)
					this.var_matchesargumenttypes_1_2 = this.builderType.GetMethod("MatchesArgumentTypes", true, pmethod, pargumentTypes).Import();
			
				return this.var_matchesargumenttypes_1_2;
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeFactory value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeFactory value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_add_objectcreated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_ObjectCreated(System.EventHandler`1[Cauldron.Activator.FactoryObjectCreatedEventArgs])<para/>
		/// </summary>
		public Method GetMethod_add_ObjectCreated(TypeReference pvalue)
		{
						
						
			if(this.var_add_objectcreated_0_1 == null)
				this.var_add_objectcreated_0_1 = this.builderType.GetMethod("add_ObjectCreated", true, pvalue).Import();
			
			return this.var_add_objectcreated_0_1;
						
		}
				
		private Method var_remove_objectcreated_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_ObjectCreated(System.EventHandler`1[Cauldron.Activator.FactoryObjectCreatedEventArgs])<para/>
		/// </summary>
		public Method GetMethod_remove_ObjectCreated(TypeReference pvalue)
		{
						
						
			if(this.var_remove_objectcreated_0_1 == null)
				this.var_remove_objectcreated_0_1 = this.builderType.GetMethod("remove_ObjectCreated", true, pvalue).Import();
			
			return this.var_remove_objectcreated_0_1;
						
		}
				
		private Method var_add_rebuilt_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Rebuilt(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Rebuilt(TypeReference pvalue)
		{
						
						
			if(this.var_add_rebuilt_0_1 == null)
				this.var_add_rebuilt_0_1 = this.builderType.GetMethod("add_Rebuilt", true, pvalue).Import();
			
			return this.var_add_rebuilt_0_1;
						
		}
				
		private Method var_remove_rebuilt_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Rebuilt(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Rebuilt(TypeReference pvalue)
		{
						
						
			if(this.var_remove_rebuilt_0_1 == null)
				this.var_remove_rebuilt_0_1 = this.builderType.GetMethod("remove_Rebuilt", true, pvalue).Import();
			
			return this.var_remove_rebuilt_0_1;
						
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
				this.var_get_canraiseexceptions_0_0 = this.builderType.GetMethod("get_CanRaiseExceptions", true);

			return this.var_get_canraiseexceptions_0_0;
						
						
		}
				
		private Method var_set_canraiseexceptions_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_CanRaiseExceptions(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_CanRaiseExceptions(TypeReference pvalue)
		{
						
						
			if(this.var_set_canraiseexceptions_0_1 == null)
				this.var_set_canraiseexceptions_0_1 = this.builderType.GetMethod("set_CanRaiseExceptions", true, pvalue).Import();
			
			return this.var_set_canraiseexceptions_0_1;
						
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
				this.var_get_factorytypes_0_0 = this.builderType.GetMethod("get_FactoryTypes", true);

			return this.var_get_factorytypes_0_0;
						
						
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
				this.var_get_registeredtypes_0_0 = this.builderType.GetMethod("get_RegisteredTypes", true);

			return this.var_get_registeredtypes_0_0;
						
						
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
				this.var_get_resolvers_0_0 = this.builderType.GetMethod("get_Resolvers", true);

			return this.var_get_resolvers_0_0;
						
						
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
					this.var_create_0_2 = this.builderType.GetMethod("Create", true, pcontractName, pparameters).Import();
			
				return this.var_create_0_2;
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_create_1_2 == null)
					this.var_create_1_2 = this.builderType.GetMethod("Create", true, pcontractName, pparameters).Import();
			
				return this.var_create_1_2;
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
					this.var_createfirst_0_2 = this.builderType.GetMethod("CreateFirst", true, pcontractType, pparameters).Import();
			
				return this.var_createfirst_0_2;
			}
			
			if(typeof(System.String).AreEqual(pcontractType) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createfirst_1_2 == null)
					this.var_createfirst_1_2 = this.builderType.GetMethod("CreateFirst", true, pcontractType, pparameters).Import();
			
				return this.var_createfirst_1_2;
			}
			
			throw new Exception("Method with defined parameters not found.");
			
		}
				
		private Method var_createinstance_0_2;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object CreateInstance(System.Type, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_CreateInstance(TypeReference ptype, TypeReference pargs)
		{
						
						
			if(this.var_createinstance_0_2 == null)
				this.var_createinstance_0_2 = this.builderType.GetMethod("CreateInstance", true, ptype, pargs).Import();
			
			return this.var_createinstance_0_2;
						
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
					this.var_createmany_0_2 = this.builderType.GetMethod("CreateMany", true, pcontractName, pparameters).Import();
			
				return this.var_createmany_0_2;
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createmany_1_2 == null)
					this.var_createmany_1_2 = this.builderType.GetMethod("CreateMany", true, pcontractName, pparameters).Import();
			
				return this.var_createmany_1_2;
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
					this.var_createmanyordered_0_2 = this.builderType.GetMethod("CreateManyOrdered", true, pcontractName, pparameters).Import();
			
				return this.var_createmanyordered_0_2;
			}
			
			if(typeof(System.Type).AreEqual(pcontractName) && typeof(System.Object[]).AreEqual(pparameters))
			{
				if(this.var_createmanyordered_1_2 == null)
					this.var_createmanyordered_1_2 = this.builderType.GetMethod("CreateManyOrdered", true, pcontractName, pparameters).Import();
			
				return this.var_createmanyordered_1_2;
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
					this.var_destroy_0_1 = this.builderType.GetMethod("Destroy", true, pcontractType).Import();
			
				return this.var_destroy_0_1;
			}
			
			if(typeof(System.String).AreEqual(pcontractType))
			{
				if(this.var_destroy_1_1 == null)
					this.var_destroy_1_1 = this.builderType.GetMethod("Destroy", true, pcontractType).Import();
			
				return this.var_destroy_1_1;
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

			return this.var_destroy_0_0;
						
						
		}
				
		private Method var_getfactorytypeinfo_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.IFactoryTypeInfo GetFactoryTypeInfo(System.String)<para/>
		/// </summary>
		public Method GetMethod_GetFactoryTypeInfo(TypeReference pcontractName)
		{
						
						
			if(this.var_getfactorytypeinfo_0_1 == null)
				this.var_getfactorytypeinfo_0_1 = this.builderType.GetMethod("GetFactoryTypeInfo", true, pcontractName).Import();
			
			return this.var_getfactorytypeinfo_0_1;
						
		}
				
		private Method var_getfactorytypeinfofirst_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Cauldron.Activator.IFactoryTypeInfo GetFactoryTypeInfoFirst(System.String)<para/>
		/// </summary>
		public Method GetMethod_GetFactoryTypeInfoFirst(TypeReference pcontractName)
		{
						
						
			if(this.var_getfactorytypeinfofirst_0_1 == null)
				this.var_getfactorytypeinfofirst_0_1 = this.builderType.GetMethod("GetFactoryTypeInfoFirst", true, pcontractName).Import();
			
			return this.var_getfactorytypeinfofirst_0_1;
						
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
					this.var_hascontract_0_1 = this.builderType.GetMethod("HasContract", true, pcontractName).Import();
			
				return this.var_hascontract_0_1;
			}
			
			if(typeof(System.Type).AreEqual(pcontractName))
			{
				if(this.var_hascontract_1_1 == null)
					this.var_hascontract_1_1 = this.builderType.GetMethod("HasContract", true, pcontractName).Import();
			
				return this.var_hascontract_1_1;
			}
			
			throw new Exception("Method with defined parameters not found.");
			
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
					this.var_removetype_0_2 = this.builderType.GetMethod("RemoveType", true, pcontractType, ptype).Import();
			
				return this.var_removetype_0_2;
			}
			
			if(typeof(System.String).AreEqual(pcontractType) && typeof(System.Type).AreEqual(ptype))
			{
				if(this.var_removetype_1_2 == null)
					this.var_removetype_1_2 = this.builderType.GetMethod("RemoveType", true, pcontractType, ptype).Import();
			
				return this.var_removetype_1_2;
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeFactory1 value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeFactory1 value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_tostring_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String ToString()<para/>
		/// </summary>
		public Method GetMethod_ToString()
		{
						
			if(this.var_tostring_0_0 == null)
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeGenericComponentAttribute value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeGenericComponentAttribute value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_get_contractname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_ContractName()<para/>
		/// </summary>
		public Method GetMethod_get_ContractName()
		{
						
			if(this.var_get_contractname_0_0 == null)
				this.var_get_contractname_0_0 = this.builderType.GetMethod("get_ContractName", true);

			return this.var_get_contractname_0_0;
						
						
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
				this.var_get_invokeonobjectcreationevent_0_0 = this.builderType.GetMethod("get_InvokeOnObjectCreationEvent", true);

			return this.var_get_invokeonobjectcreationevent_0_0;
						
						
		}
				
		private Method var_set_invokeonobjectcreationevent_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_InvokeOnObjectCreationEvent(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_InvokeOnObjectCreationEvent(TypeReference pvalue)
		{
						
						
			if(this.var_set_invokeonobjectcreationevent_0_1 == null)
				this.var_set_invokeonobjectcreationevent_0_1 = this.builderType.GetMethod("set_InvokeOnObjectCreationEvent", true, pvalue).Import();
			
			return this.var_set_invokeonobjectcreationevent_0_1;
						
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
				this.var_get_policy_0_0 = this.builderType.GetMethod("get_Policy", true);

			return this.var_get_policy_0_0;
						
						
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
				this.var_get_priority_0_0 = this.builderType.GetMethod("get_Priority", true);

			return this.var_get_priority_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", true);

			return this.var_get_typeid_0_0;
						
						
		}
				
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match(TypeReference pobj)
		{
						
						
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", true, pobj).Import();
			
			return this.var_match_0_1;
						
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
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", true);

			return this.var_isdefaultattribute_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeIConstructorInterceptor value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIConstructorInterceptor value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_onbeforeinitialization_0_3;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnBeforeInitialization(System.Type, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnBeforeInitialization(TypeReference pdeclaringType, TypeReference pmethodbase, TypeReference pvalues)
		{
						
						
			if(this.var_onbeforeinitialization_0_3 == null)
				this.var_onbeforeinitialization_0_3 = this.builderType.GetMethod("OnBeforeInitialization", true, pdeclaringType, pmethodbase, pvalues).Import();
			
			return this.var_onbeforeinitialization_0_3;
						
		}
				
		private Method var_onenter_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnEnter(System.Type, System.Object, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnEnter(TypeReference pdeclaringType, TypeReference pinstance, TypeReference pmethodbase, TypeReference pvalues)
		{
						
						
			if(this.var_onenter_0_4 == null)
				this.var_onenter_0_4 = this.builderType.GetMethod("OnEnter", true, pdeclaringType, pinstance, pmethodbase, pvalues).Import();
			
			return this.var_onenter_0_4;
						
		}
				
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException(TypeReference pe)
		{
						
						
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", true, pe).Import();
			
			return this.var_onexception_0_1;
						
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
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", true);

			return this.var_onexit_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeIDisposableObject value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIDisposableObject value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_add_disposed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void add_Disposed(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_add_Disposed(TypeReference pvalue)
		{
						
						
			if(this.var_add_disposed_0_1 == null)
				this.var_add_disposed_0_1 = this.builderType.GetMethod("add_Disposed", true, pvalue).Import();
			
			return this.var_add_disposed_0_1;
						
		}
				
		private Method var_remove_disposed_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void remove_Disposed(System.EventHandler)<para/>
		/// </summary>
		public Method GetMethod_remove_Disposed(TypeReference pvalue)
		{
						
						
			if(this.var_remove_disposed_0_1 == null)
				this.var_remove_disposed_0_1 = this.builderType.GetMethod("remove_Disposed", true, pvalue).Import();
			
			return this.var_remove_disposed_0_1;
						
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
				this.var_get_isdisposed_0_0 = this.builderType.GetMethod("get_IsDisposed", true);

			return this.var_get_isdisposed_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeIFactoryExtension value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIFactoryExtension value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_initialize_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void Initialize(System.Collections.Generic.IEnumerable`1[Cauldron.Activator.IFactoryTypeInfo])<para/>
		/// </summary>
		public Method GetMethod_Initialize(TypeReference pfactoryInfoTypes)
		{
						
						
			if(this.var_initialize_0_1 == null)
				this.var_initialize_0_1 = this.builderType.GetMethod("Initialize", true, pfactoryInfoTypes).Import();
			
			return this.var_initialize_0_1;
						
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
		public static implicit operator BuilderType(BuilderTypeIFactoryTypeInfo value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIFactoryTypeInfo value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_get_contractname_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.String get_ContractName()<para/>
		/// </summary>
		public Method GetMethod_get_ContractName()
		{
						
			if(this.var_get_contractname_0_0 == null)
				this.var_get_contractname_0_0 = this.builderType.GetMethod("get_ContractName", true);

			return this.var_get_contractname_0_0;
						
						
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
				this.var_get_creationpolicy_0_0 = this.builderType.GetMethod("get_CreationPolicy", true);

			return this.var_get_creationpolicy_0_0;
						
						
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
				this.var_get_instance_0_0 = this.builderType.GetMethod("get_Instance", true);

			return this.var_get_instance_0_0;
						
						
		}
				
		private Method var_set_instance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_Instance(System.Object)<para/>
		/// </summary>
		public Method GetMethod_set_Instance(TypeReference pvalue)
		{
						
						
			if(this.var_set_instance_0_1 == null)
				this.var_set_instance_0_1 = this.builderType.GetMethod("set_Instance", true, pvalue).Import();
			
			return this.var_set_instance_0_1;
						
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
				this.var_get_priority_0_0 = this.builderType.GetMethod("get_Priority", true);

			return this.var_get_priority_0_0;
						
						
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
				this.var_get_type_0_0 = this.builderType.GetMethod("get_Type", true);

			return this.var_get_type_0_0;
						
						
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
				this.var_createinstance_0_1 = this.builderType.GetMethod("CreateInstance", true, parguments).Import();
			
			return this.var_createinstance_0_1;
						
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
		public static implicit operator BuilderType(BuilderTypeIMethodInterceptor value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIMethodInterceptor value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_onenter_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnEnter(System.Type, System.Object, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnEnter(TypeReference pdeclaringType, TypeReference pinstance, TypeReference pmethodbase, TypeReference pvalues)
		{
						
						
			if(this.var_onenter_0_4 == null)
				this.var_onenter_0_4 = this.builderType.GetMethod("OnEnter", true, pdeclaringType, pinstance, pmethodbase, pvalues).Import();
			
			return this.var_onenter_0_4;
						
		}
				
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException(TypeReference pe)
		{
						
						
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", true, pe).Import();
			
			return this.var_onexception_0_1;
						
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
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", true);

			return this.var_onexit_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeInterceptionRuleAttribute value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeInterceptionRuleAttribute value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", true);

			return this.var_get_typeid_0_0;
						
						
		}
				
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match(TypeReference pobj)
		{
						
						
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", true, pobj).Import();
			
			return this.var_match_0_1;
						
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
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", true);

			return this.var_isdefaultattribute_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeInterceptorOptionsAttribute value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeInterceptorOptionsAttribute value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_get_alwayscreatenewinstance_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean get_AlwaysCreateNewInstance()<para/>
		/// </summary>
		public Method GetMethod_get_AlwaysCreateNewInstance()
		{
						
			if(this.var_get_alwayscreatenewinstance_0_0 == null)
				this.var_get_alwayscreatenewinstance_0_0 = this.builderType.GetMethod("get_AlwaysCreateNewInstance", true);

			return this.var_get_alwayscreatenewinstance_0_0;
						
						
		}
				
		private Method var_set_alwayscreatenewinstance_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_AlwaysCreateNewInstance(Boolean)<para/>
		/// </summary>
		public Method GetMethod_set_AlwaysCreateNewInstance(TypeReference pvalue)
		{
						
						
			if(this.var_set_alwayscreatenewinstance_0_1 == null)
				this.var_set_alwayscreatenewinstance_0_1 = this.builderType.GetMethod("set_AlwaysCreateNewInstance", true, pvalue).Import();
			
			return this.var_set_alwayscreatenewinstance_0_1;
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_get_typeid_0_0 = this.builderType.GetMethod("get_TypeId", true);

			return this.var_get_typeid_0_0;
						
						
		}
				
		private Method var_match_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Match(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Match(TypeReference pobj)
		{
						
						
			if(this.var_match_0_1 == null)
				this.var_match_0_1 = this.builderType.GetMethod("Match", true, pobj).Import();
			
			return this.var_match_0_1;
						
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
				this.var_isdefaultattribute_0_0 = this.builderType.GetMethod("IsDefaultAttribute", true);

			return this.var_isdefaultattribute_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeIPropertyGetterInterceptor value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIPropertyGetterInterceptor value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException(TypeReference pe)
		{
						
						
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", true, pe).Import();
			
			return this.var_onexception_0_1;
						
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
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", true);

			return this.var_onexit_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeIPropertyInterceptorInitialize value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIPropertyInterceptorInitialize value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
		
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
		public static implicit operator BuilderType(BuilderTypeIPropertySetterInterceptor value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeIPropertySetterInterceptor value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_onexception_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean OnException(System.Exception)<para/>
		/// </summary>
		public Method GetMethod_OnException(TypeReference pe)
		{
						
						
			if(this.var_onexception_0_1 == null)
				this.var_onexception_0_1 = this.builderType.GetMethod("OnException", true, pe).Import();
			
			return this.var_onexception_0_1;
						
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
				this.var_onexit_0_0 = this.builderType.GetMethod("OnExit", true);

			return this.var_onexit_0_0;
						
						
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
		public static implicit operator BuilderType(BuilderTypeISimpleMethodInterceptor value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeISimpleMethodInterceptor value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_onenter_0_4;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void OnEnter(System.Type, System.Object, System.Reflection.MethodBase, System.Object[])<para/>
		/// </summary>
		public Method GetMethod_OnEnter(TypeReference pdeclaringType, TypeReference pinstance, TypeReference pmethodbase, TypeReference pvalues)
		{
						
						
			if(this.var_onenter_0_4 == null)
				this.var_onenter_0_4 = this.builderType.GetMethod("OnEnter", true, pdeclaringType, pinstance, pmethodbase, pvalues).Import();
			
			return this.var_onenter_0_4;
						
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
		public static implicit operator BuilderType(BuilderTypeISyncRoot value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypeISyncRoot value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_get_syncroot_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Object get_SyncRoot()<para/>
		/// </summary>
		public Method GetMethod_get_SyncRoot()
		{
						
			if(this.var_get_syncroot_0_0 == null)
				this.var_get_syncroot_0_0 = this.builderType.GetMethod("get_SyncRoot", true);

			return this.var_get_syncroot_0_0;
						
						
		}
				
		private Method var_set_syncroot_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void set_SyncRoot(System.Object)<para/>
		/// </summary>
		public Method GetMethod_set_SyncRoot(TypeReference pvalue)
		{
						
						
			if(this.var_set_syncroot_0_1 == null)
				this.var_set_syncroot_0_1 = this.builderType.GetMethod("set_SyncRoot", true, pvalue).Import();
			
			return this.var_set_syncroot_0_1;
						
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
		public static implicit operator BuilderType(BuilderTypePropertyInterceptionInfo value) => value.builderType;
		
		/// <exclude />
		public static implicit operator TypeReference(BuilderTypePropertyInterceptionInfo value) => (TypeReference)value.builderType;

		/// <exclude />
		public BuilderType BuilderType => this.builderType;
		
				
		private Method var_get_childtype_0_0;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// System.Type get_ChildType()<para/>
		/// </summary>
		public Method GetMethod_get_ChildType()
		{
						
			if(this.var_get_childtype_0_0 == null)
				this.var_get_childtype_0_0 = this.builderType.GetMethod("get_ChildType", true);

			return this.var_get_childtype_0_0;
						
						
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
				this.var_get_declaringtype_0_0 = this.builderType.GetMethod("get_DeclaringType", true);

			return this.var_get_declaringtype_0_0;
						
						
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
				this.var_get_getmethod_0_0 = this.builderType.GetMethod("get_GetMethod", true);

			return this.var_get_getmethod_0_0;
						
						
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
				this.var_get_instance_0_0 = this.builderType.GetMethod("get_Instance", true);

			return this.var_get_instance_0_0;
						
						
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
				this.var_get_propertyname_0_0 = this.builderType.GetMethod("get_PropertyName", true);

			return this.var_get_propertyname_0_0;
						
						
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
				this.var_get_propertytype_0_0 = this.builderType.GetMethod("get_PropertyType", true);

			return this.var_get_propertytype_0_0;
						
						
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
				this.var_get_setmethod_0_0 = this.builderType.GetMethod("get_SetMethod", true);

			return this.var_get_setmethod_0_0;
						
						
		}
				
		private Method var_setvalue_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Void SetValue(System.Object)<para/>
		/// </summary>
		public Method GetMethod_SetValue(TypeReference pvalue)
		{
						
						
			if(this.var_setvalue_0_1 == null)
				this.var_setvalue_0_1 = this.builderType.GetMethod("SetValue", true, pvalue).Import();
			
			return this.var_setvalue_0_1;
						
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
				this.var_topropertyinfo_0_0 = this.builderType.GetMethod("ToPropertyInfo", true);

			return this.var_topropertyinfo_0_0;
						
						
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
				this.var_tostring_0_0 = this.builderType.GetMethod("ToString", true);

			return this.var_tostring_0_0;
						
						
		}
				
		private Method var_equals_0_1;
		
		/// <summary>
		/// Represents the following method:
		/// <para />
		/// Boolean Equals(System.Object)<para/>
		/// </summary>
		public Method GetMethod_Equals(TypeReference pobj)
		{
						
						
			if(this.var_equals_0_1 == null)
				this.var_equals_0_1 = this.builderType.GetMethod("Equals", true, pobj).Import();
			
			return this.var_equals_0_1;
						
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
				this.var_gethashcode_0_0 = this.builderType.GetMethod("GetHashCode", true);

			return this.var_gethashcode_0_0;
						
						
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
				this.var_gettype_0_0 = this.builderType.GetMethod("GetType", true);

			return this.var_gettype_0_0;
						
						
		}
		
	}

	}


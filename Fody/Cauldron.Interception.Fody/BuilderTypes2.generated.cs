
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
	
		#region Void
        private static BuilderType _Void;
		
        /// <summary>
        /// Gets <see cref="BuilderType"/> representing void. 
        /// </summary>
        public static BuilderType Void
        {
            get
            {
                if (_Void == null) _Void = Builder.Current.GetType("System.Void");
                return _Void;
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

	}


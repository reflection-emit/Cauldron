using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public partial class BuilderTypeEnumerable
    {
        private Dictionary<TypeReference, Method> castMethod = new Dictionary<TypeReference, Method>();

        private Dictionary<TypeReference, Method> toArray = new Dictionary<TypeReference, Method>();
        private Dictionary<TypeReference, Method> toList = new Dictionary<TypeReference, Method>();

        public Method GetMethod_Cast(TypeReference child)
        {
            if (castMethod.TryGetValue(child, out Method value))
                return value;

            var result = this.builderType.GetMethod("Cast", 1, true).MakeGeneric(child).Import();
            castMethod.Add(child, result);
            return result;
        }

        public Method GetMethod_ToArray(TypeReference child)
        {
            if (toArray.TryGetValue(child, out Method value))
                return value;

            var result = this.builderType.GetMethod("ToArray", 1, true).MakeGeneric(child).Import();
            toArray.Add(child, result);
            return result;
        }

        public Method GetMethod_ToList(TypeReference child)
        {
            if (toList.TryGetValue(child, out Method value))
                return value;

            var result = this.builderType.GetMethod("ToList", 1, true).MakeGeneric(child).Import();
            toList.Add(child, result);
            return result;
        }
    }

    public partial class BuilderTypeEventHandler : TypeSystemExBase
    {
        private Method constructor;

        public Method GetConstructor()
        {
            if (constructor == null)
                constructor = this.builderType.GetMethod(".ctor", 2, true).Import();

            return constructor;
        }
    }

    public partial class BuilderTypeEventHandler1
    {
        private Method var_invoke_0_2;

        public Method GetMethod_Invoke()
        {
            if (this.var_invoke_0_2 == null)
                this.var_invoke_0_2 = this.builderType.GetMethod("Invoke", 2, true).Import();

            return this.var_invoke_0_2;
        }
    }

    public partial class BuilderTypeExtensionsReflection : TypeSystemExBase
    {
        /// <summary>
        /// Represents the following method:
        /// <para />
        /// System.Object CreateInstance(System.Type, System.Object[])<para/>
        /// System.Object CreateInstance(System.Reflection.ConstructorInfo, System.Object[])<para/>
        /// </summary>
        public Method GetMethod_CreateInstance()
        {
            if (this.var_createinstance_0_2 == null)
                this.var_createinstance_0_2 = this.builderType.GetMethod("CreateInstance", true, "System.Type", "System.Object[]").Import();

            return this.var_createinstance_0_2;
        }
    }

    public partial class BuilderTypeFactory : TypeSystemExBase
    {
    }

    public partial class BuilderTypeICollection1
    {
        private Method _add;

        public Method GetMethod_Add()
        {
            if (this._add == null)
                this._add = this.builderType.GetMethod("Add", 1, true).Import();

            return this._add;
        }
    }

    public partial class BuilderTypeIFactoryTypeInfo : TypeSystemExBase
    {
        /// <summary>
        /// Represents the following method:
        /// <para />
        /// System.Object CreateInstance(System.Object[])<para/>
        /// </summary>
        public Method GetMethod_CreateInstance_1()
        {
            if (this.var_createinstance_0_1 == null)
                this.var_createinstance_0_1 = this.builderType.GetMethod("CreateInstance", 1, true);

            return this.var_createinstance_0_1.Import();
        }
    }

    public partial class BuilderTypeInterceptionRuleAttribute : TypeSystemExBase
    {
        private Method constructor;

        public Method GetConstructor()
        {
            if (this.constructor == null)
                this.constructor = this.builderType.GetMethod("Equals", 2, true).Import();

            return this.constructor;
        }
    }

    public partial class BuilderTypeIPropertySetterInterceptor : TypeSystemExBase
    {
    }

    public partial class BuilderTypeISyncRoot : TypeSystemExBase
    {
    }

    public partial class BuilderTypeMethodBase
    {
        public Method GetMethod_GetMethodFromHandle()
        {
            if (this.var_getmethodfromhandle_0_2 == null)
                this.var_getmethodfromhandle_0_2 = this.builderType.GetMethod("GetMethodFromHandle", 2, true).Import();

            return this.var_getmethodfromhandle_0_2;
        }
    }

    public partial class BuilderTypeMonitor : TypeSystemExBase
    {
        private Method var_enter_0_2;

        /// <summary>
        /// Represents the following method:
        /// <para />
        /// public static void Enter(object obj, ref bool lockTaken);<para/>
        /// </summary>
        public Method GetMethod_Enter()
        {
            if (this.var_enter_0_2 == null)
                this.var_enter_0_2 = this.builderType.GetMethod("Enter", 2, true).Import();

            return this.var_enter_0_2;
        }
    }

    public partial class BuilderTypeNotSupportedException
    {
        private Method constructor_string;

        public Method GetConstructor_String()
        {
            if (constructor_string == null)
                constructor_string = this.builderType.GetMethod(".ctor", true, (BuilderType)BuilderTypes.String).Import();

            return constructor_string;
        }
    }

    public partial class BuilderTypeUri : TypeSystemExBase
    {
        /// <summary>
        /// Represents the following method:
        /// <para />
        /// Void .ctor(System.String)<para/>
        /// </summary>
        public Method GetMethod_ctor()
        {
            if (this.var_ctor_0_1 == null)
                this.var_ctor_0_1 = this.builderType.GetMethod(".ctor", 1, true).Import();

            return this.var_ctor_0_1;
        }
    }
}
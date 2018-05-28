using Mono.Cecil;
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
}
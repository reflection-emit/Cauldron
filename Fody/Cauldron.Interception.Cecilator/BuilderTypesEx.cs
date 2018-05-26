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

    public partial class BuilderTypeMethodBase
    {
        public Method GetMethod_GetMethodFromHandle()
        {
            if (this.var_getmethodfromhandle_0_2 == null)
                this.var_getmethodfromhandle_0_2 = this.builderType.GetMethod("GetMethodFromHandle", 2, true).Import();

            return this.var_getmethodfromhandle_0_2;
        }
    }
}
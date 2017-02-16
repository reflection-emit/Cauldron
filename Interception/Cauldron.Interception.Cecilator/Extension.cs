using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public static class Extension
    {
        public static Builder CreateBuilder(this IWeaver weaver) => new Builder(weaver);

        //public static IEnumerable<TypeReference> GetGenericInstances(this GenericInstanceType type)
        //{
        //    var result = new List<TypeReference>();
        //    result.Add(type);

        //    var resolved = type.Resolve();
        //    var genericArgumentsNames = resolved.GenericParameters.Select(x => x.FullName).ToArray();
        //    var genericArguments = type.GenericArguments.ToArray();

        //    if (resolved.BaseType != null)
        //        result.AddRange(resolved.BaseType.GetGenericInstances(genericArgumentsNames, genericArguments));

        //    if (resolved.Interfaces != null && resolved.Interfaces.Count > 0)
        //    {
        //        foreach (var item in resolved.Interfaces)
        //            result.AddRange(item.GetGenericInstances(genericArgumentsNames, genericArguments));
        //    }

        //    return result;
        //}

        internal static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            // https://groups.google.com/forum/#!topic/mono-cecil/mCat5UuR47I by ShdNx

            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return reference;
        }

        internal static TypeReference ResolveType(this TypeReference type, TypeReference inheritingOrImplementingType)
        {
            if (type.HasGenericParameters && inheritingOrImplementingType is GenericInstanceType)
            {
                var genericArgumentNames = inheritingOrImplementingType.GenericParameters.Select(x => x.FullName).ToArray();
                var genericArgumentsOfCurrentType = (inheritingOrImplementingType as GenericInstanceType).GenericArguments.ToArray();

                var genericInstanceType = type as GenericInstanceType;
                var genericArguments = new TypeReference[genericInstanceType.GenericArguments.Count];

                for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                {
                    var t = genericArgumentNames.FirstOrDefault(x => x == genericInstanceType.GenericArguments[i].FullName);

                    if (t == null)
                        genericArguments[i] = genericInstanceType.GenericArguments[i];
                    else
                        genericArguments[i] = genericArgumentsOfCurrentType[Array.IndexOf(genericArgumentNames, t)];
                }

                return type.MakeGenericInstanceType(genericArguments);
            }
            else if (type.HasGenericParameters)
            {
                var genericArgumentNames = type.GenericParameters.Select(x => x.FullName).ToArray();
                var genericArgumentsOfCurrentType = (type as GenericInstanceType).GenericArguments.ToArray();

                var genericInstanceType = type as GenericInstanceType;
                var genericArguments = new TypeReference[genericInstanceType.GenericArguments.Count];

                for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                {
                    var t = genericArgumentNames.FirstOrDefault(x => x == genericInstanceType.GenericArguments[i].FullName);

                    if (t == null)
                        genericArguments[i] = genericInstanceType.GenericArguments[i];
                    else
                        genericArguments[i] = genericArgumentsOfCurrentType[Array.IndexOf(genericArgumentNames, t)];
                }

                return type.MakeGenericInstanceType(genericArguments);
            }
            else
                return type;
        }
    }
}
using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryCreateUsages
    {
        public static readonly MethodUsage[] createGenericMethodUsages;
        public static readonly MethodUsage[] createMethodUsages;
        public static readonly MethodUsage[] createStringMethodUsages;
        public static readonly MethodUsage[] createStringTypeMethodUsages;
        public static readonly MethodUsage[] createTypeMethodUsages;
        public static readonly MethodUsage[] createTypeTypeMethodUsages;

        public static readonly TypeReference objectArray;

        static FactoryCreateUsages()
        {
            objectArray = (TypeReference)BuilderTypes.Object.BuilderType.MakeArray();
            createMethodUsages = BuilderTypes.Factory.GetMethod_Create().FindUsages();
            createTypeMethodUsages = BuilderTypes.Factory.GetMethod_Create(BuilderTypes.Type).FindUsages();
            createStringMethodUsages = BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String).FindUsages();
            createGenericMethodUsages = BuilderTypes.Factory.GetMethod_Create_Generic().FindUsages();
            createTypeTypeMethodUsages = BuilderTypes.Factory.GetMethod_Create(BuilderTypes.Type, (TypeReference)objectArray).FindUsages();
            createStringTypeMethodUsages = BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String, (TypeReference)objectArray).FindUsages();
        }

        public static IEnumerable<MethodUsage> Items =>
                createGenericMethodUsages
                .Concat(createMethodUsages)
                .Concat(createStringMethodUsages)
                .Concat(createStringTypeMethodUsages)
                .Concat(createTypeMethodUsages)
                .Concat(createTypeTypeMethodUsages)
                .Select(x => x);
    }
}
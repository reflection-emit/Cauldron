using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;

namespace Cauldron.Interception.Fody.HelperTypes
{
    internal class GetAssemblyWeaver
    {
        private static Method getAssembly;
        private static Method getTypeInfo;
        private static BuilderType introspectionExtensions;
        private static BuilderType type;
        private static BuilderType typeInfo;

        static GetAssemblyWeaver()
        {
            if (Builder.Current.IsUWP)
            {
                introspectionExtensions = Builder.Current.GetType("System.Reflection.IntrospectionExtensions")?.Import();
                typeInfo = Builder.Current.GetType("System.Reflection.TypeInfo")?.Import();
                getTypeInfo = introspectionExtensions.GetMethod("GetTypeInfo", 1).Import();
                getAssembly = typeInfo.GetMethod("get_Assembly").Import();
            }
            else
            {
                type = Builder.Current.GetType("System.Type")?.Import();
                getAssembly = type.GetMethod("get_Assembly").Import();
            }
        }

        public static CallCoder AddCode(Coder coder, BuilderType builderType)
        {
            if (Builder.Current.IsUWP)
                return coder.Call(getTypeInfo, builderType.Import()).Call(getAssembly);
            else
                return coder.Load(builderType).Call(getAssembly);
        }
    }
}
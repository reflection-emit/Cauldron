using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class ModuleWeaver : IWeaver
    {
        private List<Type> weavers = new List<Type>
        {
            typeof(AnonymouseTypeToInterfaceWeaver),
            typeof(FieldInterceptorWeaver),
            typeof(MethodInterceptorWeaver),
            typeof(PropertyInterceptorWeaver),
            typeof(ChildOfWeaver),
            typeof(MethodOfWeaver),
            typeof(FieldOfWeaver)
        };

        public Action<string> LogError { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            var builder = this.CreateBuilder();
            //var attributes = builder.FindTypesByInterfaces(
            //    "Cauldron.Interception.ILockablePropertyGetterInterceptor",
            //    "Cauldron.Interception.ILockablePropertySetterInterceptor",
            //    "Cauldron.Interception.IPropertyGetterInterceptor",
            //    "Cauldron.Interception.IPropertySetterInterceptor");
            //var fields = builder.FindFieldsByAttributes(attributes);
            //var fieldUsage = fields.SelectMany(x => x.Field.FindUsages());

            var type = builder.FindTypes("FieldOf_Implementer_Test").FirstOrDefault();
            var field = type.Fields["field2"];

            foreach (var item in type.GetRelevantConstructors())
            {
                item.Code
                    .AssignToField(field, "This is a test")
                    .Insert(Cecilator.InsertionPosition.Beginning);
            }

            //Extensions.ModuleWeaver = this;

            //// Check if th module has a reference to Cauldron.Interception
            //var assemblyNameReference = this.ModuleDefinition.AllReferencedAssemblies().FirstOrDefault(x => x.Name.Name == "Cauldron.Interception");
            //if (assemblyNameReference == null)
            //    return;

            //foreach (var weaverType in this.weavers)
            //{
            //    var weaver = Activator.CreateInstance(weaverType, this) as ModuleWeaverBase;
            //    try
            //    {
            //        weaver.Implement();
            //    }
            //    catch (NotImplementedException e)
            //    {
            //        this.LogWarning(e.Message);
            //    }
            //    catch (Exception e)
            //    {
            //        this.LogError(e.GetStackTrace());
            //        this.LogError(e.StackTrace);
            //    }
            //}
        }
    }
}
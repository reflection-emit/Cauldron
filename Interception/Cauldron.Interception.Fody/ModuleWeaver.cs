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
            var attributes = builder.FindTypesByInterfaces(
                "Cauldron.Interception.ILockableMethodInterceptor",
                "Cauldron.Interception.IMethodInterceptor");

            var methods = builder.FindMethodsByAttributes(attributes);
            var test = builder.GetType("Cauldron.Interception.Test.TestClass");

            foreach (var method in methods)
            {
                this.LogInfo(method);
                var variable = method.Method.CreateVariable(method.Attribute);

                method.Method.Code
                    .Assign(variable)
                    .NewObj(method)
                    .Load(variable)
                    .Callvirt(method.Attribute.GetMethod("OnExit"))
                    .Insert(Cecilator.InsertionPosition.Beginning);
            }

            //var fields = builder.FindFieldsByAttributes(attributes);
            //var fieldUsage = fields.SelectMany(x => x.Field.FindUsages());

            //var type = builder.FindTypes("FieldOf_Implementer_Test").FirstOrDefault();
            //var field = type.Fields["field2"];

            //foreach (var item in type.GetRelevantConstructors())
            //{
            //    item.Code
            //        .Assign(field)
            //        .Set("This is a test")
            //        .Insert(Cecilator.InsertionPosition.End);
            //}

            //type.GetMethod("Static_Private_Field_Info")
            //    .Code
            //    .Assign(field)
            //    .Set("My god it works")
            //    .Insert(Cecilator.InsertionPosition.End);

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
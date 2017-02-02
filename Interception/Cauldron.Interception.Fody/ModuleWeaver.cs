using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class ModuleWeaver
    {
        private IEnumerable<AssemblyDefinition> assemblies;
        private IEnumerable<TypeDefinition> types;
        public IAssemblyResolver AssemblyResolver { get; set; }
        public Action<string> LogError { get; set; }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            try
            {
                this.assemblies = this.GetAssemblies();
                this.types = this.assemblies.SelectMany(x => x.Modules).SelectMany(x => x.Types).ToArray();

                this.ImplementMethodInterception();
            }
            catch (Exception e)
            {
                this.LogError(e.Message);
            }
        }

        public IEnumerable<AssemblyDefinition> GetAssemblies() => this.ModuleDefinition.AssemblyReferences.Select(x => this.AssemblyResolver.Resolve(x)).Concat(new AssemblyDefinition[] { this.ModuleDefinition.Assembly }).ToArray();

        private AssemblyDefinition GetCauldronCore()
        {
            var assemblyNameReference = this.ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Cauldron.Core");

            if (assemblyNameReference == null)
                throw new Exception($"The project {this.ModuleDefinition.Name} does not reference to 'Cauldron.Core'. Please add Cauldron.Core to your project.");

            return this.AssemblyResolver.Resolve(assemblyNameReference);
        }

        private TypeDefinition GetType(string interfaceName) => this.GetCauldronCore().Modules.SelectMany(x => x.Types).FirstOrDefault(x => x.FullName == interfaceName);

        private TypeDefinition GetType(Type type) => types.FirstOrDefault(x => x.FullName == type.FullName);

        private void ImplementMethod(MethodDefinition method, CustomAttribute[] attributes)
        {
            this.LogInfo($"Implenting Method interception: {method.Name} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");

            var processor = method.Body.GetILProcessor();

            // Try Catch
            var catchBody = processor.Create(OpCodes.Nop); //il.Create(        OpCodes.Call,        module.Import(typeof(Console).GetMethod("WriteLine", new[] { typeof(object) })));
            var ret = processor.Create(OpCodes.Ret);
            var endFinally = processor.Create(OpCodes.Leave, ret);

            processor.InsertAfter(method.Body.Instructions.Last(), catchBody);
            processor.InsertAfter(catchBody, endFinally);
            processor.InsertAfter(endFinally, ret);

            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = method.Body.Instructions.First(),
                TryEnd = catchBody,
                HandlerStart = catchBody,
                HandlerEnd = ret,
                CatchType = this.ModuleDefinition.Import(typeof(Exception)),
            };

            method.Body.ExceptionHandlers.Add(handler);

            // Try Finally
            var finallyBody = processor.Create(OpCodes.Nop);
            ret = processor.Create(OpCodes.Ret);
            endFinally = processor.Create(OpCodes.Endfinally, ret);

            processor.InsertAfter(method.Body.Instructions.Last(), finallyBody);
            processor.InsertAfter(finallyBody, endFinally);
            processor.InsertAfter(endFinally, ret);

            handler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = method.Body.Instructions.First(),
                TryEnd = finallyBody,
                HandlerStart = finallyBody,
                HandlerEnd = ret,
            };

            method.Body.ExceptionHandlers.Add(handler);
        }

        private void ImplementMethodInterception()
        {
            var methodInterceptorInterface = this.GetType("Cauldron.Core.Interceptors.IMethodInterceptor");

            if (methodInterceptorInterface == null)
                throw new Exception($"Unable to find the interface IMethodInterceptor.");

            var methodInterceptors = types.Where(x => x.Implements(methodInterceptorInterface.Name)).ToArray();

            // find all types with methods that are decorated with any of the found method interceptors
            var methodsAndAttributes = this.ModuleDefinition.Types.SelectMany(x => x.Methods).Where(x => x.HasCustomAttributes)
                .Select(x => new { Method = x, Attributes = x.CustomAttributes.Where(y => methodInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0);

            foreach (var method in methodsAndAttributes)
                this.ImplementMethod(method.Method, method.Attributes);
        }

        private void ImplementPropertyInterception()
        {
            var propertyInterceptorInterface = this.GetType("Cauldron.Core.Interceptors.IPropertyInterceptor");

            if (propertyInterceptorInterface == null)
                throw new Exception($"Unable to find the interface IPropertyInterceptor.");
        }
    }
}
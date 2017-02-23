using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorBase
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IEnumerable<AssemblyDefinition> allAssemblies;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IEnumerable<TypeDefinition> allTypes;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ModuleDefinition moduleDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logError;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logInfo;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logWarning;

        internal CecilatorBase(IWeaver weaver)
        {
            this.logError = weaver.LogError;
            this.logInfo = weaver.LogInfo;
            this.logWarning = weaver.LogWarning;
            this.moduleDefinition = weaver.ModuleDefinition;

            this.allAssemblies = this.GetAllAssemblyDefinitions(this.moduleDefinition.AssemblyReferences)
                  .Concat(new AssemblyDefinition[] { this.moduleDefinition.Assembly }).ToArray();

            this.logInfo("-----------------------------------------------------------------------------");

            foreach (var item in allAssemblies)
                this.logInfo("<<Assembly>> " + item.FullName);

            this.allTypes = this.allAssemblies.SelectMany(x => x.Modules).Where(x => x != null).SelectMany(x => x.Types).Where(x => x != null).Concat(this.moduleDefinition.Types).ToArray();
            this.logInfo("-----------------------------------------------------------------------------");

            this.Identification = GenerateName();
        }

        internal CecilatorBase(CecilatorBase builderBase)
        {
            this.logError = builderBase.logError;
            this.logInfo = builderBase.logInfo;
            this.logWarning = builderBase.logWarning;
            this.moduleDefinition = builderBase.moduleDefinition;
            this.allAssemblies = builderBase.allAssemblies;
            this.allTypes = builderBase.allTypes;

            this.Identification = GenerateName();
        }

        public string Identification { get; private set; }

        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        internal ParamResult AddParameter(ILProcessor processor, TypeReference targetType, object parameter)
        {
            var result = new ParamResult();

            if (parameter == null)
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldnull));
                return result;
            }

            var type = parameter.GetType();

            if (type == typeof(string))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldstr, parameter.ToString()));
                result.Type = this.GetTypeDefinition(typeof(string));
            }
            else if (type == typeof(FieldDefinition))
            {
                var value = parameter as FieldDefinition;

                if (!value.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value.CreateFieldReference()));
                result.Type = value.FieldType;
            }
            else if (type == typeof(FieldReference))
            {
                var value = parameter as FieldReference;
                var fieldDef = value.Resolve();

                if (!fieldDef.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                result.Instructions.Add(processor.Create(fieldDef.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value));
                result.Type = value.FieldType;
            }
            else if (type == typeof(Field))
            {
                var value = parameter as Field;

                if (!value.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value.fieldRef));
                result.Type = value.fieldRef.FieldType;
            }
            else if (type == typeof(int))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)parameter));
                result.Type = this.GetTypeDefinition(typeof(int));
            }
            else if (type == typeof(uint))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(uint)parameter));
                result.Type = this.GetTypeDefinition(typeof(uint));
            }
            else if (type == typeof(bool))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (bool)parameter ? 1 : 0));
                result.Type = this.GetTypeDefinition(typeof(bool));
            }
            else if (type == typeof(char))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (char)parameter));
                result.Type = this.GetTypeDefinition(typeof(char));
            }
            else if (type == typeof(short))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (short)parameter));
                result.Type = this.GetTypeDefinition(typeof(short));
            }
            else if (type == typeof(ushort))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (ushort)parameter));
                result.Type = this.GetTypeDefinition(typeof(ushort));
            }
            else if (type == typeof(byte))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(byte)parameter));
                result.Type = this.GetTypeDefinition(typeof(byte));
            }
            else if (type == typeof(sbyte))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(sbyte)parameter));
                result.Type = this.GetTypeDefinition(typeof(sbyte));
            }
            else if (type == typeof(long))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, (long)parameter));
                result.Type = this.GetTypeDefinition(typeof(long));
            }
            else if (type == typeof(ulong))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, (long)(ulong)parameter));
                result.Type = this.GetTypeDefinition(typeof(ulong));
            }
            else if (type == typeof(double))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R8, (double)parameter));
                result.Type = this.GetTypeDefinition(typeof(double));
            }
            else if (type == typeof(float))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R4, (float)parameter));
                result.Type = this.GetTypeDefinition(typeof(float));
            }
            else if (type == typeof(LocalVariable))
            {
                var value = AddVariableDefinitionToInstruction(processor, result.Instructions, (parameter as LocalVariable).variable);
                result.Type = value.VariableType;
            }
            else if (type == typeof(VariableDefinition))
            {
                var value = AddVariableDefinitionToInstruction(processor, result.Instructions, parameter);
                result.Type = value.VariableType;
            }
            else if (type == typeof(Crumb))
            {
                var crumb = parameter as Crumb;

                switch (crumb.CrumbType)
                {
                    case CrumbTypes.Exception:
                    case CrumbTypes.Parameters:
                        {
                            var variable = crumb.Context.LocalVariables[crumb.Name];
                            result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable.variable));
                            break;
                        }
                    case CrumbTypes.This:
                        result.Instructions.Add(processor.Create(crumb.Context.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            else if (type == typeof(BuilderType))
            {
                var bt = parameter as BuilderType;
                result.Instructions.AddRange(this.TypeOf(processor, bt.typeReference));
                result.Type = this.GetTypeDefinition(typeof(Type));
            }
            else if (type == typeof(Method))
            {
                var method = parameter as Method;

                result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.methodReference));
                result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType.typeReference));
                result.Instructions.Add(processor.Create(OpCodes.Call,
                    this.moduleDefinition.Import(
                        this.GetTypeDefinition(typeof(System.Reflection.MethodBase))
                            .Methods.FirstOrDefault(x => x.Name == "GetMethodFromHandle" && x.Parameters.Count == 2))));
            }
            else if (type == typeof(ParameterDefinition))
            {
                var value = parameter as ParameterDefinition;
                result.Instructions.Add(processor.Create(OpCodes.Ldarg_S, value));
                result.Type = value.ParameterType;
            }
            else if (parameter is IEnumerable<Instruction>)
            {
                foreach (var item in parameter as IEnumerable<Instruction>)
                    result.Instructions.Add(item);
            }

            if (result.Type != null && result.Type.Resolve() == null)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));

            if (result.Type != null && result.Type.IsValueType && !targetType.IsValueType)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));

            return result;
        }

        internal IEnumerable<TypeReference> GetBaseClasses(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef != null && typeDef.BaseType != null)
            {
                yield return typeDef.BaseType;
                foreach (var item in GetBaseClasses(typeDef.BaseType))
                    yield return item;
            }
        }

        internal IEnumerable<TypeReference> GetInterfaces(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.Interfaces != null && typeDef.Interfaces.Count > 0)
                return type.Recursive(x => x.Resolve().Interfaces).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetInterfaces(typeDef.BaseType);

            return new TypeReference[0];
        }

        internal IEnumerable<TypeReference> GetNestedTypes(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.NestedTypes != null && typeDef.NestedTypes.Count > 0)
                return type.Recursive(x => x.Resolve().NestedTypes).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetNestedTypes(typeDef.BaseType);

            return new TypeReference[0];
        }

        internal TypeDefinition GetTypeDefinition(Type type)
        {
            var result = this.allTypes.FirstOrDefault(x => x.FullName == type.FullName);

            if (result == null)
                throw new Exception($"Unable to proceed. The type '{type.FullName}' was not found.");

            return result;
        }

        internal void LogError(object value)
        {
            if (value is string)
                this.logError(value as string);
            else
                this.logError(value.ToString());
        }

        internal void LogInfo(object value)
        {
            if (value is string)
                this.logInfo(value as string);
            else
                this.logInfo(value.ToString());
        }

        internal void LogWarning(object value)
        {
            if (value is string)
                this.logWarning(value as string);
            else
                this.logWarning(value.ToString());
        }

        protected IEnumerable<Instruction> TypeOf(ILProcessor processor, TypeReference type)
        {
            return new Instruction[] {
                processor.Create(OpCodes.Ldtoken, type),
                processor.Create(OpCodes.Call,
                    this.moduleDefinition.Import(
                        this.GetTypeDefinition(typeof(Type))
                            .Methods.FirstOrDefault(x=>x.Name == "GetTypeFromHandle" && x.Parameters.Count == 1)))
            };
        }

        private static VariableDefinition AddVariableDefinitionToInstruction(ILProcessor processor, List<Instruction> instructions, object p)
        {
            var value = p as VariableDefinition;
            var index = value.Index;

            switch (index)
            {
                case 0: instructions.Add(processor.Create(OpCodes.Ldloc_0)); break;
                case 1: instructions.Add(processor.Create(OpCodes.Ldloc_1)); break;
                case 2: instructions.Add(processor.Create(OpCodes.Ldloc_2)); break;
                case 3: instructions.Add(processor.Create(OpCodes.Ldloc_3)); break;
                default:
                    instructions.Add(processor.Create(OpCodes.Ldloc_S, value));
                    break;
            }

            return value;
        }

        private void GetAllAssemblyDefinitions(IEnumerable<AssemblyNameReference> target, List<AssemblyDefinition> result)
        {
            result.AddRange(target.Select(x => this.moduleDefinition.AssemblyResolver.Resolve(x)).Where(x => x != null));

            foreach (var item in target)
            {
                var assembly = this.moduleDefinition.AssemblyResolver.Resolve(item);

                if (assembly == null)
                    continue;

                if (result.Contains(assembly))
                    continue;

                result.Add(assembly);

                if (assembly.MainModule.HasAssemblyReferences)
                {
                    foreach (var a in assembly.Modules)
                        GetAllAssemblyDefinitions(a.AssemblyReferences, result);
                }
            }
        }

        private IEnumerable<AssemblyDefinition> GetAllAssemblyDefinitions(IEnumerable<AssemblyNameReference> target)
        {
            var result = new List<AssemblyDefinition>();
            result.AddRange(target.Select(x => this.moduleDefinition.AssemblyResolver.Resolve(x)).Where(x => x != null));

            foreach (var item in target)
            {
                var assembly = this.moduleDefinition.AssemblyResolver.Resolve(item);

                if (assembly == null)
                    continue;

                result.Add(assembly);

                if (assembly.MainModule.HasAssemblyReferences)
                {
                    foreach (var a in assembly.Modules)
                        this.GetAllAssemblyDefinitions(a.AssemblyReferences, result);
                }
            }

            return result.Distinct(new AssemblyDefinitionEqualityComparer());
        }
    }
}
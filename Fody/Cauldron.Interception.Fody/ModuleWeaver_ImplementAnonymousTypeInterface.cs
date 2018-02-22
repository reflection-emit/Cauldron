using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementAnonymousTypeInterface(Builder builder)
        {
            var cauldronCoreExtension = builder.GetType("Cauldron.Interception.Extensions");
            var createTypeMethod = cauldronCoreExtension.GetMethod("CreateType", 1).FindUsages().ToArray();
            var createdTypes = new Dictionary<string, BuilderType>();

            if (!createTypeMethod.Any())
                return;

            using (new StopwatchLog(this, "anonymous type to interface"))
            {
                foreach (var item in createTypeMethod)
                {
                    this.Log(LogTypes.Info, $"Implementing anonymous to interface {item}");
                    var interfaceToImplement = item.GetGenericArgument(0);

                    if (interfaceToImplement == null || !interfaceToImplement.IsInterface)
                    {
                        this.Log(LogTypes.Error, interfaceToImplement, $"{interfaceToImplement.Fullname} is not an interface.");
                        continue;
                    }

                    try
                    {
                        var type = item.GetPreviousInstructionObjectType();

                        if (type.Fullname.GetHashCode() == "System.Object".GetHashCode() && type.Fullname == "System.Object")
                        {
                            type = item.GetLastNewObjectType();

                            if (type.Fullname.GetHashCode() == "System.Object".GetHashCode() && type.Fullname == "System.Object")
                            {
                                this.Log(LogTypes.Error, item.HostMethod, $"Error in CreateObject in method '{item.HostMethod}'. Unable to detect anonymous type.");
                                continue;
                            }
                        }

                        if (createdTypes.ContainsKey(interfaceToImplement.Fullname))
                        {
                            item.Replace(CreateAssigningMethod(type, createdTypes[interfaceToImplement.Fullname], interfaceToImplement, item.HostMethod));
                            continue;
                        }

                        var anonymousTypeName = $"<>f__{interfaceToImplement.Name}_Cauldron_AnonymousType{counter++}";
                        this.Log($"- Creating new type: {type.Namespace}.{anonymousTypeName}");

                        var newType = builder.CreateType("", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable, anonymousTypeName);
                        newType.AddInterface(interfaceToImplement);

                        // Implement the methods
                        foreach (var method in interfaceToImplement.Methods.Where(x => !x.IsPropertyGetterSetter))
                            newType.CreateMethodImplicitInterface(method);

                        // Implement the properties
                        foreach (var property in interfaceToImplement.Properties)
                            newType.CreateProperty(Modifiers.Public | Modifiers.Overrides, property.ReturnType, property.Name);

                        // Create ctor
                        newType.CreateConstructor();
                        newType.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

                        createdTypes.Add(interfaceToImplement.Fullname, newType);

                        item.Replace(CreateAssigningMethod(type, newType, interfaceToImplement, item.HostMethod));
                    }
                    catch (Exception e)
                    {
                        this.Log(e, item.ToHostMethodInstructionsString());

                        throw;
                    }
                }
            }
        }
    }
}
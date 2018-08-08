using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public sealed class Weaver_ImplementAnonymousTypeInterface
{
    public static string Name = "Anonymous Type To Interface";
    public static int Priority = -1;
    private static int counter = 0;

    [Display("Anonymous Type To Interface")]
    public static void ImplementAnonymousTypeInterface(Builder builder)
    {
        var cauldronCoreExtension = builder.GetType("Cauldron.Interception.ExtensionsInterception");
        var createTypeMethod = cauldronCoreExtension.GetMethod("CreateType", 1).FindUsages().ToArray();
        var createdTypes = new Dictionary<string, BuilderType>();

        if (!createTypeMethod.Any())
            return;

        foreach (var item in createTypeMethod)
        {
            builder.Log(LogTypes.Info, $"Implementing anonymous to interface {item}");
            var interfaceToImplement = item.GetGenericArgument(0);

            if (interfaceToImplement == null || !interfaceToImplement.IsInterface)
            {
                builder.Log(LogTypes.Error, interfaceToImplement, $"{interfaceToImplement.Fullname} is not an interface.");
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
                        builder.Log(LogTypes.Error, item.HostMethod, $"Error in CreateObject in method '{item.HostMethod}'. Unable to detect anonymous type.");
                        continue;
                    }
                }

                if (createdTypes.ContainsKey(interfaceToImplement.Fullname))
                {
                    item.Replace(CreateAssigningMethod(builder, type, createdTypes[interfaceToImplement.Fullname], interfaceToImplement, item.HostMethod));
                    continue;
                }

                var anonymousTypeName = $"<>f__{interfaceToImplement.Name}_Cauldron_AnonymousType{counter++}";
                builder.Log(LogTypes.Info, $"- Creating new type: {type.Namespace}.{anonymousTypeName}");

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

                item.Replace(CreateAssigningMethod(builder, type, newType, interfaceToImplement, item.HostMethod));
            }
            catch (Exception e)
            {
                builder.Log(LogTypes.Error, e.GetStackTrace() + "\r\n" + item.ToHostMethodInstructionsString());

                throw;
            }
        }
    }

    private static Method CreateAssigningMethod(Builder builder, BuilderType anonSource, BuilderType anonTarget, BuilderType anonTargetInterface, Method method)
    {
        var name = $"<{counter++}>f__Anon_Assign";
        var assignMethod = method.OriginType.CreateMethod(Modifiers.PrivateStatic, anonTarget, name, anonSource);
        assignMethod.NewCoder()
            .Context(context =>
            {
                var resultVar = context.GetOrCreateReturnVariable();
                context.SetValue(resultVar, x => x.NewObj(anonTarget.ParameterlessContructor));

                foreach (var property in anonSource.Properties)
                {
                    try
                    {
                        var targetProperty = anonTarget.GetProperty(property.Name);
                        if (property.ReturnType.Fullname != targetProperty.ReturnType.Fullname)
                        {
                            builder.Log(LogTypes.Error, property, $"The property '{property.Name}' does not have the expected return type. Is: {property.ReturnType.Fullname} Expected: {targetProperty.ReturnType.Fullname}");
                            continue;
                        }
                        context.Load(resultVar).Call(targetProperty.Setter, x => x.Load(CodeBlocks.GetParameter(0)).Call(property.Getter));
                    }
                    catch (MethodNotFoundException)
                    {
                        builder.Log(LogTypes.Warning, anonTarget, $"The property '{property.Name}' does not exist in '{anonTarget.Name}'");
                    }
                }

                return context.Load(resultVar).Return();
            })
            .Replace();

        assignMethod.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

        return assignMethod;
    }
}
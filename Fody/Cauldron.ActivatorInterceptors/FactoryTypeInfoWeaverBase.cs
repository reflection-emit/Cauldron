using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryTypeInfoWeaverBase
    {
        public const string NoIDisposableObjectExceptionText = "An object with creation policy 'Singleton' with an implemented 'IDisposable' must also implement the 'IDisposableObject' interface.";
        public readonly TypeReference childType;
        public readonly ComponentAttributeValues componentAttributeValue;
        public readonly BuilderType componentType;
        public readonly Coder componentTypeCtor;
        public readonly bool isIEnumerable;

        /// <summary>
        /// Used to make implementations unique even with the same name
        /// </summary>
        private static volatile int indexer = 0;

        internal FactoryTypeInfoWeaverBase(ComponentAttributeValues componentAttributeValue, BuilderType componentType, Coder componentTypeCtor, (TypeReference childType, bool isSuccessful) childType)
        {
            this.componentType = componentType;
            this.componentAttributeValue = componentAttributeValue;
            this.componentTypeCtor = componentTypeCtor;
            this.childType = childType.childType;
            this.isIEnumerable = childType.isSuccessful;

            // Implement the methods
            AddCreateInstanceMethod(builder, cauldron, BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance_1(), component, componentAttributeValue, componentType).Replace();
            AddCreateInstanceMethod(builder, cauldron, BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance(), component, componentAttributeValue, componentType).Replace();
        }

        private static Coder AddContext(Builder builder, Coder context, AttributedType component)
        {
            var componentAttributeValues = new ComponentAttributeValues(component);
            var ctors = GetComponentConstructors(component).ToArray();

            if (ctors.Length > 0 && context.AssociatedMethod.Parameters.Length > 0)
            {
                var parameterlessCtorAlreadyHandled = false;

                for (int index = 0; index < ctors.Length; index++)
                {
                    builder.Log(LogTypes.Info, "- " + ctors[index].Fullname);

                    var ctor = ctors[index];
                    // add a EditorBrowsable attribute
                    ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                    var ctorParameters = ctor.Parameters;

                    if (ctorParameters.Length > 0)
                    {
                        // In this case we have to find a parameterless constructor first
                        if (component.Type.ParameterlessContructor != null && !parameterlessCtorAlreadyHandled && component.Type.ParameterlessContructor.IsPublicOrInternal)
                        {
                            context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull().OrOr(y => y.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(0)), then =>
                            {
                                then.NewObj(component.Type.ParameterlessContructor);

                                if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                    then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                                return then.Return();
                            });
                            parameterlessCtorAlreadyHandled = true;
                        }

                        context.If(@if =>
                        {
                            var resultCoder = @if.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(ctorParameters.Length);
                            for (int i = 0; i < ctorParameters.Length; i++)
                                resultCoder = resultCoder.AndAnd(x => x.Load(CodeBlocks.GetParameter(0)).ArrayElement(i).Is(ctorParameters[i]));

                            return resultCoder;
                        }, @then =>
                        {
                            if (ctor.Name == ".ctor")
                            {
                                then.NewObj(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                                if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                    then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);
                            }
                            else
                            {
                                then.Call(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                                if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                    then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);
                            }

                            return then.Return();
                        });
                    }
                    else if (ctorParameters.Length == 0)
                    {
                        CreateComponentParameterlessCtor(context, ctor, componentAttributeValues);
                        parameterlessCtorAlreadyHandled = true;
                    }
                }

                context.ThrowNew(typeof(NotImplementedException), x =>
                    x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), FactoryTypeInfoWeaver.unknownConstructorText,
                        x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);
            }
            else if (ctors.Length > 0)
            {
                var ctor = ctors.FirstOrDefault(x => x.Parameters.Length == 0);
                if (ctor == null)
                    context.ThrowNew(typeof(NotImplementedException), x =>
                        x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), FactoryTypeInfoWeaver.unknownConstructorText,
                            x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);
                else
                {
                    ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                    CreateComponentParameterlessCtor(context, ctor, componentAttributeValues);
                }
            }
            else
            {
                context.ThrowNew(typeof(NotImplementedException), x =>
                    x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), FactoryTypeInfoWeaver.unknownConstructorText,
                        x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);

                builder.Log(LogTypes.Error, component.Type, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute or the constructor is not public or internal");
            }

            return context;
        }

        private static Coder AddCreateInstanceMethod(Builder builder, BuilderType cauldron, Method createInstanceInterfaceMethod, AttributedType component, ComponentAttributeValues componentAttributeValue, BuilderType componentType)
        {
            var instanceFieldName = $"<{component.Type}>_componentInstance";
            return componentType.CreateMethod(Modifiers.Public | Modifiers.Overrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                .NewCoder()
                .Context(x =>
                {
                    if (componentAttributeValue.Policy == 1)
                    {
                        var instanceSyncObjectName = $"<{component.Type}>_componentInstanceSyncObject";
                        var instanceField = cauldron.GetField(instanceFieldName, false) ?? cauldron.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                        var instanceFieldSyncObject = cauldron.GetField(instanceSyncObjectName, false) ?? cauldron.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceSyncObjectName);
                        var instancedCreator = componentType.CreateMethod(Modifiers.Private, createInstanceInterfaceMethod.ReturnType, $"<{component.Type.Fullname}>__CreateInstance_{indexer++}", createInstanceInterfaceMethod.Parameters);
                        var lockTaken = instancedCreator.GetOrCreateVariable((BuilderType)BuilderTypes.Boolean);
                        instancedCreator.NewCoder()
                            .SetValue(lockTaken, false)
                            .Try(@try => @try
                                .Call(BuilderTypes.Monitor.GetMethod_Enter(), instanceFieldSyncObject, lockTaken).End
                                .If(@if => @if.Load(instanceField).IsNull(),
                                    then => AddContext(builder, then, component),
                                    @else => @else.Load(instanceField)))
                            .Finally(@finally => @finally.If(@if => @if.Load(lockTaken).Is(true), then => @then.Call(BuilderTypes.Monitor.GetMethod_Exit(), instanceFieldSyncObject)))
                            .EndTry()
                            .Return()
                            .Replace();
                        cauldron.CreateStaticConstructor().NewCoder().SetValue(instanceFieldSyncObject, y => y.NewObj(BuilderTypes.Object.GetMethod_ctor())).Insert(InsertionPosition.Beginning);

                        x.If(@if => @if.Load(instanceField).IsNotNull(), then => then.Load(instanceField).Return());
                        if (createInstanceInterfaceMethod.Parameters.Length == 0)
                            x.SetValue(instanceField, m => m.Call(instancedCreator));
                        else
                            x.SetValue(instanceField, m => m.Call(instancedCreator, CodeBlocks.GetParameters()));
                        // every singleton that implements the idisposable interface has also to
                        // implement the IDisposableObject interface this is because we want to know if
                        // an instance was disposed (somehow)
                        if (component.Type.Implements(BuilderTypes.IDisposable))
                        {
                            if (!component.Type.Implements(BuilderTypes.IDisposableObject))
                                builder.Log(LogTypes.Info, component.Type + " : " + NoIDisposableObjectExceptionText);
                            else
                            {
                                // Create an event handler method
                                var eventHandlerMethod =
                                componentType.GetMethod($"<IDisposableObject>_Handler", 2, false) ??
                                componentType.CreateMethod(Modifiers.Private, $"<IDisposableObject>_Handler", BuilderTypes.Object, BuilderTypes.EventArgs);
                                eventHandlerMethod.NewCoder()
                                    .If(ehIf => ehIf.Load(CodeBlocks.GetParameter(0)).IsNotNull(), ehThen => ehThen.Load(CodeBlocks.GetParameter(0)).As(BuilderTypes.IDisposable).Call(BuilderTypes.IDisposable.GetMethod_Dispose()))
                                    .SetValue(instanceField, null)
                                    .If(@if => @if.Load(instanceField).IsNotNull(), @then => @then
                                        .Load(instanceField).As(BuilderTypes.IDisposableObject).Call(BuilderTypes.IDisposableObject.GetMethod_remove_Disposed(), o => o.NewObj(BuilderTypes.EventHandler.GetConstructor(), CodeBlocks.This, eventHandlerMethod)))
                                    .Return()
                                    .Replace();

                                x.Load(instanceField).As(BuilderTypes.IDisposableObject).Call(BuilderTypes.IDisposableObject.GetMethod_add_Disposed(), o => o.NewObj(BuilderTypes.EventHandler.GetConstructor(), CodeBlocks.This, eventHandlerMethod));
                            }
                        }

                        x.Load(instanceField).Return();
                    }
                    else
                        AddContext(builder, x, component);

                    return x;
                });
        }

        private static void CreateComponentParameterlessCtor(Coder context, Method contructor, ComponentAttributeValues componentAttributeValues)
        {
            if (context.AssociatedMethod.Parameters.Length > 0)
                context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull().OrOr(y => y.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(0)), then =>
                {
                    if (contructor.Name == ".ctor")
                        then.NewObj(contructor);
                    else
                        then.Call(contructor);

                    if (componentAttributeValues.InvokeOnObjectCreationEvent)
                        then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                    return then.Return();
                });
            else
            {
                if (contructor.Name == ".ctor")
                    context.NewObj(contructor);
                else
                    context.Call(contructor);

                if (componentAttributeValues.InvokeOnObjectCreationEvent)
                    context.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                context.Return();
            }
        }

        private static IEnumerable<Method> GetComponentConstructors(AttributedType component)
        {
            var methods = component.Type.GetMethods().OrderBy(x => x.Parameters.Length);

            // First all ctors with component Attribute
            foreach (var item in methods)
            {
                if (item.Name != ".ctor")
                    continue;

                if (item.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                    yield return item;
            }

            // Get all properties with component attribute
            foreach (var item in component.Type.GetAllProperties())
            {
                if (item.Getter == null)
                    continue;

                if (item.IsStatic && item.Getter.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                    yield return item.Getter;
            }

            // Then all static methods with component attribute
            foreach (var item in methods)
            {
                if (!item.IsStatic)
                    continue;

                if (item.Name == ".cctor")
                    continue;

                if (item.Name.StartsWith("set_"))
                    continue;

                if (item.Name.StartsWith("get_"))
                    continue;

                if (item.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                    yield return item;
            }

            // At last all ctors without component Attribute
            foreach (var item in methods)
            {
                if (item.Name != ".ctor")
                    continue;

                if (item.DeclaringType != component.Type)
                    continue;

                if (item.IsPublicOrInternal && !item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                    yield return item;
            }
        }
    }
}
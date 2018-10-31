using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.ActivatorInterceptors
{
    internal abstract class FactoryTypeInfoWeaverBase
    {
        public const string NoIDisposableObjectExceptionText = "An object with creation policy 'Singleton' with an implemented 'IDisposable' must also implement the 'IDisposableObject' interface.";
        public readonly TypeReference childType;
        public readonly ComponentAttributeValues componentAttributeValue;
        public readonly BuilderType componentInfoType;
        public readonly BuilderType componentType;
        public readonly Coder componentTypeCtor;
        public readonly bool isIEnumerable;

        /// <summary>
        /// Used to make implementations unique even with the same name
        /// </summary>
        private static volatile int indexer = 0;

        private readonly Builder builder;

        internal FactoryTypeInfoWeaverBase(ComponentAttributeValues componentAttributeValue, BuilderType componentInfoType, Coder componentTypeCtor, BuilderType componentType, (TypeReference childType, bool isSuccessful) childType)
        {
            this.builder = Builder.Current;

            this.componentType = componentType;
            this.componentInfoType = componentInfoType;
            this.componentAttributeValue = componentAttributeValue;
            this.componentTypeCtor = componentTypeCtor;
            this.childType = childType.childType;
            this.isIEnumerable = childType.isSuccessful;

            this.OnInitialize();

            // Implement the methods
            this.AddCreateInstanceMethod(BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance_1())?.Replace();
            this.AddCreateInstanceMethod(BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance())?.Replace();

            this.ImplementProperties();
        }

        public static implicit operator BuilderType(FactoryTypeInfoWeaverBase factoryType) => factoryType.componentInfoType;

        protected virtual Coder AddCreateInstanceMethod(Method createInstanceInterfaceMethod)
        {
            var instanceFieldName = $"<{this.componentInfoType}>_componentInstance";
            return this.componentInfoType.CreateMethod(Modifiers.Public | Modifiers.Overrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                  .NewCoder()
                  .Context(x =>
                  {
                      if (this.componentAttributeValue.Policy == 1)
                      {
                          var instanceSyncObjectName = $"<{this.componentType}>_componentInstanceSyncObject";
                          var instanceField = FactoryTypeInfoWeaver.cauldronInterceptionHelper.GetField(instanceFieldName, false) ?? FactoryTypeInfoWeaver.cauldronInterceptionHelper.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                          var instanceFieldSyncObject = FactoryTypeInfoWeaver.cauldronInterceptionHelper.GetField(instanceSyncObjectName, false) ?? FactoryTypeInfoWeaver.cauldronInterceptionHelper.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceSyncObjectName);
                          var instancedCreator = this.componentInfoType.CreateMethod(Modifiers.Private, createInstanceInterfaceMethod.ReturnType, $"<{this.componentType.Fullname.GetValidName()}>__CreateInstance_{indexer++}", createInstanceInterfaceMethod.Parameters);
                          var lockTaken = instancedCreator.GetOrCreateVariable((BuilderType)BuilderTypes.Boolean);
                          instancedCreator.NewCoder()
                              .SetValue(lockTaken, false)
                              .Try(@try => @try
                                  .Call(BuilderTypes.Monitor.GetMethod_Enter(), instanceFieldSyncObject, lockTaken).End
                                  .If(@if => @if.Load(instanceField).IsNull(),
                                      then => this.AddContext(then),
                                      @else => @else.Load(instanceField)))
                              .Finally(@finally => @finally.If(@if => @if.Load(lockTaken).Is(true), then => @then.Call(BuilderTypes.Monitor.GetMethod_Exit(), instanceFieldSyncObject)))
                              .EndTry()
                              .Return()
                              .Replace();
                          FactoryTypeInfoWeaver.cauldronInterceptionHelper.CreateStaticConstructor().NewCoder().SetValue(instanceFieldSyncObject, y => y.NewObj(BuilderTypes.Object.GetMethod_ctor())).Insert(InsertionPosition.Beginning);

                          x.If(@if => @if.Load(instanceField).IsNotNull(), then => then.Load(instanceField).Return());
                          if (createInstanceInterfaceMethod.Parameters.Length == 0)
                              x.SetValue(instanceField, m => m.Call(instancedCreator));
                          else
                              x.SetValue(instanceField, m => m.Call(instancedCreator, CodeBlocks.GetParameters()));
                          // every singleton that implements the idisposable interface has also to
                          // implement the IDisposableObject interface this is because we want to know if
                          // an instance was disposed (somehow)
                          if (this.componentType.Implements(BuilderTypes.IDisposable))
                          {
                              if (!this.componentType.Implements(BuilderTypes.IDisposableObject))
                                  this.builder.Log(LogTypes.Info, this.componentType + " : " + NoIDisposableObjectExceptionText);
                              else
                              {
                                  // Create an event handler method
                                  var eventHandlerMethod =
                                    this.componentInfoType.GetMethod($"<IDisposableObject>_Handler", 2, false) ??
                                    this.componentInfoType.CreateMethod(Modifiers.Private, $"<IDisposableObject>_Handler", BuilderTypes.Object, BuilderTypes.EventArgs);
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
                          this.AddContext(x);

                      return x;
                  });
        }

        protected virtual void OnChildTypeSet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            propertyResult.Getter.NewCoder().Load(this.isIEnumerable ? this.childType : null).Return().Replace();
        }

        protected virtual void OnContractNameSet(Property propertyResult)
        {
            if (string.IsNullOrEmpty(this.componentAttributeValue.ContractName))
                this.componentTypeCtor.SetValue(propertyResult.BackingField, x =>
                    x.Load(this.componentAttributeValue.ContractType).Call(BuilderTypes.Type.GetMethod_get_FullName()));
            else
            {
                propertyResult.BackingField.Remove();
                propertyResult.Getter.NewCoder().Load(this.componentAttributeValue.ContractName).Return().Replace();
            }
        }

        protected virtual void OnContractTypeSet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            if (this.componentAttributeValue.ContractType == null)
                propertyResult.Getter.NewCoder().Load(value: null).Return().Replace();
            else
                propertyResult.Getter.NewCoder().Load(this.componentAttributeValue.ContractType).Return().Replace();
        }

        protected virtual void OnCreationPolicySet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            propertyResult.Getter.NewCoder().Load(this.componentAttributeValue.Policy).Return().Replace();
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnInstanceSet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            if (this.componentAttributeValue.Policy == 0)
            {
                propertyResult.Getter.NewCoder().Load(value: null).Return().Replace();
                propertyResult.Setter.NewCoder().Return().Replace();
            }
            else
            {
                var instanceFieldName = $"<{this.componentType}>_componentInstance";
                var instanceField = FactoryTypeInfoWeaver.cauldronInterceptionHelper.GetField(instanceFieldName, false) ?? FactoryTypeInfoWeaver.cauldronInterceptionHelper.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                propertyResult.Getter.NewCoder().Load(instanceField).Return().Replace();
                propertyResult.Setter.NewCoder().SetValue(instanceField, CodeBlocks.GetParameter(0)).Return().Replace();
            }
        }

        protected virtual void OnIsEnumerableSet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            propertyResult.Getter.NewCoder().Load(this.isIEnumerable).Return().Replace();
        }

        protected virtual void OnPrioritySet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            propertyResult.Getter.NewCoder().Load(this.componentAttributeValue.Priority).Return().Replace();
        }

        protected virtual void OnTypeSet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            propertyResult.Getter.NewCoder().Load(this.componentType).Return().Replace();
        }

        private Coder AddContext(Coder context)
        {
            var ctors = this.GetComponentConstructors().OrderByDescending(x=> x.GenericParameters?.Count ?? 0).ToArray();

            if (ctors.Length > 0 && context.AssociatedMethod.Parameters.Length > 0)
            {
                var parameterlessCtorAlreadyHandled = false;

                for (int index = 0; index < ctors.Length; index++)
                {
                    this.builder.Log(LogTypes.Info, "- " + ctors[index].Fullname);

                    var ctor = ctors[index];
                    // add a EditorBrowsable attribute
                    ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                    var ctorParameters = ctor.Parameters;

                    if (ctorParameters.Length > 0)
                    {
                        // In this case we have to find a parameterless constructor first
                        if (this.componentType.ParameterlessContructor != null && !parameterlessCtorAlreadyHandled && this.componentType.ParameterlessContructor.IsPublicOrInternal)
                        {
                            var paramlessCtor =
                                this.componentType.HasGenericParameters && !this.componentType.ParameterlessContructor.HasGenericParameters ?
                                this.componentType.ParameterlessContructor.MakeGeneric(this.componentType.GenericParameters.ToArray()) :
                                this.componentType.ParameterlessContructor;

                            context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull().OrOr(y => y.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(0)), then =>
                            {
                                then.NewObj(paramlessCtor);

                                if (this.componentAttributeValue.InvokeOnObjectCreationEvent)
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

                                if (this.componentAttributeValue.InvokeOnObjectCreationEvent)
                                    then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);
                            }
                            else
                            {
                                then.Call(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                                if (this.componentAttributeValue.InvokeOnObjectCreationEvent)
                                    then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);
                            }

                            return then.Return();
                        });
                    }
                    else if (ctorParameters.Length == 0 && !parameterlessCtorAlreadyHandled)
                    {
                        this.CreateComponentParameterlessCtor(context, ctor);
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
                    this.CreateComponentParameterlessCtor(context, ctor);
                }
            }
            else
            {
                context.ThrowNew(typeof(NotImplementedException), x =>
                    x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), FactoryTypeInfoWeaver.unknownConstructorText,
                        x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);

                this.builder.Log(LogTypes.Error, this.componentType, $"The component '{this.componentType.Fullname}' has no ComponentConstructor attribute or the constructor is not public or internal");
            }

            return context;
        }

        private void CreateComponentParameterlessCtor(Coder context, Method contructor)
        {
            if (context.AssociatedMethod.Parameters.Length > 0)
                context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull().OrOr(y => y.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(0)), then =>
                {
                    if (contructor.Name == ".ctor")
                        then.NewObj(contructor);
                    else
                        then.Call(contructor);

                    if (this.componentAttributeValue.InvokeOnObjectCreationEvent)
                        then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                    return then.Return();
                });
            else
            {
                if (contructor.Name == ".ctor")
                    context.NewObj(contructor);
                else
                    context.Call(contructor);

                if (this.componentAttributeValue.InvokeOnObjectCreationEvent)
                    context.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                context.Return();
            }
        }

        private IEnumerable<Method> GetComponentConstructors()
        {
            var methods = this.componentType.GetMethods().OrderBy(x => x.Parameters.Length);

            // First all ctors with component Attribute
            foreach (var item in methods)
            {
                if (item.Name != ".ctor")
                    continue;

                if (item.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                    yield return item;
            }

            // Get all properties with component attribute
            foreach (var item in this.componentType.GetAllProperties())
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

                if (item.DeclaringType != this.componentType)
                    continue;

                if (item.IsPublicOrInternal && !item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                    yield return item;
            }
        }

        private void ImplementProperties()
        {
            foreach (var property in BuilderTypes.IFactoryTypeInfo.BuilderType.Properties)
            {
                var propertyResult = this.componentInfoType.CreateProperty(Modifiers.Public | Modifiers.Overrides, property.ReturnType, property.Name,
                    property.Setter == null ? PropertySetterCreationOption.DontCreateSetter : PropertySetterCreationOption.AlwaysCreate);

                switch (property.Name)
                {
                    case "ContractName":
                        this.OnContractNameSet(propertyResult);
                        break;

                    case "ContractType":
                        this.OnContractTypeSet(propertyResult);
                        break;

                    case "CreationPolicy":
                        this.OnCreationPolicySet(propertyResult);
                        break;

                    case "Priority":
                        this.OnPrioritySet(propertyResult);
                        break;

                    case "Type":
                        this.OnTypeSet(propertyResult);
                        break;

                    case "IsEnumerable":
                        this.OnIsEnumerableSet(propertyResult);
                        break;

                    case "ChildType":
                        this.OnChildTypeSet(propertyResult);
                        break;

                    case "Instance":
                        this.OnInstanceSet(propertyResult);
                        break;
                }
            }
        }
    }
}
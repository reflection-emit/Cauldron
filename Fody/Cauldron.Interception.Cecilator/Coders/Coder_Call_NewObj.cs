using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        public Coder Call(Method method, params object[] parameters) => CallInternal(null, method, OpCodes.Call, parameters);

        public Coder NewObj(Method method, params object[] parameters) => CallInternal(null, method, OpCodes.Newobj, parameters);

        internal Coder CallInternal(object instance, Method method, OpCode opcode, params object[] parameters)
        {
            if (instance != null)
                this.instructions.Append(this.AddParameter(this.processor, null, instance).Instructions);

            if (parameters != null && parameters.Length > 0 && parameters[0] is ArrayCodeBlock arrayCodeSet)
            {
                var methodParameters = method.methodDefinition.Parameters;
                for (int i = 0; i < methodParameters.Count; i++)
                {
                    this.instructions.Append(this.AddParameter(this.processor, null, arrayCodeSet).Instructions);
                    this.instructions.Append(this.AddParameter(this.processor, null, i).Instructions);
                    this.instructions.Append(this.processor.Create(OpCodes.Ldelem_Ref));

                    var paramResult = new ParamResult { Type = Builder.Current.TypeSystem.Object };
                    this.processor.CastOrBoxValues(methodParameters[i].ParameterType, paramResult, methodParameters[i].ParameterType.Resolve());
                    this.instructions.Append(paramResult.Instructions);
                }
            }
            else if (parameters != null && parameters.Length > 0 && parameters[0] is ParametersCodeBlock parameterCodeSet && parameterCodeSet.IsAllParameters)
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli && opcode != OpCodes.Newobj)
                    opcode = OpCodes.Callvirt;

                for (int i = 0; i < method.methodReference.Parameters.Count; i++)
                {
                    var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                        method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                        method.methodDefinition.Parameters[i].ParameterType;

                    var inst = this.AddParameter(this.processor, Builder.Current.Import(parameterType), CodeBlocks.GetParameter(i));
                    this.instructions.Append(inst.Instructions);
                }
            }
            else
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli && opcode != OpCodes.Newobj)
                    opcode = OpCodes.Callvirt;

                if (parameters != null)
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                            method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                            method.methodDefinition.Parameters[i].ParameterType;

                        var inst = this.AddParameter(this.processor, Builder.Current.Import(parameterType), parameters[i]);
                        this.instructions.Append(inst.Instructions);
                    }
            }

            try
            {
                this.instructions.Append(this.processor.Create(opcode, Builder.Current.Import(method.methodReference)));
            }
            catch (NullReferenceException)
            {
                this.instructions.Append(this.processor.Create(opcode, Builder.Current.Import(method.methodReference, this.method.methodReference)));
            }

            return this;
        }
    }
}
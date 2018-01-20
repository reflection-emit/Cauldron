using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class CallNewObjCoderExtensions
    {
        public static Coder Call(this Coder coder, Method method, params object[] parameters) => CallInternal(coder, null, method, OpCodes.Call, parameters);

        public static Coder Call(this Coder coder, Crumb instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Call, parameters);

        public static Coder Call(this Coder coder, Field instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Call, parameters);

        public static Coder Call(this Coder coder, LocalVariable instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Call, parameters);

        public static Coder Call(this Coder coder, Coder instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Call, parameters);

        public static Coder Callvirt(this Coder coder, Method method, params object[] parameters) => CallInternal(coder, null, method, OpCodes.Callvirt, parameters);

        public static Coder Callvirt(this Coder coder, Crumb instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Callvirt, parameters);

        public static Coder Callvirt(this Coder coder, Field instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Callvirt, parameters);

        public static Coder Callvirt(this Coder coder, LocalVariable instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Callvirt, parameters);

        public static Coder Callvirt(this Coder coder, Coder instance, Method method, params object[] parameters) => CallInternal(coder, instance, method, OpCodes.Callvirt, parameters);

        private static Coder CallInternal(Coder coder, object instance, Method method, OpCode opcode, params object[] parameters)
        {
            if (instance != null)
                coder.instructions.Append(coder.AddParameter(coder.processor, null, instance).Instructions);

            if (parameters != null && parameters.Length > 0 && parameters[0] is Crumb && (parameters[0] as Crumb).UnPackArray)
            {
                var methodParameters = method.methodDefinition.Parameters;
                for (int i = 0; i < methodParameters.Count; i++)
                {
                    coder.instructions.Append(coder.AddParameter(coder.processor, null, parameters[0]).Instructions);
                    coder.instructions.Append(coder.AddParameter(coder.processor, null, i).Instructions);
                    coder.instructions.Append(coder.processor.Create(OpCodes.Ldelem_Ref));

                    var paramResult = new ParamResult { Type = Builder.Current.TypeSystem.Object };
                    coder.processor.CastOrBoxValues(methodParameters[i].ParameterType, paramResult, methodParameters[i].ParameterType.Resolve());
                    coder.instructions.Append(paramResult.Instructions);
                }
            }
            else if (parameters != null && parameters.Length > 0 && parameters[0] is Crumb && (parameters[0] as Crumb).Index < 0)
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli)
                    opcode = OpCodes.Callvirt;

                for (int i = 0; i < method.methodReference.Parameters.Count; i++)
                {
                    var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                        method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                        method.methodDefinition.Parameters[i].ParameterType;

                    var inst = coder.AddParameter(coder.processor, Builder.Current.Import(parameterType), Crumb.GetParameter(i));
                    coder.instructions.Append(inst.Instructions);
                }
            }
            else
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli)
                    opcode = OpCodes.Callvirt;

                if (parameters != null)
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                            method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                            method.methodDefinition.Parameters[i].ParameterType;

                        var inst = coder.AddParameter(coder.processor, Builder.Current.Import(parameterType), parameters[i]);
                        coder.instructions.Append(inst.Instructions);
                    }
            }

            try
            {
                coder.instructions.Append(coder.processor.Create(opcode, Builder.Current.Import(method.methodReference)));
            }
            catch (NullReferenceException)
            {
                coder.instructions.Append(coder.processor.Create(opcode, Builder.Current.Import(method.methodReference, coder.method.methodReference)));
            }

            if (!method.IsVoid && coder is IAssignCoder assignCoder)
                assignCoder.Store();

            return coder;
        }
    }
}
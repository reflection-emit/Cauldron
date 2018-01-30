using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    internal static class CastingExtensions
    {
        internal static void CastOrBoxValues(this ILProcessor processor, TypeReference targetType, ParamResult result, TypeDefinition targetDef)
        {
            // TODO - Support for nullable types required

            bool IsInstRequired()
            {
                if (targetDef.FullName == typeof(string).FullName || result.Type.FullName == typeof(object).FullName || targetDef.IsInterface)
                    return true;

                if (targetType.IsValueType)
                    return false;

                if (targetType.IsArray)
                    return false;

                if (targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1"))
                    return false;

                return false;
            }

            // TODO - adds additional checks for not resolved generics
            if (targetDef == null && result.Type.Resolve() != null) /* This happens if the target type is a generic */ result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            else if (IsInstRequired()) result.Instructions.Add(processor.Create(OpCodes.Isinst, Builder.Current.Import(targetType)));
            else if (targetDef.IsEnum)
            {
                if (result.Type.FullName == typeof(string).FullName)
                {
                    result.Instructions.InsertRange(0, processor.TypeOf(targetType));

                    result.Instructions.AddRange(processor.TypeOf(Builder.Current.Import(targetType)));
                    result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
                }
                else
                    result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));

                // Bug #23
                //result.Instructions.InsertRange(0, this.TypeOf(processor, targetType));

                //result.Instructions.AddRange(this.TypeOf(processor, Builder.Current.Import(targetType)));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
            }
            else if (result.Type.FullName == typeof(object).FullName && (targetType.IsArray || targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1")))
            {
                var childType = Builder.Current.GetChildrenType(targetType);
                var castMethod = Builder.Current.Import(Builder.Current.Import(typeof(System.Linq.Enumerable)).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(null, childType));
                var toArrayMethod = Builder.Current.Import(Builder.Current.Import(typeof(System.Linq.Enumerable)).GetMethodReference("ToArray", 1).MakeGeneric(null, childType));

                result.Instructions.Add(processor.Create(OpCodes.Isinst, Builder.Current.Import(typeof(IEnumerable))));
                result.Instructions.Add(processor.Create(OpCodes.Call, castMethod));

                if (targetType.IsArray)
                    result.Instructions.Add(processor.Create(OpCodes.Call, toArrayMethod));
            }
            else if ((result.Type.FullName == typeof(object).FullName && targetDef.IsValueType) || (result.Type != targetType && result.Type.IsPrimitive && targetType.IsPrimitive))
            {
                if (targetDef.FullName == typeof(int).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToInt32", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(uint).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToUInt32", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(bool).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToBoolean", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(byte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToByte", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(char).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToChar", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(DateTime).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToDateTime", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(decimal).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToDecimal", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(double).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToDouble", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(short).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToInt16", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(long).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToInt64", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(sbyte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToSByte", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(float).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToSingle", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(ushort).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToUInt16", new TypeReference[] { result.Type }))));
                else if (targetDef.FullName == typeof(ulong).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToUInt64", new TypeReference[] { result.Type }))));
                else result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            }
            else if ((result.Type.Resolve() == null || result.Type.IsValueType) && !targetType.IsValueType)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName == "System.Object")
            {
                // Nope nothing....
            }
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName != result.Type.FullName && targetType.AreReferenceAssignable(Builder.Current.Import(result.Type)))
                result.Instructions.Add(processor.Create(OpCodes.Castclass, Builder.Current.Import(result.Type)));
        }
    }
}
using System;
using System.Reflection;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TestMethodInterceptorWithParameter : Attribute, IMethodInterceptor
    {
        public TestMethodInterceptorWithParameter(
            int paramInt,
            uint paramUInt,
            bool paramBool,
            byte paramByte,
            sbyte paramSByte,
            char paramChar,
            short paramShort,
            ushort paramUShort,
            long paramLong,
            ulong paramULong,
            double paramDouble,
            float paramFloat,
            string paramString,
            object paramObject,
            object paramObject2,
            Type paramType,
            TestEnum paramEnum,
            int[] paramIntArray)
        {
        }

        public TestMethodInterceptorWithParameter(
            int paramInt,
            uint paramUInt,
            bool paramBool,
            byte paramByte,
            sbyte paramSByte,
            char paramChar,
            short paramShort,
            ushort paramUShort,
            long paramLong,
            ulong paramULong,
            double paramDouble,
            float paramFloat,
            string paramString,
            object paramObject,
            object paramObject2,
            Type paramType,
            TestEnum paramEnum,
            int[] paramIntArray,
            uint[] paramUIntArray,
            bool[] paramBoolArray,
            byte[] paramByteArray,
            sbyte[] paramSByteArray,
            char[] paramCharArray,
            short[] paramShortArray,
            ushort[] paramUShortArray,
            long[] paramLongArray,
            ulong[] paramULongArray,
            double[] paramDoubleArray,
            float[] paramFloatArray,
            string[] paramStringArray,
            object[] paramObjectArray,
            object[] paramObjectArray2,
            Type[] paramTypeArray,
            TestEnum[] paramEnumArray)
        {
        }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }
}
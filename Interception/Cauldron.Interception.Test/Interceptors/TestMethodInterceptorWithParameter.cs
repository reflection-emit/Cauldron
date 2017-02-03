using Cauldron.Core.Interceptors;
using System;
using System.Reflection;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TestMethodInterceptorWithParameter : Attribute, IMethodInterceptor
    {
        public TestMethodInterceptorWithParameter(
            Type paramType, int ff)
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

        public bool BoolParam { get; private set; }

        public byte ByteParam { get; private set; }

        public char CharParam { get; private set; }

        public double DoubleParam { get; private set; }

        public TestEnum[] EnumArrayParam { get; private set; }

        public TestEnum EnumParam { get; private set; }

        public float[] FloatArrayParam { get; private set; }

        public int[] IntArrayParam { get; private set; }

        public object ObjectParam { get; private set; }

        public string[] StringArrayParam { get; private set; }

        public string StringParam { get; private set; }

        public Type TypeParam { get; private set; }

        public void OnEnter(object instance, MethodBase methodbase, object[] values)
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
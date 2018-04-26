using System;

namespace Win32_Fody_Assembly_Validation_Tests
{
    public class DisposableTestClass : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class TestClass : ITestInterface
    {
        public byte ByteProperty { get; set; }

        public char CharProperty { get; set; }

        public double DoubleProperty { get; set; }

        public TestEnum EnumProperty { get; set; }

        public float FloatProperty { get; set; }

        public int IntegerProperty { get; set; }

        public ITestInterface[] Items { get; set; }

        public long LongProperty { get; set; }

        public short ShortProperty { get; set; }

        public string StringProperty { get; set; }

        public TestStruct StructProperty { get; set; }
    }
}
namespace Cauldron.Interception.Test
{
    public interface ITestInterface
    {
        byte ByteProperty { get; set; }

        char CharProperty { get; set; }

        double DoubleProperty { get; set; }

        TestEnum EnumProperty { get; set; }

        float FloatProperty { get; set; }

        int IntegerProperty { get; set; }

        long LongProperty { get; set; }

        short ShortProperty { get; set; }

        string StringProperty { get; set; }

        TestStruct StructProperty { get; set; }
    }
}
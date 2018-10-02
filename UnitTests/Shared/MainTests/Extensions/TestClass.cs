namespace MainTests.Extensions
{
    public class TestClass
    {
        public static implicit operator int(TestClass testStruct) => 22;
    }
}
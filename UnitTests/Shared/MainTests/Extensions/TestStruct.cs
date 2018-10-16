namespace MainTests.Extensions
{
    public struct TestStruct
    {
        public static implicit operator int(TestStruct testStruct) => 22;
    }
}
namespace UnitTests.Cecilator
{
    public class TestClass
    {
        private int value;

        private TestClass()
        {
        }

        public static implicit operator TestClass(int value) => new TestClass { value = value };

        public static int operator &(TestClass a, TestClass b) => a.value & b.value;

        public static int operator |(TestClass a, TestClass b) => a.value | b.value;

        public static int operator |(TestClass a, int b) => a.value | b;
    }
}
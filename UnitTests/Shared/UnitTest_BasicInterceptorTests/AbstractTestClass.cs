using UnitTest_InterceptorsForTest;

namespace UnitTest_BasicInterceptorTests
{
    public abstract class AbstractTestClass
    {
        [LogEnterExit]
        public abstract string TestProperty { get; set; }

        [LogEnterExit]
        public abstract void TestMethod();
    }

    public class TestMethod2 : AbstractTestClass
    {
        public override string TestProperty { get; set; }

        public override void TestMethod()
        {
        }
    }

    public class TestMethod3 : TestMethod2
    {
        public override void TestMethod()
        {
        }
    }
}
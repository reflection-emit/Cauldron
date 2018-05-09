using Cauldron.Activator;

namespace UnitTest_ActivatorTests
{
    [Component(typeof(TestComponent))]
    public class TestComponent
    {
        [ComponentConstructor]
        public static TestComponent Create() => new TestComponent();
    }
}
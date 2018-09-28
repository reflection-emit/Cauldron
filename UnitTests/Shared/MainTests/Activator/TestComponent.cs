using Cauldron.Activator;

namespace UnitTests.Activator
{
    [Component(typeof(TestComponent))]
    public class TestComponent
    {
        [ComponentConstructor]
        public static TestComponent Create() => new TestComponent();
    }
}
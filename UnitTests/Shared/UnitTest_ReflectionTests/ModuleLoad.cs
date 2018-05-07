using System.Diagnostics;
using System.Reflection;

namespace UnitTest_ReflectionTests
{
    public class Module
    {
        public static void ModuleLoad(Assembly[] referenceCopyLocal)
        {
            foreach (var item in referenceCopyLocal)
                Debug.WriteLine($">>> {item.FullName}");
        }
    }
}
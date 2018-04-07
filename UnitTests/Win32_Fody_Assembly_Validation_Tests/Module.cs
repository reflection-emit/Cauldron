using System;

namespace Cauldron.UnitTest.AssemblyValidation
{
    public class Module
    {
        private static Action<object> blub;

        public static void ModuleLoad(string[] array)
        {
            blub = new Action<object>(bla);
        }

        private static void bla(object b)
        {
        }
    }
}
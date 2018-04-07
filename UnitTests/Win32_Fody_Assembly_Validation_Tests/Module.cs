using System;

namespace Cauldron.UnitTest.AssemblyValidation
{
    public class Module
    {
        private int? hi;

        private int Bla { get; set; }

        public static void ModuleLoad(string[] array)
        {
        }

        private object Buhuh(object io)
        {
            return Bla;
        }
    }
}
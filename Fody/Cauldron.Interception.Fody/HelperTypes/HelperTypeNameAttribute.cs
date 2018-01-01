using System;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HelperTypeNameAttribute : Attribute
    {
        public HelperTypeNameAttribute(string fullname) => this.Fullname = fullname;

        public HelperTypeNameAttribute(string fullname, string uwpFullname)
        {
            this.Fullname = fullname;
            this.UWPFullname = uwpFullname;
        }

        public HelperTypeNameAttribute(string fullname, string uwpFullname, string importUWPAssembly)
        {
            this.ImportUWPAssembly = importUWPAssembly;
            this.Fullname = fullname;
            this.UWPFullname = uwpFullname;
        }

        public string Fullname { get; private set; }
        public string ImportUWPAssembly { get; private set; }
        public string UWPFullname { get; private set; }
    }
}
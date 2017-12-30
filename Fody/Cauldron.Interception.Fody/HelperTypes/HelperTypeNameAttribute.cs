using System;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HelperTypeNameAttribute : Attribute
    {
        public HelperTypeNameAttribute(string fullname) => this.Fullname1 = fullname;

        public HelperTypeNameAttribute(string fullname1, string fullname2)
        {
            this.Fullname1 = fullname1;
            this.Fullname2 = fullname2;
        }

        public string Fullname1 { get; private set; }
        public string Fullname2 { get; private set; }
    }
}
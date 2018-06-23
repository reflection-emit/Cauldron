using System.Collections.Generic;

namespace Cauldron.Activator
{
    internal sealed class FactoryStringDictionary<TValue> : FastDictionary<string, TValue>
        where TValue : class
    {
        protected override bool AreEqual(string a, string b) => a == b;
    }
}
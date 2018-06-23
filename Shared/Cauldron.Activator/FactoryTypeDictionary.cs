using System;
using System.Collections.Generic;

namespace Cauldron.Activator
{
    internal sealed class FactoryTypeDictionary<TValue> : FastDictionary<Type, TValue>
        where TValue : class
    {
        protected override bool AreEqual(Type a, Type b) => object.ReferenceEquals(a, b);
    }
}
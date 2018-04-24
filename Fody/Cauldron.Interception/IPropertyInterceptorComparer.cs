using System;

namespace Cauldron.Interception
{
    public interface IPropertyInterceptorComparer
    {
        Func<object, object, bool> AreEqual { get; set; }
    }
}
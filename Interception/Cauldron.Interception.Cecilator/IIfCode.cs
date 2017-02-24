using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface IIfCode
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        IIfCode Then(Action<ICode> action);

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
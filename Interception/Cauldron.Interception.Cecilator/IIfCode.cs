using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface IIfCode : ICode
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        new bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        IIfCode Then(Action<ICode> action);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();
    }
}
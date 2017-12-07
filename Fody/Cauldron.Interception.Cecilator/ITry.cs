using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ITry
    {
        ICatch Catch(Type exceptionType, Action<ICatchCode> body);

        ICatch Catch(BuilderType exceptionType, Action<ICatchCode> body);

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        IFinally Finally(Action<ICatchCode> body);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
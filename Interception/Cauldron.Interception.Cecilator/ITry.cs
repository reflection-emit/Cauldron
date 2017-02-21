using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ITry
    {
        ICatch Catch(Type exceptionType, Func<ICatchCode, ICode> body);

        ICatch Catch(BuilderType exceptionType, Func<ICatchCode, ICode> body);

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        IFinally Finally(Func<ICatchCode, ICode> body);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
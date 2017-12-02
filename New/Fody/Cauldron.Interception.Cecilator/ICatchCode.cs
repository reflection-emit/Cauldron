using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ICatchCode : ICode
    {
        Crumb Exception { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        new bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        ICode Rethrow();

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();
    }
}
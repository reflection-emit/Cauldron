using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ITryCode : ICode
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        new bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        ICode OriginalBody();

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();
    }
}
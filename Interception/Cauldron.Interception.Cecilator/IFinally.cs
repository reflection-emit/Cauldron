using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface IFinally
    {
        ICode EndTry();

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ICatch : ITry
    {
        ICode EndTry();

        [EditorBrowsable(EditorBrowsableState.Never)]
        new bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();
    }
}
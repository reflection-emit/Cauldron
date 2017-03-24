using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface IAction
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        void Insert(InsertionPosition position);

        void Insert(InsertionAction action, Position position);

        void Replace();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
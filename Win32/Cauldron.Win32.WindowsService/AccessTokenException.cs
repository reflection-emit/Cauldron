using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.WindowsService
{
    /// <summary>
    /// Represents an exception that occures while interacting with process tokens.
    /// </summary>
    public sealed class AccessTokenException : Exception
    {
        internal AccessTokenException(string message) : base(message)
        {
        }
    }
}
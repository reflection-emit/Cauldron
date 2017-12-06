using System.IO;
using System.Threading.Tasks;

namespace Cauldron.Potions
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public partial interface IUserInformation
    {
        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        bool IsLocalAccount { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        Task<string> GetEmailAddressAsync();

        /// <summary>
        /// Gets the user's home directory.
        /// </summary>
        /// <returns>The user's home directory</returns>
        Task<DirectoryInfo> GetHomeDirectory();

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        Task<string> GetTelephoneNumberAsync();

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        Task<bool> IsLockedOutAsync();
    }
}
using System.Threading.Tasks;

namespace Cauldron.Potions
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public partial interface IUserInformation
    {
        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        Task<string> GetDisplayNameAsync();

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        Task<string> GetDomainNameAsync();

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name.</returns>
        Task<string> GetFirstNameAsync();

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name.</returns>
        Task<string> GetLastNameAsync();

        /// <summary>
        /// Gets the principal name for the user. This name is the User Principal Name (typically the user's address, although this is not always true.)
        /// </summary>
        /// <returns> The user's principal name.</returns>
        Task<string> GetPrincipalNameAsync();

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        Task<string> GetUserNameAsync();
    }
}
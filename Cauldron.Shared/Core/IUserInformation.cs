using System.Threading.Tasks;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else

using System.Windows.Media.Imaging;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public interface IUserInformation
    {
        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        Task<BitmapImage> GetAccountPictureAsync();

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name of the user account</returns>
        Task<string> GetDisplayNameAsync();

        /// <summary>
        /// Gets the domain of the user
        /// </summary>
        Task<string> GetDomainNameAsync();

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name</returns>
        Task<string> GetFirstNameAsync();

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name</returns>
        Task<string> GetLastNameAsync();

        /// <summary>
        /// Gets the username the application is running on
        /// </summary>
        Task<string> GetUserNameAsync();

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        Task<bool> IsLocalAccountAsync();
    }
}
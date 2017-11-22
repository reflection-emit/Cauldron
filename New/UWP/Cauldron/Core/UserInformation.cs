using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// <para/>
    /// This is a wrapper for <see cref="User.FindAllAsync(UserType)"/>.
    /// </summary>
    public static class UserInformation
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private static User user;

        /// <summary>
        /// Gets the account picture for the user.
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        /// <returns>The image of the user</returns>
        public static byte[] AccountPicture => GetAccountPictureAsync().RunSync();

        /// <summary>
        /// Gets the current user
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        public static User CurrentUser
        {
            get
            {
                GetUserInformationAsync().RunSync();
                return user;
            }
        }

        /// <summary>
        /// Gets the display name for the user account.
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        public static string DisplayName => GetDisplayNameAsync().RunSync();

        /// <summary>
        /// Gets the domain name for the user.
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public static string DomainName => GetDomainNameAsync().RunSync();

        /// <summary>
        /// Gets the user's first name.
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        /// <returns>The user's first name.</returns>
        public static string FirstName => GetFirstNameAsync().RunSync();

        /// <summary>
        /// Gets the user's last name.
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        /// <returns>The user's last name.</returns>
        public static string LastName => GetLastNameAsync().RunSync();

        /// <summary>
        /// Gets the user name of the user.
        /// <para/>
        /// Attention: Cauld cause thread-locks.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public static string UserName => GetUserNameAsync().RunSync();

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        public static async Task<string> GetDisplayNameAsync()
        {
            await GetUserInformationAsync();
            return (await user.GetPropertyAsync(KnownUserProperties.DisplayName))?.ToString();
        }

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public static async Task<string> GetDomainNameAsync()
        {
            await GetUserInformationAsync();
            return (await user.GetPropertyAsync(KnownUserProperties.DomainName))?.ToString();
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name.</returns>
        public static async Task<string> GetFirstNameAsync()
        {
            await GetUserInformationAsync();
            return (await user.GetPropertyAsync(KnownUserProperties.FirstName))?.ToString();
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name.</returns>
        public static async Task<string> GetLastNameAsync()
        {
            await GetUserInformationAsync();
            return (await user.GetPropertyAsync(KnownUserProperties.LastName))?.ToString();
        }

        /// <summary>
        /// Gets the principal name for the user. This name is the User Principal Name (typically the user's address, although this is not always true.)
        /// </summary>
        /// <returns> The user's principal name.</returns>
        public static async Task<string> GetPrincipalNameAsync()
        {
            await GetUserInformationAsync();
            return (await user.GetPropertyAsync(KnownUserProperties.PrincipalName))?.ToString();
        }

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public static async Task<string> GetUserNameAsync()
        {
            await GetUserInformationAsync();
            return (await user.GetPropertyAsync(KnownUserProperties.AccountName))?.ToString();
        }

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        private static async Task<byte[]> GetAccountPictureAsync()
        {
            var avatar = await user.GetPictureAsync(UserPictureSize.Size424x424);

            using (var stream = await avatar.OpenReadAsync())
                return await stream.ToBytesAsync();
        }

        private static async Task GetUserInformationAsync()
        {
            if (user != null)
                return;

            await semaphoreSlim.WaitAsync();

            // we have to avoid thing that already entered and waited for
            // the semaphore to be released from other thread

            if (user != null)
            {
                semaphoreSlim.Release();
                return;
            }

            try
            {
                user = (await User.FindAllAsync(UserType.LocalUser)).FirstOrDefault(x => x.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
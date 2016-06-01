using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    [Factory(typeof(IUserInformation))]
    public sealed class UserInformation : IUserInformation
    {
        private static object lockCurrentObject = new object();
        private static volatile IUserInformation userInformation;

        private volatile bool loaded = false;

        private User user;

        private UserInformation()
        {
        }

        /// <summary>
        /// Gets the <see cref="IUserInformation"/> of the current user that runs the application
        /// </summary>
        public static IUserInformation Current
        {
            get
            {
                if (userInformation == null)
                {
                    lock (lockCurrentObject)
                    {
                        if (userInformation == null)
                        {
                            userInformation = Factory.Create<IUserInformation>();
                        }
                    }
                }
                return userInformation;
            }
        }

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        public async Task<BitmapImage> GetAccountPictureAsync()
        {
            await this.GetInformation();
            var result = new BitmapImage();
            result.CreateOptions = BitmapCreateOptions.None;
            await result.SetSourceAsync(await (await user.GetPictureAsync(UserPictureSize.Size208x208)).OpenReadAsync());
            return result;
        }

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name of the user account</returns>
        public async Task<string> GetDisplayNameAsync()
        {
            await this.GetInformation();
            return (await user.GetPropertyAsync(KnownUserProperties.DisplayName)).ToString();
        }

        /// <summary>
        /// Gets the domain of the user
        /// </summary>
        public async Task<string> GetDomainNameAsync()
        {
            await this.GetInformation();
            return (await user.GetPropertyAsync(KnownUserProperties.DomainName)).ToString();
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name</returns>
        public async Task<string> GetFirstNameAsync()
        {
            await this.GetInformation();
            return (await user.GetPropertyAsync(KnownUserProperties.FirstName)).ToString();
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name</returns>
        public async Task<string> GetLastNameAsync()
        {
            await this.GetInformation();
            return (await user.GetPropertyAsync(KnownUserProperties.LastName)).ToString();
        }

        /// <summary>
        /// Gets the username the application is running on
        /// </summary>
        public async Task<string> GetUserNameAsync()
        {
            await this.GetInformation();
            return (await user.GetPropertyAsync(KnownUserProperties.AccountName)).ToString();
        }

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        public async Task<bool> IsLocalAccountAsync()
        {
            await this.GetInformation();
            return user.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated;
        }

        private async Task GetInformation()
        {
            if (this.loaded)
                return;

            this.loaded = true;

            this.user = (await User.FindAllAsync()).FirstOrDefault();
        }
    }
}
using Cauldron.Activator;
using System;
using System.Threading.Tasks;
using Cauldron.Core;

#if WINDOWS_UWP

using Windows.System;
using System.Threading;
using Cauldron.Core.Extensions;
using System.Linq;

#else

using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Principal;
using System.Text;

#endif

namespace Cauldron.Potions
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    [Component(typeof(IUserInformation), FactoryCreationPolicy.Instanced)]
    public sealed partial class UserInformation : FactoryObject<IUserInformation>, IUserInformation
    {
        private string _domainName;
        private string _username;
        private bool isInitialized = false;
#if WINDOWS_UWP
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private User user;
#else
        private object lockObject = new object();
#endif

#if WINDOWS_UWP

        /// <summary>
        /// Initializes a new instance of <see cref="UserInformation"/>. The default is the current user.
        /// </summary>
        [ComponentConstructor]
        private UserInformation()
        {
        }

        /// <summary>
        /// Wraps a <see cref="User"/> to its wrapper object <see cref="UserInformation"/>
        /// </summary>
        /// <param name="user"></param>
        public static implicit operator UserInformation(User user)
        {
            var userInformation = new UserInformation();
            userInformation.user = user;
            userInformation.isInitialized = true;

            return userInformation;
        }

#endif

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        public async Task<string> GetDisplayNameAsync()
        {
            await this.GetUserInformationAsync();
#if WINDOWS_UWP
            return (await this.user.GetPropertyAsync(KnownUserProperties.DisplayName))?.ToString();
#else
            return this._displayName;
#endif
        }

#if WINDOWS_UWP

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public async Task<string> GetDomainNameAsync()
        {
            await this.GetUserInformationAsync();
            return this._domainName;
        }

#endif

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name.</returns>
        public async Task<string> GetFirstNameAsync()
        {
            await this.GetUserInformationAsync();
#if WINDOWS_UWP
            return (await this.user.GetPropertyAsync(KnownUserProperties.FirstName))?.ToString();
#else
            return this._firstName;
#endif
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name.</returns>
        public async Task<string> GetLastNameAsync()
        {
            await this.GetUserInformationAsync();
#if WINDOWS_UWP
            return (await this.user.GetPropertyAsync(KnownUserProperties.LastName))?.ToString();
#else
            return this._lastName;
#endif
        }

        /// <summary>
        /// Gets the principal name for the user. This name is the User Principal Name (typically the user's address, although this is not always true.)
        /// </summary>
        /// <returns> The user's principal name.</returns>
        public async Task<string> GetPrincipalNameAsync()
        {
            await this.GetUserInformationAsync();
#if WINDOWS_UWP
            return (await this.user.GetPropertyAsync(KnownUserProperties.PrincipalName))?.ToString();
#else
            return this._principalName;
#endif
        }

#if WINDOWS_UWP

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public async Task<string> GetUserNameAsync()
        {
            await this.GetUserInformationAsync();
            return this._username;
        }

#endif

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{this._domainName}\\{this._username}".ToLower();

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        private async Task<byte[]> GetAccountPictureAsync()
        {
#if WINDOWS_UWP
            var avatar = await this.user.GetPictureAsync(UserPictureSize.Size424x424);

            using (var stream = await avatar.OpenReadAsync())
            {
                return await stream.ToBytesAsync();
            }
#else
            if (!this.IsLocalAccount)
            {
                var data = await this.GetUserPictureFromActiveDirectoryAsync();

                if (data != null)
                    return data;
            }

            string path = null;

            await Task.Run(() =>
            {
                if (path == null)
                    path = GetAccountPicturePathFromDatFile();

                if (path == null)
                    path = Win32Api.GetUserTilePath(this._username);
            });

            if (File.Exists(path))
                return File.ReadAllBytes(path);

            return null;

#endif
        }

        private async Task GetUserInformationAsync()
        {
            if (this.isInitialized)
                return;

#if WINDOWS_UWP
            await semaphoreSlim.WaitAsync();

            // we have to avoid thing that already entered and waited for
            // the semaphore to be released from other thread
            if (this.isInitialized)
            {
                semaphoreSlim.Release();
                return;
            }

            try
            {
                this.user = (await User.FindAllAsync(UserType.LocalUser)).FirstOrDefault(x => x.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated);
                this._username = (await this.user.GetPropertyAsync(KnownUserProperties.AccountName))?.ToString();
                this._domainName = (await this.user.GetPropertyAsync(KnownUserProperties.DomainName))?.ToString();
                this.isInitialized = true;
            }
            finally
            {
                semaphoreSlim.Release();
            }
#else
            await Task.Run(() =>
            {
                lock (this.lockObject)
                {
                    // we have to avoid thing that already entered and waited for
                    // the lock to be released from other thread
                    if (!this.isInitialized)
                    {
                        try
                        {
                            if (this.IsLocalAccount)
                                // The user is not a domain user
                                this.GetInformation(ContextType.Machine);
                            else if (Network.CreateInstance().IsNetworkAvailable)
                                // Try to gather information only if we have any network connection
                                this.GetInformation(ContextType.Domain);
                            else
                            {
                                // Else we just try our best
                                this._emailAddress = (this._username + "@" + this._domainName).ToLower();
                                this._displayName = this._username.ToLower();
                                this._firstName = string.Empty;
                                this._lastName = this._username.ToLower();
                                this._telephoneNumber = string.Empty;
                                this._isLockedOut = false;
                            }
                        }
                        catch
                        {
                            this._emailAddress = (this._username + "@" + this._domainName).ToLower();
                            this._displayName = this._username.ToLower();
                            this._lastName = this._username.ToLower();
                        }

                        this.isInitialized = true;
                    }
                }
            });
#endif
        }
    }
}
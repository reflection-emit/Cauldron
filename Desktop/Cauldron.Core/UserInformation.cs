using Cauldron.Core.Extensions;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents information about the current user, such as name and account picture.
    /// </summary>
    public static class UserInformation
    {
        private static User currentUser;

        static UserInformation()
        {
            currentUser = new User(WindowsIdentity.GetCurrent().Name);
        }

        /// <summary>
        /// Gets the current user
        /// </summary>
        public static User CurrentUser { get { return currentUser; } }

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        public static bool IsLocalAccount { get { return currentUser.IsLocalAccount; } }

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        public static async Task<string> GetDisplayNameAsync() => await currentUser.GetDisplayNameAsync();

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public static async Task<string> GetDomainNameAsync() => await currentUser.GetDomainNameAsync();

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        public static async Task<string> GetEmailAddressAsync() => await currentUser.GetEmailAddressAsync();

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name.</returns>
        public static async Task<string> GetFirstNameAsync() => await currentUser.GetFirstNameAsync();

        /// <summary>
        /// Gets the user's home directory.
        /// </summary>
        /// <returns>The user's home directory</returns>
        public static async Task<DirectoryInfo> GetHomeDirectory() => await currentUser.GetHomeDirectory();

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name.</returns>
        public static async Task<string> GetLastNameAsync() => await currentUser.GetLastNameAsync();

        /// <summary>
        /// Gets the principal name for the user. This name is the User Principal Name (typically the user's address, although this is not always true.)
        /// </summary>
        /// <returns> The user's principal name.</returns>
        public static async Task<string> GetPrincipalNameAsync() => await currentUser.GetPrincipalNameAsync();

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        public static async Task<string> GetTelephoneNumberAsync() => await currentUser.GetTelephoneNumberAsync();

        /// <summary>
        /// Gets the user information of the given user by its user name.
        /// </summary>
        /// <param name="userName">The domain and id of the user</param>
        /// <exception cref="ArgumentNullException"><paramref name="userName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="userName"/> is empty</exception>
        public static User GetUserInformation(string userName) => new User(userName);

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public static async Task<string> GetUserNameAsync() => await currentUser.GetUserNameAsync();

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        public static async Task<bool> IsLockedOutAsync() => await currentUser.IsLockedOutAsync();
    }

    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public sealed class User
    {
        private string _displayName;
        private string _domainName;
        private string _emailAddress;
        private string _firstName;
        private string _homeDirectory;
        private bool _isLockedOut;
        private string _lastName;
        private string _principalName;
        private string _telephoneNumber;
        private string _username;
        private bool isInitialized = false;
        private object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of <see cref="User"/>
        /// </summary>
        /// <param name="userName">The domain and id of the user</param>
        /// <exception cref="ArgumentNullException"><paramref name="userName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="userName"/> is empty</exception>
        internal User(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (userName == "")
                throw new ArgumentException("Parameter 'userName' cannot be empty", nameof(userName));

            if (userName.Contains("@")) // johnd@companydomain
            {
                var splittedInfo = userName.Split(new char[] { '@' }, 2);
                this._domainName = splittedInfo[1];
                this._username = splittedInfo[0];
            }
            else if (!userName.Contains("\\")) // johnd
            {
                this._domainName = Environment.UserDomainName;
                this._username = userName;
            }
            else // companydomain\johnd
            {
                this._domainName = Path.GetDirectoryName(userName).ToLower();
                this._username = Path.GetFileName(userName).ToLower();
            }
        }

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        public bool IsLocalAccount
        {
            get { return string.Equals(Environment.MachineName, _domainName, StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        public async Task<string> GetDisplayNameAsync()
        {
            await GetUserInformationAsync();
            return _displayName;
        }

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public Task<string> GetDomainNameAsync() => Task.FromResult(_domainName);

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        public async Task<string> GetEmailAddressAsync()
        {
            await GetUserInformationAsync();
            return _emailAddress;
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name.</returns>
        public async Task<string> GetFirstNameAsync()
        {
            await GetUserInformationAsync();
            return _firstName;
        }

        /// <summary>
        /// Gets the user's home directory.
        /// </summary>
        /// <returns>The user's home directory</returns>
        public async Task<DirectoryInfo> GetHomeDirectory()
        {
            await GetUserInformationAsync();

            if (Directory.Exists(_homeDirectory))
                return new DirectoryInfo(_homeDirectory);

            return null;
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name.</returns>
        public async Task<string> GetLastNameAsync()
        {
            await GetUserInformationAsync();
            return _lastName;
        }

        /// <summary>
        /// Gets the principal name for the user. This name is the User Principal Name (typically the user's address, although this is not always true.)
        /// </summary>
        /// <returns> The user's principal name.</returns>
        public async Task<string> GetPrincipalNameAsync()
        {
            await GetUserInformationAsync();
            return _principalName;
        }

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        public async Task<string> GetTelephoneNumberAsync()
        {
            await GetUserInformationAsync();
            return _telephoneNumber;
        }

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public Task<string> GetUserNameAsync() => Task.FromResult(_username);

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        public async Task<bool> IsLockedOutAsync()
        {
            await GetUserInformationAsync();
            return _isLockedOut;
        }

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        private async Task<byte[]> GetAccountPictureAsync()
        {
            if (!IsLocalAccount)
            {
                var data = await GetUserPictureFromActiveDirectoryAsync();

                if (data != null)
                    return data;
            }

            string path = null;

            await Task.Run(() =>
            {
                if (path == null)
                    path = GetAccountPicturePathFromDatFile();

                if (path == null)
                    path = Win32Api.GetUserTilePath(_username);
            });

            if (File.Exists(path))
                return File.ReadAllBytes(path);

            return null;
        }

        private string GetAccountPicturePathFromDatFile()
        {
            try
            {
                var programData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\User Account Pictures");
                var filename = Path.Combine(programData, string.Format("{0}+{1}.dat", _domainName, _username));

                if (!File.Exists(filename))
                    return null;

                var data = File.ReadAllBytes(filename);
                var position = (int)data.IndexOf(new byte[] { 0x62, 0, 0x6d, 0, 0x70, 0 });

                if (position == -1)
                    return null;

                var lengthOfData = data.GetBytes(position + 12, 4).ToInteger() - 2;
                var path = Encoding.Unicode.GetString(data.GetBytes(position + 16, lengthOfData)).Trim(new char[] { ' ', '\0', '\r', '\n' });

                if (!File.Exists(path))
                    return null;

                return path;
            }
            catch
            {
                return null;
            }
        }

        private void GetInformation(ContextType contextType)
        {
            using (var context = new PrincipalContext(contextType))
            {
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, _username))
                {
                    _emailAddress = user.EmailAddress?.ToLower();
                    _displayName = user.DisplayName;
                    _firstName = user.GivenName;
                    _lastName = user.Surname;
                    _telephoneNumber = user.VoiceTelephoneNumber;
                    _isLockedOut = user.IsAccountLockedOut();
                    _homeDirectory = user.HomeDirectory;
                    _principalName = user.UserPrincipalName;
                }
            }
        }

        private async Task GetUserInformationAsync()
        {
            if (isInitialized)
                return;

            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    // we have to avoid thing that already entered and waited for
                    // the lock to be released from other thread
                    if (!isInitialized)
                    {
                        try
                        {
                            if (IsLocalAccount)
                                // The user is not a domain user
                                GetInformation(ContextType.Machine);
                            else if (Network.IsNetworkAvailable)
                                // Try to gather information only if we have any network connection
                                GetInformation(ContextType.Domain);
                            else
                            {
                                // Else we just try our best
                                _emailAddress = (_username + "@" + _domainName).ToLower();
                                _displayName = _username.ToLower();
                                _firstName = string.Empty;
                                _lastName = _username.ToLower();
                                _telephoneNumber = string.Empty;
                                _isLockedOut = false;
                            }
                        }
                        catch
                        {
                            _emailAddress = (_username + "@" + _domainName).ToLower();
                            _displayName = _username.ToLower();
                            _lastName = _username.ToLower();
                        }

                        isInitialized = true;
                    }
                }
            });
        }

        private async Task<byte[]> GetUserPictureFromActiveDirectoryAsync()
        {
            byte[] data = null;

            await Task.Run(() =>
            {
                try
                {
                    using (var rootDSE = new DirectoryEntry("LDAP://RootDSE"))
                    {
                        var domainDN = rootDSE.Properties["DefaultNamingContext"].Value;
                        using (var adEntry = new DirectoryEntry("LDAP://" + domainDN))
                        {
                            using (DirectorySearcher dsSearcher = new DirectorySearcher(adEntry))
                            {
                                dsSearcher.Filter = "(&(objectCategory=user)(|(CN={0})(sAMAccountName={0})))".ToString(_username);
                                SearchResult result = dsSearcher.FindOne();

                                using (DirectoryEntry user = new DirectoryEntry(result.Path))
                                {
                                    data = user.Properties["thumbnailPhoto"].Value as byte[];

                                    if (data == null)
                                        data = user.Properties["jpegPhoto"].Value as byte[];
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            });

            return data;
        }
    }
}

using Cauldron.Activator;
using Cauldron.Core.Extensions;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Potions
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public sealed partial class UserInformation
    {
        private string _displayName;
        private string _emailAddress;
        private string _firstName;
        private string _homeDirectory;
        private bool _isLockedOut;
        private string _lastName;
        private string _principalName;
        private string _telephoneNumber;

        /// <summary>
        /// Initializes a new instance of <see cref="UserInformation"/> with <see cref="WindowsIdentity.GetCurrent()"/>.Name
        /// </summary>
        private UserInformation() : this(WindowsIdentity.GetCurrent().Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UserInformation"/>
        /// </summary>
        /// <param name="userName">The domain and id of the user</param>
        /// <exception cref="ArgumentNullException"><paramref name="userName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="userName"/> is empty</exception>
        private UserInformation(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (userName == "")
                throw new ArgumentException("Parameter 'userName' cannot be empty", nameof(userName));

            if (!userName.Contains("\\"))
            {
                this._domainName = Environment.UserDomainName;
                this._username = userName;
            }
            else
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
            get { return string.Equals(Environment.MachineName, this._domainName, StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Creates a new instance of the object using <see cref="Factory.Create(string, object[])"/>.
        /// </summary>
        /// <param name="userName">The domain and id of the user</param>
        /// <returns>A new instance of the object</returns>
        public static IUserInformation CreateInstance(string userName) => Factory.Create<IUserInformation>(userName);

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public Task<string> GetDomainNameAsync() => Task.FromResult(this._domainName);

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        public async Task<string> GetEmailAddressAsync()
        {
            await this.GetUserInformationAsync();
            return this._emailAddress;
        }

        /// <summary>
        /// Gets the user's home directory.
        /// </summary>
        /// <returns>The user's home directory</returns>
        public async Task<DirectoryInfo> GetHomeDirectory()
        {
            await this.GetUserInformationAsync();

            if (Directory.Exists(this._homeDirectory))
                return new DirectoryInfo(this._homeDirectory);

            return null;
        }

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        public async Task<string> GetTelephoneNumberAsync()
        {
            await this.GetUserInformationAsync();
            return this._telephoneNumber;
        }

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public Task<string> GetUserNameAsync() => Task.FromResult(this._username);

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        public async Task<bool> IsLockedOutAsync()
        {
            await this.GetUserInformationAsync();
            return this._isLockedOut;
        }

        private string GetAccountPicturePathFromDatFile()
        {
            try
            {
                var programData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\User Account Pictures");
                var filename = Path.Combine(programData, string.Format("{0}+{1}.dat", this._domainName, this._username));

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
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, this._username))
                {
                    this._emailAddress = user.EmailAddress?.ToLower();
                    this._displayName = user.DisplayName;
                    this._firstName = user.GivenName;
                    this._lastName = user.Surname;
                    this._telephoneNumber = user.VoiceTelephoneNumber;
                    this._isLockedOut = user.IsAccountLockedOut();
                    this._homeDirectory = user.HomeDirectory;
                    this._principalName = user.UserPrincipalName;
                }
            }
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
                                dsSearcher.Filter = "(&(objectCategory=user)(|(CN={0})(sAMAccountName={0})))".ToString(this._username);
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
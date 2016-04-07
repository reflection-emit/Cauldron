using System;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Couldron.Core
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public sealed class UserInformation
    {
        private static object lockCurrentObject = new object();
        private static volatile UserInformation userInformation;

        private string _displayName;
        private string _emailAddress;
        private string _firstName;
        private bool _isLockedOut;
        private string _lastName;
        private string _telephoneNumber;
        private volatile bool loaded = false;
        private object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of <see cref="UserInformation"/>
        /// </summary>
        /// <param name="userName">The domain and id of the user</param>
        public UserInformation(string userName)
        {
            this.DomainName = Path.GetDirectoryName(userName);
            this.UserName = Path.GetFileName(userName);
        }

        /// <summary>
        /// Gets the <see cref="UserInformation"/> of the current user that runs the application
        /// </summary>
        public static UserInformation Current
        {
            get
            {
                if (userInformation == null)
                {
                    lock (lockCurrentObject)
                    {
                        if (userInformation == null)
                        {
                            userInformation = new UserInformation(WindowsIdentity.GetCurrent().Name);
                        }
                    }
                }
                return userInformation;
            }
        }

        /// <summary>
        /// Gets the domain of the user
        /// </summary>
        public string DomainName { get; private set; }

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        public bool IsLocalAccount
        {
            get { return string.Equals(Environment.MachineName, this.DomainName, StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Gets the username the application is running on
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        public BitmapImage GetAccountPicture()
        {
            var result = GetAccountPictureFromDatFile();

            if (result == null)
            {
                var stringBuilder = new StringBuilder(1000);
                UnsafeNative.GetUserTilePath(this.UserName, 0x80000000, stringBuilder, stringBuilder.Capacity);
                return new BitmapImage(new Uri(stringBuilder.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name of the user account</returns>
        public async Task<string> GetDisplayNameAsync()
        {
            await this.GetInformation();
            return this._displayName;
        }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        public async Task<string> GetEmailAddressAsync()
        {
            await this.GetInformation();
            return this._emailAddress;
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name</returns>
        public async Task<string> GetFirstNameAsync()
        {
            await this.GetInformation();
            return this._firstName;
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name</returns>
        public async Task<string> GetLastNameAsync()
        {
            await this.GetInformation();
            return this._lastName;
        }

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        public async Task<string> GetTelephoneNumberAsync()
        {
            await this.GetInformation();
            return this._telephoneNumber;
        }

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        public async Task<bool> IsLockedOutAsync()
        {
            await this.GetInformation();
            return this._isLockedOut;
        }

        private BitmapImage GetAccountPictureFromDatFile()
        {
            var programData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\User Account Pictures");
            var filename = Path.Combine(programData, string.Format("{0}+{1}.dat", DomainName, UserName));

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

            return new BitmapImage(new Uri(path));
        }

        private async Task GetInformation()
        {
            await Task.Run(() =>
            {
                if (!this.loaded)
                {
                    lock (this.lockObject)
                    {
                        if (!loaded)
                        {
                            try
                            {
                                if (this.IsLocalAccount)
                                    // The user is not a domain user
                                    this.GetInformation(ContextType.Machine);
                                else if (Utils.IsNetworkAvailable)
                                    // Try to gather information only if we have any network connection
                                    this.GetInformation(ContextType.Domain);
                                else
                                {
                                    // Else we just try our best
                                    this._emailAddress = UserName + "@" + this.DomainName;
                                    this._displayName = UserName;
                                    this._firstName = string.Empty;
                                    this._lastName = UserName;
                                    this._telephoneNumber = string.Empty;
                                    this._isLockedOut = false;
                                }
                            }
                            catch
                            {
                                this._emailAddress = this.UserName + "@" + this.DomainName;
                                this._displayName = this.UserName;
                                this._lastName = this.UserName;
                            }

                            this.loaded = true;
                        }
                    }
                }
            });
        }

        private void GetInformation(ContextType contextType)
        {
            using (var context = new PrincipalContext(contextType))
            {
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserName))
                {
                    this._emailAddress = user.EmailAddress;
                    this._displayName = user.DisplayName;
                    this._firstName = user.GivenName;
                    this._lastName = user.Surname;
                    this._telephoneNumber = user.VoiceTelephoneNumber;
                    this._isLockedOut = user.IsAccountLockedOut();
                }
            }
        }
    }
}
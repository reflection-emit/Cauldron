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
    public static class UserInformation
    {
        private static string _displayName;
        private static string _emailAddress;
        private static string _firstName;
        private static bool _isLockedOut;
        private static string _lastName;
        private static string _telephoneNumber;
        private static volatile bool loaded = false;
        private static object lockObject = new object();

        /// <summary>
        /// Gets the domain of the user
        /// </summary>
        public static string DomainName
        {
            get
            {
                return Path.GetDirectoryName(WindowsIdentity.GetCurrent().Name);
            }
        }

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        public static bool IsLocalAccount
        {
            get { return string.Equals(Environment.MachineName, DomainName, StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Gets the username the application is running on
        /// </summary>
        public static string UserName
        {
            get
            {
                return Path.GetFileName(WindowsIdentity.GetCurrent().Name);
            }
        }

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns></returns>
        public static BitmapImage GetAccountPicture()
        {
            var sb = new StringBuilder(1000);
            UnsafeNative.GetUserTilePath(UserName, 0x80000000, sb, sb.Capacity);
            return new BitmapImage(new Uri(sb.ToString()));
        }

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name of the user account</returns>
        public static async Task<string> GetDisplayNameAsync()
        {
            await GetInformation();
            return _displayName;
        }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        public static async Task<string> GetEmailAddressAsync()
        {
            await GetInformation();
            return _emailAddress;
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name</returns>
        public static async Task<string> GetFirstNameAsync()
        {
            await GetInformation();
            return _firstName;
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name</returns>
        public static async Task<string> GetLastNameAsync()
        {
            await GetInformation();
            return _lastName;
        }

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        public static async Task<string> GetTelephoneNumberAsync()
        {
            await GetInformation();
            return _telephoneNumber;
        }

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        public static async Task<bool> IsLockedOutAsync()
        {
            await GetInformation();
            return _isLockedOut;
        }

        private static async Task GetInformation()
        {
            await Task.Run(() =>
            {
                if (!loaded)
                {
                    lock (lockObject)
                    {
                        if (!loaded)
                        {
                            try
                            {
                                if (IsLocalAccount)
                                    // The user is not a domain user
                                    GetInformation(ContextType.Machine);
                                else if (Utils.IsNetworkAvailable)
                                    // Try to gather information only if we have any network connection
                                    GetInformation(ContextType.Domain);
                                else
                                {
                                    // Else we just try our best
                                    _emailAddress = UserName + "@" + DomainName;
                                    _displayName = UserName;
                                    _firstName = string.Empty;
                                    _lastName = UserName;
                                    _telephoneNumber = string.Empty;
                                    _isLockedOut = false;
                                }
                            }
                            catch
                            {
                                _emailAddress = UserName + "@" + DomainName;
                                _displayName = UserName;
                                _lastName = UserName;
                            }

                            loaded = true;
                        }
                    }
                }
            });
        }

        private static void GetInformation(ContextType contextType)
        {
            using (var context = new PrincipalContext(contextType))
            {
                using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserName))
                {
                    _emailAddress = user.EmailAddress;
                    _displayName = user.DisplayName;
                    _firstName = user.GivenName;
                    _lastName = user.Surname;
                    _telephoneNumber = user.VoiceTelephoneNumber;
                    _isLockedOut = user.IsAccountLockedOut();
                }
            }
        }
    }
}
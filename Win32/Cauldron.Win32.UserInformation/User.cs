using Cauldron.Core.Net;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents information about the user, such as name and account picture.
    /// </summary>
    public sealed class User : IEquatable<User>
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
        private string _wtsClientName;
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
                this._domainName = splittedInfo[1].ToLower();
                this._username = splittedInfo[0].ToLower();
            }
            else if (!userName.Contains("\\")) // johnd
            {
                this._domainName = Environment.UserDomainName.ToLower();
                this._username = userName.ToLower();
            }
            else // companydomain\johnd
            {
                this._domainName = Path.GetDirectoryName(userName).ToLower();
                this._username = Path.GetFileName(userName).ToLower();
            }
        }

        /// <summary>
        /// Gets the account picture for the user.
        /// </summary>
        /// <returns>The image of the user</returns>
        public byte[] AccountPicture
        {
            get
            {
                if (!IsLocalAccount)
                {
                    var data = this.GetUserPictureFromActiveDirectory();

                    if (data != null)
                        return data;
                }

                var path = GetAccountPicturePathFromDatFile();

                if (path == null)
                    path = UnsafeNative.GetUserTilePath(_username);

                if (File.Exists(path))
                    return File.ReadAllBytes(path);

                return null;
            }
        }

        /// <summary>
        /// Gets the display name for the user account.
        /// </summary>
        /// <returns>The display name for the user account.</returns>
        public string DisplayName
        {
            get
            {
                this.GetUserInformation();
                return _displayName;
            }
        }

        /// <summary>
        /// Gets the domain name for the user.
        /// </summary>
        /// <returns>A string that represents the domain name for the user.</returns>
        public string DomainName => _domainName;

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        /// <returns>The user's email address name</returns>
        public string EmailAddress
        {
            get
            {
                this.GetUserInformation();
                return _emailAddress;
            }
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        /// <returns>The user's first name.</returns>
        public string FirstName
        {
            get
            {
                this.GetUserInformation();
                return _firstName;
            }
        }

        /// <summary>
        /// Gets the user's home directory.
        /// </summary>
        /// <returns>The user's home directory</returns>
        public DirectoryInfo HomeDirectory
        {
            get
            {
                this.GetUserInformation();

                if (Directory.Exists(_homeDirectory))
                    return new DirectoryInfo(_homeDirectory);

                return null;
            }
        }

        /// <summary>
        /// Gets a value that indicates if the user account is local or domain.
        /// <para/>
        /// Returns true if the account is a local account, otherwise false
        /// </summary>
        public bool IsLocalAccount => string.Equals(Environment.MachineName, _domainName, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets a value that indicates if the user is locked out or not
        /// </summary>
        /// <returns>Returns true if the user is locked out, otherwise false</returns>
        public bool IsLockedOut
        {
            get
            {
                this.GetUserInformation();
                return _isLockedOut;
            }
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        /// <returns>The user's last name.</returns>
        public string LastName
        {
            get
            {
                this.GetUserInformation();
                return _lastName;
            }
        }

        /// <summary>
        /// Gets the principal name for the user. This name is the User Principal Name (typically the
        /// user's address, although this is not always true.)
        /// </summary>
        /// <returns>The user's principal name.</returns>
        public string PrincipalName
        {
            get
            {
                this.GetUserInformation();
                return _principalName;
            }
        }

        /// <summary>
        /// Gets the telephone number of the user
        /// </summary>
        /// <returns>The telephone number of the user</returns>
        public string TelephoneNumber
        {
            get
            {
                this.GetUserInformation();
                return _telephoneNumber;
            }
        }

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <returns>A string that represents the user name of the user.</returns>
        public string UserName => _username;

        /// <summary>
        /// Gets a the user's Windows Terminal Service's client name. The value will fallback to <see cref="Environment.MachineName"/> if there is no client name available.
        /// </summary>
        public string WTSClientName
        {
            get
            {
                if (this._wtsClientName == null)
                {
                    var buffer = IntPtr.Zero;

                    try
                    {
                        UnsafeNative.WTSQuerySessionInformation(IntPtr.Zero,
                            UnsafeNative.WTS_CURRENT_SESSION,
                            UnsafeNative.WTS_INFO_CLASS.WTSClientName,
                            out buffer, out uint returnedBytes);

                        this._wtsClientName = Marshal.PtrToStringAnsi(buffer, (int)returnedBytes - 1);
                    }
                    catch
                    {
                        this._wtsClientName = Environment.GetEnvironmentVariable("ClientName");

                        if (string.IsNullOrEmpty(this._wtsClientName))
                            this._wtsClientName = Environment.MachineName;
                    }
                    finally
                    {
                        UnsafeNative.WTSFreeMemory(buffer);
                    }
                }

                return this._wtsClientName;
            }
        }

        /// <exclude/>
        public static bool operator !=(User a, User b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return false;

            if (object.Equals(a, null))
                return true;

            return !a.Equals(b);
        }

        /// <exclude/>
        public static bool operator ==(User a, User b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return true;

            if (object.Equals(a, null))
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="obj">The object to compare with the current object</param>
        /// <returns>True if equal; otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj is User)
                return this.Equals(obj as User);

            return false;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="other">The object to compare with the current object</param>
        /// <returns>True if equal; otherwise false</returns>
        public bool Equals(User other) =>
            !object.Equals(other, null) &&
            (
                object.ReferenceEquals(this, other) ||
                (
                    other._username.Equals(this._username, StringComparison.InvariantCultureIgnoreCase) &&
                    other._domainName.Equals(this._domainName, StringComparison.InvariantCultureIgnoreCase)
                )
            );

        /// <summary>
        /// Returns the hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => this._domainName.GetHashCode() ^ this._username.GetHashCode();

        /// <summary>
        /// Returns domainname and username
        /// </summary>
        /// <returns>The domain and user name</returns>
        public override string ToString() => this._domainName + "\\" + this._username;

        private string GetAccountPicturePathFromDatFile()
        {
            try
            {
                var programData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\User Account Pictures");
                var filename = string.Format("{0}+{1}.dat", _domainName, _username);
                var filepath = Path.Combine(programData, filename);

                if (!File.Exists(filepath))
                {
                    filepath = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), filename, SearchOption.AllDirectories)
                        .FirstOrDefault();

                    if (!File.Exists(filepath))
                        return null;
                }

                var data = File.ReadAllBytes(filepath);
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

        private void GetUserInformation()
        {
            if (isInitialized)
                return;

            lock (lockObject)
            {
                // we have to avoid thing that already entered and waited for the lock to be
                // released from other thread
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
        }

        private byte[] GetUserPictureFromActiveDirectory()
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
                                var data = user.Properties["thumbnailPhoto"].Value as byte[];

                                if (data == null)
                                    data = user.Properties["jpegPhoto"].Value as byte[];

                                return data;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
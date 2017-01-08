using System.IO;
using System.Threading.Tasks;

namespace Cauldron.Potions.Implementation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// A wrapper for <see cref="Cauldron.Core.UserInformation"/>.
    /// See <see cref="Cauldron.Core.UserInformation"/> for further details.
    /// </summary>
    public sealed partial class UserInformation : IUserInformation
    {
        public bool IsLocalAccount
        {
            get
            {
                return Core.UserInformation.IsLocalAccount;
            }
        }

        public Task<string> GetEmailAddressAsync() => Core.UserInformation.GetEmailAddressAsync();

        public Task<DirectoryInfo> GetHomeDirectory() => Core.UserInformation.GetHomeDirectory();

        public Task<string> GetTelephoneNumberAsync() => Core.UserInformation.GetTelephoneNumberAsync();

        public Task<bool> IsLockedOutAsync() => Core.UserInformation.IsLockedOutAsync();
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
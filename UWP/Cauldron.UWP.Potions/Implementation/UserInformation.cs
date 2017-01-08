using Cauldron.Activator;
using System.Threading.Tasks;

namespace Cauldron.Potions.Implementation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// A wrapper for <see cref="Cauldron.Core.UserInformation"/>.
    /// See <see cref="Cauldron.Core.UserInformation"/> for further details.
    /// </summary>
    [Component(typeof(IUserInformation), FactoryCreationPolicy.Instanced)]
    public sealed partial class UserInformation : IUserInformation
    {
        [ComponentConstructor]
        private UserInformation()
        {
        }

        public Task<string> GetDisplayNameAsync() =>
            Core.UserInformation.GetDisplayNameAsync();

        public Task<string> GetDomainNameAsync() =>
            Core.UserInformation.GetDomainNameAsync();

        public Task<string> GetFirstNameAsync() =>
            Core.UserInformation.GetFirstNameAsync();

        public Task<string> GetLastNameAsync() =>
            Core.UserInformation.GetLastNameAsync();

        public Task<string> GetPrincipalNameAsync() =>
            Core.UserInformation.GetPrincipalNameAsync();

        public Task<string> GetUserNameAsync() =>
            Core.UserInformation.GetUserNameAsync();
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
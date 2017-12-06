using Cauldron.Activator;
using Cauldron.Core;
using System.Threading.Tasks;

namespace Cauldron.Potions.Implementation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// A wrapper for <see cref="Cauldron.Core.Network"/>. See <see cref="Cauldron.Core.Network"/> for further details.
    /// </summary>
    [Component(typeof(ISerializer), FactoryCreationPolicy.Instanced)]
    public sealed partial class Network : INetwork
    {
        [ComponentConstructor]
        private Network()
        {
        }

        public ConnectionGenerationTypes ConnectionType { get { return Core.Network.ConnectionType; } }

        public bool HasInternetConnection { get { return Core.Network.HasInternetConnection; } }

        public Task<PingResults> Ping(string hostname, uint port) => Core.Network.Ping(hostname, port);
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
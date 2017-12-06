using Cauldron.Activator;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cauldron.Potions.Implementation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// A wrapper for <see cref="Cauldron.Core.Serializer"/>.
    /// See <see cref="Cauldron.Core.Serializer"/> for further details.
    /// </summary>
    [Component(typeof(ISerializer), FactoryCreationPolicy.Instanced)]
    public sealed class Serializer : ISerializer
    {
        [ComponentConstructor]
        private Serializer()
        {
        }

        public Task<object> DeserializeAsync(Type type, string name) =>
            Core.Serializer.DeserializeAsync(type, name);

        public Task<object> DeserializeAsync(Type type, StorageFolder folder, string name) =>
            Core.Serializer.DeserializeAsync(type, folder, name);

        public Task<T> DeserializeAsync<T>(string name) where T : class =>
            Core.Serializer.DeserializeAsync<T>(name);

        public Task<T> DeserializeAsync<T>(StorageFolder folder, string name) where T : class =>
            Core.Serializer.DeserializeAsync<T>(folder, name);

#if !NETFX_CORE

        public void Serialize(object context, StorageFolder folder, string name) =>
            Core.Serializer.Serialize(context, folder, name);

#endif

        public Task SerializeAsync(object context, string name) =>
            Core.Serializer.SerializeAsync(context, name);

        public Task SerializeAsync(object context, StorageFolder folder, string name) =>
            Core.Serializer.SerializeAsync(context, folder, name);
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
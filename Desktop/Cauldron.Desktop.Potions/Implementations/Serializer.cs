using Cauldron.Activator;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public Task<object> DeserializeAsync(Type type, DirectoryInfo folder, string name) =>
            Core.Serializer.DeserializeAsync(type, folder, name);

        public Task<T> DeserializeAsync<T>(string name) where T : class =>
            Core.Serializer.DeserializeAsync<T>(name);

        public Task<T> DeserializeAsync<T>(DirectoryInfo folder, string name) where T : class =>
            Core.Serializer.DeserializeAsync<T>(folder, name);

        public void Serialize(object context, DirectoryInfo folder, string name) =>
            Core.Serializer.Serialize(context, folder, name);

        public Task SerializeAsync(object context, string name) =>
            Core.Serializer.SerializeAsync(context, name);

        public Task SerializeAsync(object context, DirectoryInfo folder, string name) =>
            Core.Serializer.SerializeAsync(context, folder, name);
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
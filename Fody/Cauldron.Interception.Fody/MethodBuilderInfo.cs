using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public interface IMethodBuilderInfoItem
    {
        AttributedMethod Attribute { get; }
        bool HasSyncRootInterface { get; }
    }

    public sealed class MethodBuilderInfo<T> where T : IMethodBuilderInfoItem
    {
        private Field _syncRoot;

        public MethodBuilderInfo(MethodKey key, IEnumerable<T> items)
        {
            this.Key = key;
            this.Item = items.ToArray();
        }

        public T[] Item { get; private set; }

        public MethodKey Key { get; private set; }

        public bool RequiresSyncRootField => this.Item?.Any(x => x.HasSyncRootInterface) ?? false;

        public Field SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    _syncRoot = this.Key.Method.DeclaringType.CreateField(this.Key.Method.Modifiers.GetPrivate(), typeof(object), $"<{this.Key.Method.Name}>_syncObject_{this.Key.Method.Identification}");
                    _syncRoot.CustomAttributes.AddNonSerializedAttribute();
                }

                return _syncRoot;
            }
        }
    }

    public sealed class MethodBuilderInfoItem<T> : IMethodBuilderInfoItem
    {
        public MethodBuilderInfoItem(AttributedMethod attribute, T @interface)
        {
            this.Attribute = attribute;
            this.Interface = @interface;
            this.HasSyncRootInterface = attribute.Attribute.Type.Implements(__ISyncRoot.TypeName);
        }

        public AttributedMethod Attribute { get; private set; }
        public bool HasSyncRootInterface { get; private set; }
        public T Interface { get; private set; }
    }

    public sealed class MethodKey
    {
        public MethodKey(Method method, Method asyncMethod)
        {
            this.Method = method;
            this.AsyncMethod = asyncMethod;
        }

        public Method AsyncMethod { get; private set; }
        public Method Method { get; private set; }

        public override bool Equals(object obj) => (obj as MethodKey)?.Method.Equals(this.Method) ?? false;

        public override int GetHashCode() => this.Method.GetHashCode();
    }
}
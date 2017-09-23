using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class MethodBuilderInfo
    {
        public MethodBuilderInfo(MethodKey key, IEnumerable<MethodBuilderInfoItem> items)
        {
            this.Key = key;
            this.Item = items.ToArray();
        }

        public MethodBuilderInfoItem[] Item { get; private set; }
        public MethodKey Key { get; private set; }
    }

    public sealed class MethodBuilderInfoItem
    {
        public MethodBuilderInfoItem(AttributedMethod attribute, __IMethodInterceptor @interface)
        {
            this.Attribute = attribute;
            this.Interface = @interface;
        }

        public AttributedMethod Attribute { get; private set; }
        public __IMethodInterceptor Interface { get; private set; }
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

        public override int GetHashCode()
        {
            if (this.AsyncMethod != null && this.Method != null)
                return this.AsyncMethod.GetHashCode() ^ this.Method.GetHashCode();

            if (this.AsyncMethod != null && this.Method == null)
                return this.AsyncMethod.GetHashCode();

            if (this.AsyncMethod == null && this.Method != null)
                return this.Method.GetHashCode();

            return base.GetHashCode();
        }
    }
}
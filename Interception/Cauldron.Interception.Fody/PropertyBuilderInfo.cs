using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class PropertyBuilderInfo
    {
        public PropertyBuilderInfo(Property property, IEnumerable<PropertyBuilderInfoItem> item)
        {
            this.Property = property;
            this.InterceptorInfos = item.ToArray();
        }

        public bool HasGetterInterception => this.InterceptorInfos.Any(x => x.InterfaceGetter != null);
        public bool HasSetterInterception => this.InterceptorInfos.Any(x => x.InterfaceSetter != null);
        public PropertyBuilderInfoItem[] InterceptorInfos { get; private set; }
        public Property Property { get; private set; }
    }

    public sealed class PropertyBuilderInfoItem
    {
        public PropertyBuilderInfoItem(AttributedProperty attribute, Property property, __IPropertyGetterInterceptor interfaceGetter, __IPropertySetterInterceptor interfaceSetter)
        {
            this.Attribute = attribute;
            this.InterfaceGetter = interfaceGetter;
            this.InterfaceSetter = interfaceSetter;
            this.Property = property;
        }

        public AttributedProperty Attribute { get; private set; }
        public __IPropertyGetterInterceptor InterfaceGetter { get; private set; }
        public __IPropertySetterInterceptor InterfaceSetter { get; private set; }
        public Property Property { get; private set; }
    }
}
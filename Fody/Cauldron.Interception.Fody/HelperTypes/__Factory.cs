using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Activator.Factory")]
    public sealed class __Factory : HelperTypeBase<__Factory>
    {
        [HelperTypeMethod("Create", "System.String", "System.Object[]")]
        public Method Create { get; private set; }

        [HelperTypeMethod("CreateFirst", "System.String", "System.Object[]")]
        public Method CreateFirst { get; private set; }

        [HelperTypeMethod("CreateMany", "System.String", "System.Object[]")]
        public Method CreateMany { get; private set; }

        [HelperTypeMethod("CreateManyOrdered", "System.String", "System.Object[]")]
        public Method CreateManyOrdered { get; private set; }

        [HelperTypeMethod("GetFactoryTypeInfo", 1)]
        public Method GetFactoryTypeInfo { get; private set; }

        [HelperTypeMethod("OnObjectCreation", 2)]
        public Method OnObjectCreation { get; private set; }
    }
}
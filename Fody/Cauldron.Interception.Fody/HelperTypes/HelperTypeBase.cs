using Cauldron.Interception.Cecilator;
using System;
using System.Linq;
using System.Reflection;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public abstract class HelperTypeBase<T> where T : HelperTypeBase<T>, new()
    {
        private BuilderType builderType;

        public HelperTypeBase()
        {
            if (Type == null)
                throw new TypeNotFoundException($"Unable to find type: '{Name}'");

            this.FindMethods();
            this.FindFields();
        }

        public static T Instance => new T();

        public static bool IsReferenced => Builder.Current.TypeExists(Name);

        public static string Name
        {
            get
            {
                var builder = Builder.Current;
                var type = typeof(T);
                var attrib = type.GetCustomAttribute<HelperTypeNameAttribute>();

                if (attrib == null)
                    throw new Exception("The helper needs a HelperTypeNameAttribute");

                if (builder.IsUWP)
                    return string.IsNullOrEmpty(attrib.UWPFullname) ? attrib.Fullname : attrib.UWPFullname;
                else
                    return attrib.Fullname;
            }
        }

        public static BuilderType Type
        {
            get
            {
                var builder = Builder.Current;
                var result = Name;

                if (builder.TypeExists(result))
                    return builder.GetType(result).Import();

                throw new TypeNotFoundException($"The type '{result}' does not exist.");
            }
        }

        public BuilderType ToBuilderType
        {
            get
            {
                if (builderType == null)
                    builderType = Type;

                return builderType;
            }
        }

        private void FindFields()
        {
            foreach (var property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.PropertyType == typeof(Field)))
            {
                var attrib = property.GetCustomAttribute<HelperTypeFieldAttribute>();
                if (attrib == null)
                    throw new Exception($"The property '{property.Name}' needs a HelperTypeFieldAttribute");

                property.SetValue(this, attrib.GetField(Type));
            }
        }

        private void FindMethods()
        {
            foreach (var property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.PropertyType == typeof(Method)))
            {
                var attrib = property.GetCustomAttribute<HelperTypeMethodAttribute>();
                if (attrib == null)
                    throw new Exception($"The property '{property.Name}' needs a HelperTypeMethodAttribute");

                property.SetValue(this, attrib.GetMethod(Type));
            }
        }
    }
}
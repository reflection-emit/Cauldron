using Cauldron.Interception.Cecilator;
using System;
using System.Linq;
using System.Reflection;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public abstract class HelperTypeBase<T> where T : HelperTypeBase<T>, new()
    {
        private BuilderType builderType;

        public HelperTypeBase() => this.FindMethods();

        public static T Instance => new T();

        public static string Name
        {
            get
            {
                var builder = Builder.Current;
                var type = typeof(T);
                var attrib = type.GetCustomAttribute<HelperTypeNameAttribute>();

                if (attrib == null)
                    throw new Exception("The helper needs a HelperTypeNameAttribute");

                if (builder.TypeExists(attrib.Fullname1))
                    return attrib.Fullname1;
                else if (!string.IsNullOrEmpty(attrib.Fullname2) && builder.TypeExists(attrib.Fullname2))
                    return attrib.Fullname2;
                else
                    return null;
            }
        }

        public static BuilderType Type
        {
            get
            {
                var builder = Builder.Current;
                var result = Name;

                if (result == null)
                    return null;

                return builder.GetType(result);
            }
        }

        public BuilderType ToBuilderType
        {
            get
            {
                if (builderType == null)
                {
                    var attrib = this.GetType().GetCustomAttribute<HelperTypeNameAttribute>();

                    if (attrib == null)
                        throw new Exception("The helper needs a HelperTypeNameAttribute");

                    builderType = Type;

                    if (builderType == null)
                        throw new TypeNotFoundException($"The type '{attrib.Fullname1}' or '{attrib.Fullname2}' does not exist.");
                }

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
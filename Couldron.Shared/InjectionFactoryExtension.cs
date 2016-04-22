using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Cauldron
{
    /// <summary>
    /// Adds injection functionality to the <see cref="Factory"/>
    /// </summary>
    public sealed class InjectionFactoryExtension : IFactoryExtension
    {
        private ConcurrentDictionary<TypeInfo, ImportInstanceInfo> types = new ConcurrentDictionary<TypeInfo, ImportInstanceInfo>();

        /// <summary>
        /// Returns true if a <see cref="Type"/> can be constructed with this <see cref="IFactoryExtension"/> implementation
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <param name="objectTypeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        /// <returns>True if can be constructed</returns>
        public bool CanConstruct(Type objectType, TypeInfo objectTypeInfo)
        {
            if (!this.types.ContainsKey(objectTypeInfo))
                return false;

            return this.types[objectTypeInfo].constructor != null;
        }

        /// <summary>
        /// Creates an <see cref="object"/> described by <paramref name="objectType"/> or <paramref name="objectTypeInfo"/>.
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <param name="objectTypeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        /// <returns>The new instance of <paramref name="objectType"/> or <paramref name="objectTypeInfo"/></returns>
        public object Construct(Type objectType, TypeInfo objectTypeInfo)
        {
            var info = this.types[objectTypeInfo];

            var parameterInfos = info.constructor.GetParameters();
            var parameters = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
                parameters[i] = Factory.Create(parameterInfos[i].ParameterType);

            return Activator.CreateInstance(objectType, parameters);
        }

        /// <summary>
        /// Occures when an object is created
        /// </summary>
        /// <param name="context">The object instance</param>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <param name="objectTypeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        public void OnCreateObject(object context, Type objectType, TypeInfo objectTypeInfo)
        {
            if (!this.types.ContainsKey(objectTypeInfo))
                return;

            var info = this.types[objectTypeInfo];

            if (info.fields != null)
            {
                for (int i = 0; i < info.fields.Length; i++)
                {
                    if (info.fields[i].FieldType.ImplementsInterface<IEnumerable>())
                        info.fields[i].SetValue(context, Factory.CreateMany(info.fields[i].FieldType));
                    else
                        info.fields[i].SetValue(context, Factory.Create(info.fields[i].FieldType));
                }
            }

            if (info.properties != null)
            {
                for (int i = 0; i < info.properties.Length; i++)
                {
                    if (info.properties[i].PropertyType.ImplementsInterface<IEnumerable>())
                        info.properties[i].SetValue(context, Factory.Create(info.properties[i].PropertyType));
                    else
                        info.properties[i].SetValue(context, Factory.Create(info.properties[i].PropertyType));
                }
            }
        }

        /// <summary>
        /// Occures when <see cref="Factory"/> is initialized
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        /// <param name="type">The <see cref="Type"/> of the object created</param>
        public void OnInitialize(TypeInfo typeInfo, Type type)
        {
            this.types.TryAdd(typeInfo, new ImportInstanceInfo
            {
                fields = type.GetRuntimeFields().Where(x => x.GetCustomAttribute<InjectAttribute>() != null).ToArray(),
                properties = type.GetRuntimeProperties().Where(x => x.CanWrite && x.GetCustomAttribute<InjectAttribute>() != null).ToArray(),
                constructor = type.GetConstructors().FirstOrDefault(x => x.GetCustomAttribute<InjectAttribute>() != null),
                typeInfo = typeInfo
            });
        }

        private struct ImportInstanceInfo
        {
            public ConstructorInfo constructor;
            public FieldInfo[] fields;
            public PropertyInfo[] properties;
            public TypeInfo typeInfo;
        }
    }
}
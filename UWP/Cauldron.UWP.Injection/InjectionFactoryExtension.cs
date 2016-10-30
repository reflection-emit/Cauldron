using Cauldron.Activator;
using Cauldron.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cauldron.Injection
{
    /// <summary>
    /// Adds injection functionality to the <see cref="Factory"/>
    /// </summary>
    public sealed class InjectionFactoryExtension : IFactoryExtension
    {
        private ConcurrentDictionary<Type, ImportInstanceInfo> types = new ConcurrentDictionary<Type, ImportInstanceInfo>();

        /// <summary>
        /// Gets a value that indicates that this extension is able to resolve <see cref="AmbiguousMatchException"/>
        /// </summary>
        public bool CanHandleAmbiguousMatch { get { return false; } }

        /// <summary>
        /// Returns true if a <see cref="Type"/> can be modify arguments passed to <see cref="IFactoryExtension.ModifyArgument(ParameterInfo[], object[])"/> with this <see cref="IFactoryExtension"/> implementation
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <returns>True if can be manipulated</returns>
        public bool CanModifyArguments(MethodBase method, Type objectType)
        {
            if (types.ContainsKey(objectType))
            {
                var o = types[objectType];
                if (!o.canManipulate.HasValue)
                    o.canManipulate = method.GetParameters().Any(x => x.GetCustomAttribute<InjectAttribute>() != null);
                return o.canManipulate.Value;
            }

            return false;
        }

        /// <summary>
        /// Modifies the arguments defined by <paramref name="arguments"/> and returns the modified array
        /// </summary>
        /// <param name="argumentTypes">The parameter types of the constructor</param>
        /// <param name="arguments">The arguments used to create an object</param>
        /// <returns>A modified array of arguments</returns>
        public object[] ModifyArgument(ParameterInfo[] argumentTypes, object[] arguments)
        {
            var result = new object[argumentTypes.Length];

            if (arguments.Length < argumentTypes.Length)
            {
                int counter = 0;
                for (int i = 0; i < argumentTypes.Length; i++)
                    result[i] = argumentTypes[i].GetCustomAttribute<InjectAttribute>() == null ? arguments[counter++] : Factory.Create(argumentTypes[i].ParameterType);
            }
            else if (arguments.Length < argumentTypes.Length)
                throw new ArgumentOutOfRangeException($"{argumentTypes.Length} or less arguments expected, but there were {arguments.Length} arguments passed to {argumentTypes[0].Member.DeclaringType.FullName}");
            else
                for (int i = 0; i < argumentTypes.Length; i++)
                    result[i] = argumentTypes[i].GetCustomAttribute<InjectAttribute>() == null ? arguments[i] : Factory.Create(argumentTypes[i].ParameterType);

            return result;
        }

        /// <summary>
        /// Occures when an object is created
        /// </summary>
        /// <param name="context">The object instance</param>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        public void OnCreateObject(object context, Type objectType)
        {
            if (!this.types.ContainsKey(objectType))
                return;

            var info = this.types[objectType];

            if (info.fields != null)
            {
                for (int i = 0; i < info.fields.Length; i++)
                {
                    if (info.fields[i].FieldType.ImplementsInterface(typeof(IEnumerable<>)))
                        info.fields[i].SetValue(context, this.CreateManyObject(info.fields[i].FieldType));
                    else
                        info.fields[i].SetValue(context, Factory.Create(info.fields[i].FieldType));
                }
            }

            if (info.properties != null)
            {
                for (int i = 0; i < info.properties.Length; i++)
                {
                    if (info.properties[i].PropertyType.ImplementsInterface(typeof(IEnumerable<>)))
                        info.properties[i].SetValue(context, this.CreateManyObject(info.properties[i].PropertyType));
                    else
                        info.properties[i].SetValue(context, Factory.Create(info.properties[i].PropertyType));
                }
            }
        }

        /// <summary>
        /// Occures when <see cref="Factory"/> is initialized
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object created</param>
        public void OnInitialize(Type type)
        {
            this.types.TryAdd(type, new ImportInstanceInfo
            {
                fields = type.GetFieldsEx(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(x => x.GetCustomAttribute<InjectAttribute>() != null).ToArray(),
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(x => x.CanWrite && x.GetCustomAttribute<InjectAttribute>() != null).ToArray(),
            });
        }

        /// <summary>
        /// Occures if multiple Types with the same <paramref name="contractName"/> was found.
        /// Should return null if <paramref name="ambiguousTypes"/> collection does not contain the required <see cref="Type"/>
        /// </summary>
        /// <param name="callingAssembly">The Assembly of the method that invoked the <see cref="Factory"/>.</param>
        /// <param name="ambiguousTypes">A collection of Types that with the same <paramref name="contractName"/></param>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>The selected <see cref="Type"/></returns>
        public Type SelectAmbiguousMatch(IEnumerable<Type> ambiguousTypes, string contractName)
        {
            throw new NotImplementedException();
        }

        private object CreateManyObject(Type type)
        {
            var childType = type.GetChildrenType();
            var objects = Factory.CreateMany(childType);

            var context = Factory.CreateInstance(type);

            // Let us check if the type has a suitable AddRange method
            // But... this will take longer than just getting the first best addrange and passing the collection to it

            // This will fail in UWP in most cases because the addition of the certain type to rd.xml will be missing.
            // AddRange is also not defined by any of the interfaces... That is why it is very hard to handle this.
            var addRange = type.GetMethod("AddRange", new Type[] { typeof(IEnumerable<>).MakeGenericType(childType) }, BindingFlags.Instance | BindingFlags.Public);

            if (addRange != null)
            {
                try
                {
                    addRange.Invoke(context, new object[] { objects });
                    return context;
                }
                catch
                {
                    // Oh no!
                }
            }

            // Lets makes this one UWP native friendly... It is also muchg faster than reflection
            var list = context as IList;

            if (list != null)
            {
                foreach (var item in objects)
                    list.Add(item);

                return context;
            }

            // No add method? No AddRange? .. What is this sorcery
            return objects;
        }

        private struct ImportInstanceInfo
        {
            public bool? canManipulate;
            public FieldInfo[] fields;
            public PropertyInfo[] properties;
        }
    }
}
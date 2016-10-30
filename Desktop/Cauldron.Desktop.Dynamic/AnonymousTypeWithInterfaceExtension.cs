using Cauldron.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Cauldron.Dynamic
{
    /// <summary>
    /// Provides usefull extension methods for anonymous class
    /// </summary>
    public static class AnonymousTypeWithInterfaceExtension
    {
        private static ConcurrentDictionary<string, Type> types = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Creates a new <see cref="Type"/> that implements the properties of an interface defined by <typeparamref name="T"/>
        /// and copies all value of <paramref name="anon"/> to the new object
        /// </summary>
        /// <typeparam name="T">The type of interface to implement</typeparam>
        /// <param name="anon">The anonymous object</param>
        /// <returns>A new object implementing the interface defined by <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> is not an interface</exception>
        public static T CreateObject<T>(this object anon) where T : class
        {
            var type = typeof(T);

            if (type.IsInterface || !type.IsSealed)
            {
                T result = null;
                var anonymousTypeType = anon.GetType();
                var typeName = "Anon_" + (type.FullName + anonymousTypeType.FullName).GetHash(Core.HashAlgorithms.Md5);

                if (types.ContainsKey(typeName))
                    result = Activator.CreateInstance(types[typeName]) as T;
                else
                {
                    var newType = CreateType(typeName, type, anonymousTypeType);
                    types.TryAdd(typeName, newType);
                    result = Activator.CreateInstance(newType) as T;
                }

                var propertiesOfResult = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in anon.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => propertiesOfResult.Any(y => y.Name == x.Name)))
                {
                    propertiesOfResult.FirstOrDefault(x => x.Name == property.Name).SetValue(result,
                        property.GetValue(anon));
                }

                return result;
            }
            else
                throw new ArgumentException($"{type} is not an interface or is sealed.");
        }

        private static Type CreateType(string name, Type type, Type anonymousTypeType)
        {
            using (var builder = DynamicAssembly.CreateBuilder(name))
            {
                builder.AddInterfaceImplementation(type);

                var propertiesToDefine = type.GetPropertiesEx(BindingFlags.Public | BindingFlags.Instance);
                var createdPropertyBackingFields = new List<FieldBuilder>();

                foreach (var property in propertiesToDefine)
                    createdPropertyBackingFields.Add(builder.Implement(property));

                foreach (var method in type.GetMethodsEx(BindingFlags.Public | BindingFlags.Instance).Where(x => !x.IsSpecialName))
                    builder.Implement(method);

                // Implement a copyer method
                builder.Implement("CopyValues", x =>
                {
                    var typeLocal = x.DeclareLocal(typeof(Type));

                    x.Emit(OpCodes.Ldarg_1);
                    x.Emit(OpCodes.Callvirt, typeof(object).GetMethod("GetType", Type.EmptyTypes, BindingFlags.Public | BindingFlags.Instance));
                    x.Emit(OpCodes.Stloc, typeLocal);

                    var propertiesInAnon = anonymousTypeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in propertiesInAnon)
                    {
                        var targetField = createdPropertyBackingFields.FirstOrDefault(y => y.Name == $"_{property.Name}_BackingField");

                        if (targetField == null)
                            continue;

                        x.Emit(OpCodes.Ldarg_0);
                        x.Emit(OpCodes.Ldloc, typeLocal);
                        x.Emit(OpCodes.Ldstr, property.Name);
                        x.Emit(OpCodes.Ldc_I4, 20);
                        x.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("GetProperty", new Type[] { typeof(string), typeof(BindingFlags) }, BindingFlags.Instance | BindingFlags.Public));
                        x.Emit(OpCodes.Ldarg_1);
                        x.Emit(OpCodes.Callvirt, typeof(PropertyInfo).GetMethod("GetValue", new Type[] { typeof(object) }, BindingFlags.Instance | BindingFlags.Public));

                        if (property.PropertyType.IsValueType)
                            x.Emit(OpCodes.Unbox_Any, property.PropertyType);
                        else
                            x.Emit(OpCodes.Castclass, property.PropertyType);
                        x.Emit(OpCodes.Stfld, targetField);
                    }
                    x.Emit(OpCodes.Ret);
                }, typeof(void), new Type[] { typeof(object) });

                return builder;
            }
        }
    }
}
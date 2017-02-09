using Cauldron.Core.Extensions;
using Cauldron.Interception;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Cauldron.Activator
{
    /// <summary>
    /// Specifies that the property or field contains a type that can be supplied by the <see cref="Factory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectAttribute : Attribute, ILockablePropertyGetterInterceptor
    {
        /// <summary>
        /// Invoked if an intercepted method has raised an exception. The method will always rethrow the exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        public void OnException(Exception e)
        {
        }

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        public void OnExit()
        {
        }

        /// <summary>
        /// Invoked if the intercepted property getter has been called
        /// </summary>
        /// <param name="semaphore">The <see cref="SemaphoreSlim"/> instance that can be used to lock the the method</param>
        /// <param name="propertyInterceptionInfo">An object that containes information about the intercepted method</param>
        /// <param name="value">The current value of the property</param>
        public void OnGet(SemaphoreSlim semaphore, PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (value == null)
            {
                semaphore.Wait();

                if (value == null)
                {
                    if (propertyInterceptionInfo.PropertyType.ImplementsInterface(typeof(IEnumerable<>)) && !Factory.HasContract(propertyInterceptionInfo.PropertyType))
                        propertyInterceptionInfo.SetValue(this.CreateManyObject(propertyInterceptionInfo.PropertyType));
                    else
                        propertyInterceptionInfo.SetValue(Factory.Create(propertyInterceptionInfo.PropertyType));
                }

                semaphore.Release();
            }
        }

        private object CreateManyObject(Type type)
        {
            var childType = type.GetChildrenType();
            var objects = Factory.CreateMany(childType);

            // Check first if it is an array, because arrays are easy to create
            if (type.IsArray)
            {
            }

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
    }
}
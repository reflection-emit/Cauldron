using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Interception;
using Cauldron.Internal;
using System;
using System.Reflection;

namespace Cauldron.Activator
{
    /// <summary>
    /// Specifies that the property or field contains a type that can be supplied by the <see cref="Factory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectAttribute : Attribute, IPropertyGetterInterceptor
    {
        private object[] arguments;

        private string contractName;

        private object syncObject = new object();
        private Type typeToCreate;

        /// <summary>
        /// Initializes a new instance of <see cref="InjectAttribute"/>
        /// </summary>
        public InjectAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InjectAttribute"/>
        /// </summary>
        /// <param name="arguments">The The arguments that can be used to initialize the instance</param>
        public InjectAttribute(params object[] arguments) : this(contractName: null, arguments: arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InjectAttribute"/>
        /// </summary>
        /// <param name="contractType">The type of the contract to inject</param>
        /// <param name="arguments">The The arguments that can be used to initialize the instance</param>
        public InjectAttribute(Type contractType, object[] arguments) : this(contractType.FullName, arguments)
        {
            this.typeToCreate = contractType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InjectAttribute"/>
        /// </summary>
        /// <param name="contractName">The name of the contract to inject</param>
        /// <param name="arguments">The The arguments that can be used to initialize the instance</param>
        public InjectAttribute(string contractName, object[] arguments)
        {
            this.contractName = contractName;
            this.arguments = arguments;
        }

        /// <summary>
        /// Invoked if an intercepted method has raised an exception. The method will always rethrow
        /// the exception.
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
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="value">The current value of the property</param>
        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (value == null)
            {
                lock (syncObject)
                {
                    if (value == null)
                    {
                        // If the property or field implements IEnumerable then it could be an array
                        // or list or anything else if it does not have a contract, then we look at
                        // its child elements... maybe they have a contract
                        if (string.IsNullOrEmpty(this.contractName))
                            this.contractName = propertyInterceptionInfo.PropertyType.FullName;

                        object injectionInstance;

                        if (Factory.HasContract(this.contractName))
                        {
                            if (this.arguments == null || this.arguments.Length == 0)
                                injectionInstance = Factory.Create(this.contractName);
                            else
                                injectionInstance = Factory.Create(this.contractName, this.arguments);
                        }
                        else if (propertyInterceptionInfo.ChildType != null && propertyInterceptionInfo.ChildType != typeof(object))
                            injectionInstance = Factory.CreateMany(propertyInterceptionInfo.ChildType);
                        else if (!propertyInterceptionInfo.PropertyType.GetTypeInfo().IsInterface)
                        {
                            // If the property type is not an interface, then we will try to create
                            // the type with its default constructor
                            if (this.arguments == null || this.arguments.Length == 0)
                                injectionInstance = this.typeToCreate == null ? propertyInterceptionInfo.PropertyType.CreateInstance() : this.typeToCreate.CreateInstance();
                            else
                                injectionInstance = this.typeToCreate == null ? propertyInterceptionInfo.PropertyType.CreateInstance(this.arguments) : this.typeToCreate.CreateInstance(this.arguments);
                        }
                        else // If everthing else fails... We will throw an exception
                            throw new InvalidOperationException($"Unable to inject the contract '{this.contractName}' to the property or field '{propertyInterceptionInfo.PropertyName}' in '{propertyInterceptionInfo.DeclaringType.FullName}'. Please make sure that the implementing type has a Component attribute.");

                        propertyInterceptionInfo.SetValue(injectionInstance);

                        // Add these to auto dispose if possible
                        var disposableInstance = injectionInstance as IDisposable;

                        if (disposableInstance != null)
                        {
                            var disposableMe = propertyInterceptionInfo.Instance as IDisposableObject;
                            // TODO - Mabe auto implement IDisposable???
                            if (disposableMe != null)
                                disposableMe.Disposed += (s, e) => disposableInstance?.Dispose();
                            else
                                Output.WriteLineError($"'{propertyInterceptionInfo.DeclaringType.FullName}' must implement '{typeof(IDisposableObject).FullName}' because '{injectionInstance}' is disposable.");
                        }
                    }
                }
            }
        }
    }
}
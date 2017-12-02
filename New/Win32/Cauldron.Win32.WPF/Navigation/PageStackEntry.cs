using Cauldron.Core.Reflection;
using Newtonsoft.Json;
using System;

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Represents an entry in the BackStack or ForwardStack of a Frame.
    /// </summary>
    public sealed class PageStackEntry
    {
        internal PageStackEntry(Type viewModelType, object[] parameters)
        {
            this.ViewModelType = viewModelType;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the navigation parameter associated with this navigation entry.
        /// </summary>
        [JsonProperty("parameters")]
        public object[] Parameters { get; private set; }

        /// <summary>
        /// Gets the type of viewmodel associated with this navigation entry.
        /// </summary>
        [JsonProperty("viewmodel-type")]
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the type fullname of viewmodel associated with this navigation entry.
        /// </summary>
        [JsonProperty("viewmodel-typename")]
        public string ViewModelTypeName
        {
            get { return this.ViewModelType.FullName; }
            set { this.ViewModelType = Assemblies.GetTypeFromName(value); }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => this.ViewModelType.Name;
    }
}
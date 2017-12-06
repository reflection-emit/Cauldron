using Cauldron.Core;
using System;
using System.Runtime.Serialization;

#if WINDOWS_UWP
using Windows.Foundation.Metadata;
#endif

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Represents an entry in the BackStack or ForwardStack of a Frame.
    /// </summary>
#if WINDOWS_UWP
    [MarshalingBehavior(MarshalingType.Agile)]
    [WebHostHidden]
    [Threading(ThreadingModel.Both)]
    [DataContract(IsReference = false, Name = "PageStackEntry", Namespace = "Cauldron.XAML.Navigation")]
#endif

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
        [DataMember]
        public object[] Parameters { get; private set; }

        /// <summary>
        /// Gets the type of viewmodel associated with this navigation entry.
        /// </summary>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the type fullname of viewmodel associated with this navigation entry.
        /// </summary>
        [DataMember]
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
using Cauldron.Consoles;
using Cauldron.Reflection;
using Cauldron.Localization;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Cauldron.WindowsService
{
    /// <summary>
    /// An execution group which adds install and uninstall execution parameters to the <see cref="ParameterParser"/>.
    /// </summary>
    /// <typeparam name="TInstaller">A <see cref="Type"/> that derives from the <see cref="WindowsServiceInstaller"/> class.</typeparam>
    [ExecutionGroup("group-name-service")]
    public sealed class ServiceExecutionGroup<TInstaller> : IExecutionGroup
        where TInstaller : WindowsServiceInstaller, new()
    {
        /// <summary>
        /// Represents the help parameter.
        /// </summary>
        [Parameter("help", "help", "h")]
        public bool Help { get; set; }

        /// <summary>
        /// Represents the install parameter.
        /// </summary>
        [Parameter("install", true, "install", "i")]
        public bool? Install { get; set; }

        /// <summary>
        /// Represents the info paremeter.
        /// </summary>
        [Parameter("service-info", "info", "I")]
        public bool ServiceInfo { get; set; }

        /// <summary>
        /// Represents the unintall parameter
        /// </summary>
        [Parameter("uninstall", true, "uninstall", "u")]
        public bool? UnInstall { get; set; }

        /// <summary>
        /// Starts the execution of the group
        /// </summary>
        /// <param name="parser">The <see cref="ParameterParser"/> instanced that executed the group</param>
        public void Execute(ParameterParser parser)
        {
            var parameters = parser.GetActiveParameters(this);

            if (this.Help)
            {
                parser.ShowHelp();
                return;
            }

            if (this.ServiceInfo)
            {
                var configJson = JsonConvert.DeserializeObject<Configuration>(
                    Assemblies.GetManifestResource(x => x.Filename.EndsWith("configuration.json", StringComparison.InvariantCultureIgnoreCase) &&
                    x.Assembly == Assembly.GetEntryAssembly())
                    .TryEncode());

                var properties = configJson
                    .GetType()
                    .GetProperties()
                    .Select(x => new { Attrib = x.GetCustomAttribute<JsonPropertyAttribute>(), Property = x })
                    .Where(x => x != null);

                ConsoleUtils.WriteTable(new ConsoleTableColumn[]
                {
                    new ConsoleTableColumn(properties.Select(x=>x.Attrib.PropertyName.ToUpper()))
                    { Foreground = ConsoleColor.Gray, WrapWords = false },
                    new ConsoleTableColumn(properties.Select(x=>x.Property.GetValue(configJson)?.ToString()))
                    { Foreground = ConsoleColor.Green, Width = 2f }
                });
            }

            if (parameters.Contains(nameof(Install)) && parameters.Contains(nameof(UnInstall)))
                throw new InvalidOperationException(Locale.Current["exception-0"]);

            if (parameters.Contains(nameof(Install)))
                new TInstaller().Install();
            else if (parameters.Contains(nameof(UnInstall)))
                new TInstaller().Uninstall();
        }
    }
}
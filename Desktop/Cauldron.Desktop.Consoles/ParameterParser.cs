using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cauldron.Consoles
{
    public sealed class ParameterParser
    {
        internal const char ParameterKey = '-';
        private List<ExecutionGroupProperties> executionGroups;
        private bool isInitialized = false;

        private Locale locale;

        public ParameterParser()
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
                this.locale = Factory.Create<Locale>();
        }

        public void Execute()
        {
            if (!this.isInitialized)
                throw new Exception("Execute Parse(object, string[]) first before invoking Execute()");

            var activatedGroups = this.executionGroups.Where(x => x.ExecutionGroup.CanExecute).OrderBy(x => x.Attribute.GroupIndex);

            foreach (var groups in activatedGroups)
                groups.ExecutionGroup.Execute(this);
        }

        public void Parse(object obj, string[] args)
        {
            var type = obj.GetType();
            var executionGroups = type.GetPropertiesEx(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType.ImplementsInterface<IExecutionGroup>())
                .Select(x => new ExecutionGroupProperties
                {
                    Attribute = x.PropertyType.GetCustomAttribute<ExecutionGroupAttribute>(),
                    ExecutionGroup = System.Activator.CreateInstance(x.PropertyType) as IExecutionGroup
                })
                .Where(x => x.Attribute != null);

            this.executionGroups = executionGroups.ToList();
            ParseGroups(this.executionGroups);
            var flatList = this.executionGroups.SelectMany(x => x.Parameters);

            // Search for dupletts and throw an exception if there is one...
            // Let the programer suffer
            var doubles = flatList
                .SelectMany(x => x.Parameters)
                .GroupBy(x => x)
                .Where(x => x.Skip(1).Any())
                .Select(x => x.Key);
            if (doubles.Any())
                throw new Exception("ParameterParser has found duplicate parameters in your parameter list. Please make sure that there are no doubles. " + string.Join(", ", doubles));

            this.isInitialized = true;

            try
            {
                TryParseParameters(flatList, args);
                // Try to find out which groups were activated and check if the isrequired parameters are set
                var activatedGroups = this.executionGroups.Where(x => x.ExecutionGroup.CanExecute);
                var requiredParameters = activatedGroups.SelectMany(x => x.Parameters.Where(y => y.Attribute.IsRequired && y.PropertyInfo.GetValue(x.ExecutionGroup) == null));

                if (requiredParameters.Any())
                    throw new RequiredParametersMissingException("Unable to continue. Required parameters are not set.", requiredParameters.Select(x => x.Parameters[0]).ToArray());
            }
            catch
            {
                ShowHelp();
                throw;
            }
        }

        public void ShowHelp()
        {
            if (!this.isInitialized)
                throw new Exception("Execute ParameterParser.Parse(object, string[]) first before invoking ParameterParser.ShowHelp()");

            var hasSource = Factory.HasContract(typeof(ILocalizationSource));

            ConsoleUtils.WriteTable(new ConsoleTableColumn[]
            {
                new ConsoleTableColumn(
                    hasSource?  this.locale["application-name"] : "Application name:",
                    hasSource?  this.locale["version"] : "Version:",
                    hasSource?  this.locale["description"] : "Description:",
                    hasSource?  this.locale["product-name"] : "Product name:",
                    hasSource?  this.locale["publisher"] : "Publisher:") { Foreground = ConsoleColor.Gray },
                new ConsoleTableColumn(
                    ApplicationInfo.ApplicationName,
                    ApplicationInfo.ApplicationVersion.ToString(),
                    ApplicationInfo.Description,
                    ApplicationInfo.ProductName,
                    ApplicationInfo.ApplicationPublisher) { Foreground = ConsoleColor.White }
            });

            Console.Write("\n");

            var assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            foreach (var group in this.executionGroups.OrderBy(x => x.Attribute.GroupIndex))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((hasSource ? this.locale[group.Attribute.GroupName] : group.Attribute.GroupName).PadRight(Console.WindowWidth - 1, '…'));
                Console.ForegroundColor = ConsoleColor.DarkGray;

                if (hasSource)
                    Console.WriteLine(this.locale["usage-example"] + ": " + Path.GetFileName(assembly.Location) + " " + group.Attribute.UsageExample);
                else
                    Console.WriteLine("Usage example: " + Path.GetFileName(assembly.Location) + " " + group.Attribute.UsageExample);

                ConsoleUtils.WriteTable(new ConsoleTableColumn[]
                {
                    new ConsoleTableColumn(group.Parameters.Select(x=> string.Join(", ", x.Parameters)).ToArray()) { Foreground = ConsoleColor.Gray },
                    new ConsoleTableColumn(group.Parameters.Select(x=> hasSource? this.locale[x.Attribute.Description] :  x.Attribute.Description).ToArray()) { Foreground = ConsoleColor.White }
                });

                Console.Write("\n");
            }
        }

        private static void ParseGroups(IEnumerable<ExecutionGroupProperties> executionGroups)
        {
            foreach (var group in executionGroups)
            {
                var type = group.ExecutionGroup.GetType();
                var parameters = type.GetPropertiesEx(BindingFlags.Public | BindingFlags.Instance)
                    .Select(x => new { Property = x, Attrib = x.GetCustomAttribute<OptionAttribute>() })
                    .Where(x => x.Attrib != null)
                    .Select(x => new ExecutionGroupParameter(group.ExecutionGroup, x.Property, x.Attrib));

                group.Parameters = parameters.ToList();
            }
        }

        private static void TryParseParameters(IEnumerable<ExecutionGroupParameter> executionGroupParameters, string[] args)
        {
            var pairs = new Dictionary<ExecutionGroupParameter, List<string>>();
            var currentList = new List<string>();

            // Add default option if we have one
            var defaultParameter = executionGroupParameters.FirstOrDefault(x =>
                    x.Parameters.Any(y => y.Length == 0)
                    /* Empty parameter is the default parameter */);

            if (defaultParameter != null)
                pairs.Add(defaultParameter, currentList);
            else // Just ignore default param
                currentList = null;

            foreach (var argument in args)
            {
                if (argument.Length == 0)
                    continue;

                if (argument[0] == ParameterKey)
                {
                    var match = executionGroupParameters.FirstOrDefault(x => x.Parameters.Any(y => y == argument));

                    if (match == null)
                        throw new UnknownParameterException("Unknown parameter", argument);

                    currentList = new List<string>();
                    pairs.Add(match, currentList);

                    continue;
                }

                if (currentList != null)
                    currentList.Add(argument);
            }

            // assign the values
            foreach (var pair in pairs)
            {
                pair.Key.ExecutionGroup.CanExecute = true;

                if (pair.Key.PropertyInfo.PropertyType.IsArray)
                {
                    var childType = pair.Key.PropertyInfo.PropertyType.GetChildrenType();
                    pair.Key.PropertyInfo.SetValue(pair.Key.ExecutionGroup, pair.Value.Select(x => x.Convert(childType)).ToArray());
                }
                else if (pair.Key.PropertyInfo.PropertyType == typeof(bool) ||
                   (pair.Key.PropertyInfo.PropertyType.IsNullable() && Nullable.GetUnderlyingType(pair.Key.PropertyInfo.PropertyType) == typeof(bool)))
                    pair.Key.PropertyInfo.SetValue(pair.Key.ExecutionGroup, true);
                else
                    pair.Key.PropertyInfo.SetValue(pair.Key.ExecutionGroup, string.Join(" ", pair.Value).Convert(pair.Key.PropertyInfo.PropertyType));
            }
        }
    }
}
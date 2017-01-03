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
    /// <summary>
    /// Parses the parameters passed to the application
    /// </summary>
    public sealed class ParameterParser
    {
        internal const char ParameterKey = '-';
        private Dictionary<IExecutionGroup, List<PropertyInfo>> activatedParameters = new Dictionary<IExecutionGroup, List<PropertyInfo>>();
        private List<ExecutionGroupProperties> executionGroups;
        private bool isInitialized = false;

        private Locale locale;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterParser"/> class
        /// </summary>
        /// <param name="executionGroups">The execution groups to parse to</param>
        public ParameterParser(params IExecutionGroup[] executionGroups)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
                this.locale = Factory.Create<Locale>();

            this.executionGroups = executionGroups
                .Select(x => new ExecutionGroupProperties
                {
                    Attribute = x.GetType().GetCustomAttribute<ExecutionGroupAttribute>(),
                    ExecutionGroup = x
                })
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.GroupIndex)
                .ToList();
        }

        /// <summary>
        /// Gets or sets the <see cref="Console.ForegroundColor"/> of the description in the help text
        /// </summary>
        public ConsoleColor DescriptionColor { get; set; } = ConsoleColor.White;

        /// <summary>
        /// Gets or sets the <see cref="Console.ForegroundColor"/> of the group name in the help text
        /// </summary>
        public ConsoleColor GroupColor { get; set; } = ConsoleColor.White;

        /// <summary>
        /// Gets or sets the <see cref="Console.ForegroundColor"/> of the key in the help text
        /// </summary>
        public ConsoleColor KeyColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets the parameters passed to the application
        /// </summary>
        public string[] Parameters { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Console.ForegroundColor"/> of the usage example text in the help text
        /// </summary>
        public ConsoleColor UsageExampleColor { get; set; } = ConsoleColor.DarkGray;

        /// <summary>
        /// Starts the execution of the execution groups
        /// </summary>
        /// <returns>true if any of the execution group was executed; otherwise false</returns>
        public bool Execute()
        {
            if (!this.isInitialized)
                throw new Exception("Execute Parse(object, string[]) first before invoking Execute()");

            if (this.activatedParameters.Count == 0)
                return false;

            var executableGroups = this.activatedParameters.Keys
                .Select(x => new
                {
                    Attrib = this.executionGroups.FirstOrDefault(y => y.ExecutionGroup == x).Attribute,
                    Group = x
                })
                .OrderBy(x => x.Attrib.GroupIndex);

            foreach (var groups in executableGroups)
            {
                try
                {
                    groups.Group.Execute(this);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the property names of all active parameters
        /// </summary>
        /// <param name="executionGroup">The execution group</param>
        /// <returns>A list of property names of all active parameters</returns>
        public IList<string> GetActiveParameters(IExecutionGroup executionGroup) => this.activatedParameters[executionGroup].Select(x => x.Name).ToList();

        /// <summary>
        /// Starts the parsing of the arguments
        /// </summary>
        /// <param name="args">A list of arguments that was passed to the application</param>
        public void Parse(string[] args)
        {
            if (this.isInitialized)
                throw new Exception("You cannot use the same instance of ParameterParser to parse another group of parameters. Please create a new instance.");

            this.Parameters = args;

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
                throw new Exception("ParameterParser has found duplicate parameters in your parameter list. Please make sure that there are no doublets. " + doubles.Join(", "));

            this.isInitialized = true;

            try
            {
                this.TryParseParameters(flatList, args);
                // Try to find out which groups were activated
                var activatedGroups = this.executionGroups.Where(x => this.activatedParameters.ContainsKey(x.ExecutionGroup));

                // check if the isrequired parameters are set
                var requiredParameters = activatedGroups.SelectMany(x => x.Parameters.Where(y => y.Attribute.IsRequired && y.PropertyInfo.GetValue(x.ExecutionGroup) == null));
                if (requiredParameters.Any())
                    throw new RequiredParametersMissingException("Unable to continue. Required parameters are not set.", requiredParameters.Select(x => x.Parameters.RandomPick()).ToArray());

                // check if parameters with non optional values are set
                var nonOptionalValues = activatedGroups.SelectMany(x => x.Parameters.Where(y => this.activatedParameters[x.ExecutionGroup].Contains(y.PropertyInfo) && !y.Attribute.ValueOptional && y.PropertyInfo.GetValue(y.ExecutionGroup) == null));
                if (nonOptionalValues.Any())
                    throw new RequiredValuesMissingException("Unable to continue. Parameters with non optional values have no values.", requiredParameters.Select(x => x.Parameters.RandomPick()).ToArray());
            }
            catch
            {
                this.ShowHelp();
                throw;
            }
        }

        /// <summary>
        /// Shows the help page of the application
        /// </summary>
        public void ShowHelp()
        {
            if (!this.isInitialized)
                throw new Exception("Execute ParameterParser.Parse(object, string[]) first before invoking ParameterParser.ShowHelp()");

            var hasSource = Factory.HasContract(typeof(ILocalizationSource));

            Console.Write("\n\n");

            // Write the application info
            ConsoleUtils.WriteTable(new ConsoleTableColumn[]
            {
                new ConsoleTableColumn(
                    hasSource?  this.locale["application-name"] : "APPLICATION NAME:",
                    hasSource?  this.locale["version"] : "VERSION:",
                    hasSource?  this.locale["description"] : "DESCRIPTION:",
                    hasSource?  this.locale["product-name"] : "PRODUCT NAME:",
                    hasSource?  this.locale["publisher"] : "PUBLISHER:") { Foreground = this.KeyColor },
                new ConsoleTableColumn(
                    ApplicationInfo.ApplicationName,
                    ApplicationInfo.ApplicationVersion.ToString(),
                    ApplicationInfo.Description,
                    ApplicationInfo.ProductName,
                    ApplicationInfo.ApplicationPublisher) { Foreground = this.DescriptionColor, Width = 2 }
            });

            Console.Write("\n\n");

            foreach (var group in this.executionGroups)
            {
                // Write the group name and divider
                Console.ForegroundColor = this.GroupColor;
                Console.WriteLine((hasSource ? this.locale[group.Attribute.GroupName] : group.Attribute.GroupName).PadRight(Console.WindowWidth - 1, '.'));

                var usageExampleText = hasSource ? $"{this.locale["usage-example"]}: " : "Usage example: ";

                // Write the usage example if there is one
                if (!string.IsNullOrEmpty(group.Attribute.UsageExample))
                {
                    Console.ForegroundColor = this.UsageExampleColor;
                    Console.WriteLine(usageExampleText + Path.GetFileNameWithoutExtension(GetStartingAssembly().Location) + " " + group.Attribute.UsageExample);
                }

                // Write the parameter - description table
                ConsoleUtils.WriteTable(new ConsoleTableColumn[]
                {
                    new ConsoleTableColumn(group.Parameters.Select(x=> x.Parameters.Where(y => !string.IsNullOrEmpty(y)).Join(", "))) { Foreground = this.KeyColor },
                    new ConsoleTableColumn(group.Parameters.Select(x=>
                    {
                        var description = hasSource? this.locale[x.Attribute.Description] : x.Attribute.Description;
                        if(x.Attribute.IsRequired)
                            return ReplaceKeywords( description + "\n!!" + (hasSource? this.locale["mandatory"] : "Mandatory"), x.Parameters.Last(), usageExampleText);

                        return ReplaceKeywords(description, x.Parameters.Last(), usageExampleText);
                    })) { Foreground = this.DescriptionColor, AlternativeForeground = this.UsageExampleColor, Width = 2 }
                });

                Console.Write("\n");
            }

            Console.ResetColor();
        }

        private static Assembly GetStartingAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            return assembly;
        }

        private static void ParseGroups(IEnumerable<ExecutionGroupProperties> executionGroups)
        {
            foreach (var group in executionGroups)
            {
                var type = group.ExecutionGroup.GetType();
                var parameters = type.GetPropertiesEx(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    .Select(x => new { Property = x, Attrib = x.GetCustomAttribute<ParameterAttribute>() })
                    .Where(x => x.Attrib != null)
                    .Select(x => new ExecutionGroupParameter(group.ExecutionGroup, x.Property, x.Attrib));

                group.Parameters = parameters.ToList();
            }
        }

        private static string ReplaceKeywords(string value, string parameter, string usageExampleText) => value
            .Replace("$ux$", $"\n!!{usageExampleText} {Path.GetFileNameWithoutExtension(GetStartingAssembly().Location)} {parameter}")
            .Replace("$mm$", $"\n!!{Path.GetFileNameWithoutExtension(GetStartingAssembly().Location)} {parameter}")
            .Replace("$me$", Path.GetFileNameWithoutExtension(GetStartingAssembly().Location))
            .Replace("$pm$", parameter);

        private void TryParseParameters(IEnumerable<ExecutionGroupParameter> executionGroupParameters, string[] args)
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

                    if (pairs.ContainsKey(match))
                    {
                        currentList = pairs[match];
                        continue;
                    }

                    currentList = new List<string>();
                    pairs.Add(match, currentList);

                    continue;
                }

                if (currentList != null)
                    currentList.Add(argument);
            }

            // Remove the default parameter execution of we have other stuff in the queue
            if (pairs.Count > 1 && defaultParameter != null)
                pairs.Remove(defaultParameter);

            // assign the values
            foreach (var pair in pairs)
            {
                var executionGroupName = pair.Key.ExecutionGroup;
                if (this.activatedParameters.ContainsKey(executionGroupName))
                    this.activatedParameters[executionGroupName].Add(pair.Key.PropertyInfo);
                else
                    this.activatedParameters.Add(executionGroupName, new List<PropertyInfo> { pair.Key.PropertyInfo });

                // TODO - Add Custom converters
                // TODO - Add List, Collection and IEnumerable converters
                if (pair.Key.PropertyInfo.PropertyType.IsArray)
                {
                    var childType = pair.Key.PropertyInfo.PropertyType.GetChildrenType();
                    pair.Key.PropertyInfo.SetValue(pair.Key.ExecutionGroup, pair.Value.Select(x => x.Convert(childType)).ToArray(childType));
                }
                else if (pair.Key.PropertyInfo.PropertyType == typeof(bool))
                    pair.Key.PropertyInfo.SetValue(pair.Key.ExecutionGroup, true);
                else
                    pair.Key.PropertyInfo.SetValue(pair.Key.ExecutionGroup, pair.Value.Join(" ").Convert(pair.Key.PropertyInfo.PropertyType));
            }
        }
    }
}
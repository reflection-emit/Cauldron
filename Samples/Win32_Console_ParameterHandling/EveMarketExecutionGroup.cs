using Cauldron.Consoles;
using Cauldron.Localization;
using System;
using System.Collections.Generic;
using Cauldron;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Win32_Console_ParameterHandling
{
    [ExecutionGroup("eveMarket", "-s typhoon --id --price", 1)]
    public sealed class EveMarketExecutionGroup : IExecutionGroup
    {
        [Parameter("get-id-of", "id", "I")]
        public bool ItemIdOf { get; private set; }

        [Parameter("get-price", "price", "p")]
        public bool PricesOf { get; private set; }

        [Parameter("searchString", false, true, "s")]
        public string SearchString { get; private set; }

        [Parameter("use-regex", "r")]
        public bool UseRegex { get; private set; }

        public void Execute(ParameterParser parser)
        {
            var parameters = parser.GetActiveParameters(this);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Locale.Current["please-wait"]);
            Console.ForegroundColor = ConsoleColor.Cyan;

            var itemTypes = EveApi.GetPrices().Where(x =>
            {
                if (this.UseRegex)
                    return Regex.IsMatch(x.Type.Name, this.SearchString);
                else
                    return x.Type.Name.Contains(this.SearchString, StringComparison.InvariantCultureIgnoreCase);
            }).ToArray();
            var consoleColumns = new List<ConsoleTableColumn>();

            if (ItemIdOf)
                consoleColumns.Add(new ConsoleTableColumn(itemTypes.Select(x => x.Type.Id.ToString("00000"))) { Width = 0.5f });

            consoleColumns.Add(new ConsoleTableColumn(itemTypes.Select(x => x.Type.Name)) { Width = 2 });
            consoleColumns.Add(new ConsoleTableColumn(itemTypes.Select(x => x.AdjustedPrice.ToString("C3", CultureInfo.CreateSpecificCulture("is-IS")))) { Alignment = ColumnAlignment.Right });

            if (PricesOf)
                consoleColumns.Add(new ConsoleTableColumn(itemTypes.Select(x => x.AveragePrice.ToString("C3", CultureInfo.CreateSpecificCulture("is-IS")))) { Alignment = ColumnAlignment.Right });

            ConsoleUtils.WriteTable(consoleColumns.ToArray());
        }
    }
}
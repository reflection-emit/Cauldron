using Cauldron.Consoles;
using Cauldron.Core.Extensions;
using Cauldron.Localization;
using EveOnlineApi;
using EveOnlineApi.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleConsoleApplication
{
    [ExecutionGroup("eveMarket", "-I --p typhoon", 1)]
    public sealed class EveMarketExecutionGroup : IExecutionGroup
    {
        [Parameter("get-id-of", "id", "I")]
        public string ItemIdOf { get; private set; }

        [Parameter("get-price", "price", "p")]
        public bool PricesOf { get; private set; }

        public void Execute(ParameterParser parser)
        {
            var parameters = parser.GetActiveParameters(this);

            Console.WriteLine(Locale.Current["please-wait"]);

            Api.Current.StaticData.UpdateStaticDataAsync().RunSync();
            Api.Current.CachePriceAsync().RunSync();

            var itemTypes = new Func<string, IEnumerable<KeyValuePair<long, ItemType>>>(searchString =>
                Api.Current.StaticData.ItemTypes
                    .Where(x => x.Value.IsPublished &&
                    (x.Value.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Value.GroupName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)))
                    .OrderBy(x => x.Value.Name)
                    .ThenBy(x => x.Value.GroupName)
                    .ThenBy(x => x.Value.Id)
                    .Take(20));

            if (parameters.Contains(nameof(ItemIdOf)))
            {
                if (string.IsNullOrEmpty(this.ItemIdOf))
                    throw new InvalidOperationException("Invalid value");

                var items = itemTypes(this.ItemIdOf);

                if (this.PricesOf)
                {
                    ConsoleUtils.WriteTable(new ConsoleTableColumn[]
                    {
                        new ConsoleTableColumn(items.Select(x=>x.Value.Id.ToString())),
                        new ConsoleTableColumn(items.Select(x=>x.Value.Name)) { Width = 4 },
                        new ConsoleTableColumn(items.Select(x=> Locale.Current[Api.Current.GetItemAveragePrice(x.Value.Id)] + " ISK")) { Width = 2, Alignment= ColumnAlignment.Right }
                });
                }
                else
                    ConsoleUtils.WriteTable(new ConsoleTableColumn[]
                    {
                        new ConsoleTableColumn(items.Select(x=>x.Value.Id.ToString())),
                        new ConsoleTableColumn(items.Select(x=>x.Value.Name)) { Width = 4 },
                        new ConsoleTableColumn(items.Select(x=> x.Value.Group.Name)) { Width = 2 }
                    });
            }
        }
    }
}
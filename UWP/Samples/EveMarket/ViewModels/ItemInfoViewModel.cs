using Cauldron.Activator;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using EveMarket.Views;
using EveOnlineApi;
using EveOnlineApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EveMarket.ViewModels
{
    [View(typeof(ItemInfoView))]
    public class ItemInfoViewModel : ViewModelBase, IFactoryInitializeComponent
    {
        [Inject]
        private IEveApi eveApi = null;

        [ComponentConstructor]
        public ItemInfoViewModel(long itemId)
        {
            this.ItemId = itemId;
        }

        public IEnumerable<QuicklookOrder> BuyOrders { get; set; }

        public string Description { get; set; }

        public string GroupName { get; set; }

        public BitmapImageEx Icon { get; private set; }

        public long ItemId { get; set; }

        public string MarketGroupName { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public IEnumerable<QuicklookOrder> SellOrders { get; set; }

        public async Task OnInitializeComponentAsync()
        {
            await this.RunAsync(async () =>
            {
                var item = this.eveApi.StaticData.GetItemType(this.ItemId);

                this.Description = EveUtils.XAMLify(item.Description);
                this.Name = item.Name;
                this.GroupName = item.GroupName;
                this.MarketGroupName = item.MarketGroup?.Name;

                this.Icon = await (await this.eveApi.GetImageAsync(ImageType.Item, this.ItemId, 1, 128)).ToBitmapImageAsync();

                var marketQuicklook = await this.eveApi.GetMarketOrdersAsync(this.ItemId);
                this.Price = this.eveApi.GetItemPrice(this.ItemId).AdjustedPrice;
                this.BuyOrders = marketQuicklook.BuyOrders.Where(x => x.Security > 0.4).OrderByDescending(x => x.Price).Take(10);
                this.SellOrders = marketQuicklook.SellOrders.Where(x => x.Security > 0.4).OrderBy(x => x.Price).Take(10);
            });
        }
    }
}
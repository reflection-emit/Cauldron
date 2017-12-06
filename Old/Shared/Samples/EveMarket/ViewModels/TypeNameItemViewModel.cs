using Cauldron.Activator;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using EveOnlineApi;

namespace EveMarket.ViewModels
{
    public sealed class TypeNameItemViewModel : ViewModelBase
    {
        [Inject]
        private IEveApi eveApi = null;

        public TypeNameItemViewModel(long itemId, string name)
        {
            this.Name = name;
            this.ItemId = itemId;

            this.InitializeComponent();
        }

        public double AveragePrice { get; private set; }

        public string GroupName { get; private set; }

        public BitmapImageEx Icon { get; private set; }

        public long ItemId { get; private set; }

        public string Name { get; private set; }

        private async void InitializeComponent()
        {
            await this.RunDispatcherAsync(async () =>
            {
                this.GroupName = this.eveApi.StaticData.GetGroupNameFromTypeId(this.ItemId);
                this.AveragePrice = this.eveApi.GetItemAveragePrice(this.ItemId);
                this.Icon = await (await eveApi.GetImageAsync(ImageType.Item, this.ItemId, 1, 64)).ToBitmapImageAsync();
            });
        }
    }
}
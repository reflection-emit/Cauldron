using Cauldron.Activator;
using System.Threading.Tasks;

namespace EveOnlineApi.Models
{
    public sealed class Location
    {
        public Location(long locationId)
        {
            this.Id = locationId;
            this.IsConquerableOutpost = (locationId > 61000000 && locationId < 66000000) || (locationId >= 60014861 && locationId <= 60014928);
            this.IsSolarSystem = locationId <= 60000000;
            this.IsNpcStation = locationId > 60000000 && locationId <= 61000000;
        }

        public long Id { get; private set; }

        public bool IsConquerableOutpost { get; private set; }

        public bool IsNpcStation { get; private set; }

        public bool IsSolarSystem { get; private set; }

        public long StationId
        {
            get
            {
                if (this.Id >= 66000000 && this.Id < 67000000)
                    return this.Id - 6000001;

                if (this.Id >= 67000000 && this.Id < 68000000)
                    return this.Id - 6000000;

                return this.Id;
            }
        }

        public async Task<string> GetLocationNameAsync(long id)
        {
            if (this.IsSolarSystem)
                return Api.Current.StaticData.GetSolarSystemName(this.Id);
            else if (this.IsNpcStation)
                return Api.Current.StaticData.GetNpcStationName(this.StationId);
            else if (this.IsConquerableOutpost)
                return (await Factory.Create<IEveApi>().GetConquerableStationAsync(this.StationId)).Name;
            else
                return string.Empty;
        }
    }
}
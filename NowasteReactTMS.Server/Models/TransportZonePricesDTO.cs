using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class TransportZonePricesDTO
    {
        public List<TransportZonePrice> Prices { get; set; }
        public List<TransportZone> TransportZones { get; set; }
        public List<TransportType> TransportTypes { get; set; }
        public List<Agent> Agents { get; set; }

        public Guid TransportZonePriceLinePK { get; set; }
        public Guid TransportZonePricePK { get; set; }
        public int PalletTypeId { get; set; }
        public PalletType PalletType { get; set; }
        public decimal Price { get; set; }
    }
}

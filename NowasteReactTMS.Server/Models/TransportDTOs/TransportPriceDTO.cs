using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.TransportDTOs
{
    public class TransportPriceDTO
    {
        public decimal Price { get; set; }
        public Guid CurrencyPK { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public Guid FromTransportZonePK { get; set; }
        public Guid ToTransportZonePK { get; set; }
        public Guid TransportTypePK { get; set; }
        public string Description { get; set; }
        public Guid TransportZonePricePK { get; set; }
        public Guid AgentPK { get; set; }
    }
}

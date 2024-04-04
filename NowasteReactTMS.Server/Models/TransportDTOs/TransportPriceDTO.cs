using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.TransportDTOs
{
    public class TransportPriceDTO
    {
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string Description { get; set; }
        public Guid TransportZonePricePK { get; set; }
        //public TransportZonePrice TransportZonePrice { get; set; }
    }
}

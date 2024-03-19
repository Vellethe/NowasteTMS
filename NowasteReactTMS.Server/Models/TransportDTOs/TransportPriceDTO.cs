using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.TransportDTOs
{
    public class TransportPriceDTO
    {
        public TransportZonePrice TransportZonePrice { get; set; }

        public Agent Agent { get; set; }
        public List<Currency> Currencies { get; set; }
        public PalletType PalletType { get; set; }
        public decimal Price { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class CreateTransportZonePriceDTO
    {
        public List<Agent> Agents { get; set; }
        public List<TransportZone> TransportZones { get; set; }
        public List<TransportType> TransportTypes { get; set; }
        public List<Currency> Currencies { get; set; }
        public TransportZonePrice TransportZonePrice { get; set; }
        public List<SelectListItem> PalletTypes { get; set; }
        public List<TransportPriceDTO> PalletPrices { get; set; }
        public List<TransportZonePricesDTO> Lines { get; set; }
    }

}

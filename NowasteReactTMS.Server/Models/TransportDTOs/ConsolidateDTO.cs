using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.TransportDTOs
{
    public class ConsolidateDTO
    {
        public List<TransportOrder> TransportOrders { get; set; }
        public List<Order> Orders { get; set; }
        public List<TransportOrderService> Services { get; set; }
        public decimal TotalFullTruckPrice { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public Guid CurrencyPK { get; set; }
        public ConsolidateServiceDTO ConsolidateServicesViewModel { get; set; }
        public bool PriceChanged { get; set; }
        public Guid AgentPK { get; set; }

        // Source of email addresses to select from.
        public List<SelectListItem> AgentEmailAddresses { get; set; }

        // Selected email addresses.
        public List<string> SelectedAgentEmailAddresses { get; set; }
    }
}

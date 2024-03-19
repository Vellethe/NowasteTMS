using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NowasteReactTMS.Server.Models.OrderDTOs
{
    public class OrderDTO
    {
        [Required]
        public string OrderId { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<OrderLineDTO> Lines { get; set; }

        public bool PalletExchange { get; set; }

        [Required]
        [DisplayName("Supplier")]
        public Guid SupplierPK { get; set; }

        [Required]
        [DisplayName("Customer")]
        public Guid CustomerPK { get; set; }

        public string HandlerID { get; set; }
        public string? Comment { get; set; }
        public string? InternalComment { get; set; }
        public bool TransportBooking { get; set; }
    }
}

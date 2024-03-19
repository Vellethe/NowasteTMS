using NowasteReactTMS.Server.Models.CustomerDTOs;
using NowasteReactTMS.Server.Models.TransportDTOs;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.OrderDTOs
{
    public class OrderViewDTO
    {
        public Guid Id { get; set; }
        public string OrderId { get; set; }
        public string Name { get; set; }
        public List<OrderLine> Lines { get; set; }
        public DateTime DeliveryDate { get; set; }

        public Agent Agent { get; set; }
        public SupplierViewDTO Supplier { get; set; }
        public SingleCustomerDTO Customer { get; set; }
        public TransportOrderDTO TransportOrder { get; set; }
    }
}

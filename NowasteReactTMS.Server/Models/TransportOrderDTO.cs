using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;
using System.ComponentModel.DataAnnotations;

namespace NowasteReactTMS.Server.Models
{
    public class TransportOrderDTO
    {
        public Guid Id { get; set; }
        public Guid OrderPK { get; set; }
        public string TransportOrderId { get; set; }
        public OrderOrigin Origin { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public string Comment { get; set; }
        public string InternalComment { get; set; }
        public bool Confirmed { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime OriginalDeliveryDate { get; set; }
        [MaxLength(50)]
        public string VehicleRegistrationPlate { get; set; }
        public Customer Customer { get; set; }
        public bool IsExisting { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int FilenameRevision { get; set; }
        public int PalletQty { get; set; }
        public Agent Agent { get; set; }
        public List<TransportOrderLineDTO> Lines { get; set; }
        public List<TransportOrderServiceDTO> Services { get; set; }
        public List<SelectListItem> CustomerAddresses { get; set; }
        public Guid? SelectedCustomerContactInformation { get; set; }
        public Guid? SelectedSupplierContactInformation { get; set; }
        public bool TransportType { get; set; }
        public string OrderIds { get; set; }
        public List<Guid> OrderPks { get; set; }
        public bool IsConsolidation { get; set; }
    }
}

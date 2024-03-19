using NowasteReactTMS.Server.Models.CustomerDTOs;

namespace NowasteReactTMS.Server.Models
{
    public class SupplierDTO
    {
        public Guid SupplierPK { get; set; }

        public string SupplierID { get; set; }

        public Guid BusinessUnitPK { get; set; }

        public CustomerInfo BusinessUnit { get; set; }
    }
}

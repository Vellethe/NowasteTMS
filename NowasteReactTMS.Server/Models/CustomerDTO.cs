namespace NowasteReactTMS.Server.Models
{
    public class CustomerDTO
    {
        public Guid CustomerPK { get; set; }

        public string CustomerID { get; set; }

        public Guid BusinessUnitPK { get; set; }

        public CustomerInfo BusinessUnit { get; set; }
    }
}

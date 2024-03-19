namespace NowasteReactTMS.Server.Models.CustomerDTOs
{
    public class CustomerDTO
    {
        public Guid CustomerPK { get; set; }

        public string CustomerID { get; set; }

        public Guid BusinessUnitPK { get; set; }

        public CustomerInfo BusinessUnit { get; set; }
    }
}

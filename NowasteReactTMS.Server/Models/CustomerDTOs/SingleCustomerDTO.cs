namespace NowasteReactTMS.Server.Models.CustomerDTOs
{
    public class SingleCustomerDTO
    {
        public Guid CustomerPK { get; set; }

        public string CustomerID { get; set; }

        public Guid BusinessUnitPK { get; set; }

        public BusinessUnitDTO BusinessUnit { get; set; }
    }
}

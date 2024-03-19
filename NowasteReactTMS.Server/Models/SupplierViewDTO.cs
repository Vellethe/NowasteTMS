namespace NowasteReactTMS.Server.Models
{
    public class SupplierViewDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public BusinessUnitDTO BusinessUnit { get; set; }
    }
}

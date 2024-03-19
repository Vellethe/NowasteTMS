using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class TransportOrderLineDTO
    {
        public Guid Id { get; set; }
        public int LineNumber { get; set; }
        public ContactInformation FromContactInformation { get; set; }
        public ContactInformation ToContactInformation { get; set; }
        public decimal Price { get; set; }
    }
}

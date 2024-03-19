using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class TransportOrderServiceDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
    }
}

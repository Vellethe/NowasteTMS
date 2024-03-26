using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class ServiceDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Guid CurrencyPK { get; set; }
        public Guid TransportOrderServicePK { get; set; }
        public Guid AgentPK { get; set; }
    }
}

using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class ServiceDTO
    {
        public TransportOrderService TransportOrderService { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<Agent> Agents { get; set; }
    }
}

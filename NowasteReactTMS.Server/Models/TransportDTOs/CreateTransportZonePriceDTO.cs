using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.TransportDTOs
{
    public class CreateTransportZonePriceDTO
    {
        //public List<Agent> Agents { get; set; }
        //public Guid AgentPK { get; set; }
        //public string AgentID { get; set; }
        //public Guid BusinessUnitPK { get; set; }
        public Guid BusinessUnitPK { get; set; }
        public TransportPriceDTO TransportZonePrice { get; set; }
    }

}

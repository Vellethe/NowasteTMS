using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models.AgentDTOs
{
    public class AgentDTO
    {
        public Guid AgentPK { get; set; }
        public string AgentID { get; set; }
        public bool IsSelfBilling { get; set; }
        public bool IsActive { get; set; } = true;
        public BusinessUnit BusinessUnit { get; set; }
    }
}

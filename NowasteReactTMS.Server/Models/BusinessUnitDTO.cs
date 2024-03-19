using NowasteTms.Model;
using System.ComponentModel.DataAnnotations;

namespace NowasteReactTMS.Server.Models
{
    public class BusinessUnitDTO
    {
        public Guid BusinessUnitPK { get; set; }
        public string Name { get; set; }
        public Guid FinanceInformationPK { get; set; }
        public FinanceInformation FinanceInformation { get; set; }
        public List<ContactInformation> ContactInformations { get; set; }
        public Guid ReferencePK { get; set; }
        public Reference Reference { get; set; }
        public string Company { get; set; }
        public bool IsEditable { get; set; }
    }
}

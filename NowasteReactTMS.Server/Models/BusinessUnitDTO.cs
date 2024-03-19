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
    public class ContactInformation
    {
        [Key]
        public Guid ContactInformationPK { get; set; }
        public Guid BusinessUnitPK { get; set; }
        public bool IsDefault { get; set; }
        public Guid? ReferencePK { get; set; }
        public List<Reference> References { get; set; }
        public List<TransportZone> TransportZones { get; set; }
        public string Phone { get; set; }
        public string CellularPhone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ExternalId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public bool IsEditable { get; set; }
    }
}

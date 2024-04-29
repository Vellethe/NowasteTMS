using NowasteTms.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NowasteReactTMS.Server.Models.CustomerDTOs
{
    public class CustomerInfo
    {
        public Guid BusinessUnitPK { get; set; }
        public string Name { get; set; }
        public Guid FinanceInformationPK { get; set; }
        public CustomerFinanceInformation FinanceInformation { get; set; }
        public List<ContactInformation> ContactInformations { get; set; }
        public string Company { get; set; }
        public bool IsEditable { get; set; }

    }
    public class CustomerFinanceInformation
    {
        public string VAT { get; set; }
        [DisplayName("Currency")]
        public Currency Currency { get; set; }
        [DisplayName("Self billing settings")]
        public List<Currency> Currencies { get; set; }
    }
}
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Models
{
    public class TransportViewDTO
    {
        public Guid SelectedTransportOrderId { get; set; }
        public DateTime SelectedArrivalDate { get; set; }
        public string CheckedTransportOrderPKs { get; set; }
        public List<Currency> Currencies { get; set; }
        public bool historicalView { get; set; }
    }
}
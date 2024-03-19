namespace NowasteReactTMS.Server.Models
{
    public class ConsolidateServiceDTO
    {
        public List<TransportOrderServiceDTO> UnselectedServices { get; set; }
        public List<TransportOrderServiceDTO> SelectedServices { get; set; }
    }
}

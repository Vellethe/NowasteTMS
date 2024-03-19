namespace NowasteReactTMS.Server.Models.TransportDTOs
{
    public class ConsolidateServiceDTO
    {
        public List<TransportOrderServiceDTO> UnselectedServices { get; set; }
        public List<TransportOrderServiceDTO> SelectedServices { get; set; }
    }
}

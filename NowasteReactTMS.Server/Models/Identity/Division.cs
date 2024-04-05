namespace NowastePalletPortal.Models
{
    public class Division
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
    }
}

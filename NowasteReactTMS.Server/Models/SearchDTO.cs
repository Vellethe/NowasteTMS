namespace NowasteReactTMS.Server.Models
{
    public class SearchDTO
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public Dictionary<string, string> Filter { get; set; }
        public Dictionary<string, bool> Column { get; set; }
    }
}

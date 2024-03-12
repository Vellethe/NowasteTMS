namespace NowasteReactTMS.Server.Models
{
    public class SearchOrderDTO
    {
       public int Size { get; set; }
       public int Page {  get; set; }
       public Dictionary<string, string> Filter { get; set; }
       public Dictionary<string, bool> Column {  get; set; }
       public bool Historical { get; set; }
    }
}

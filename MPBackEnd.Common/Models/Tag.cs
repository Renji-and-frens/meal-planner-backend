namespace MPBackEnd.Common.Models
{
    public class Tag : 
                 BaseModel<string>
    {
        public string Name        { get; set; }
        public string Description { get; set; }
        public List<string> UsedIn      { get; set; }
    }
}

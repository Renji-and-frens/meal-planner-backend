namespace MPBackEnd.Common.Models
{
    public class Ingredient :
                 BaseModel<string>
    {
        public string Name        { get; set; }
        public string Description { get; set; }
    }
}

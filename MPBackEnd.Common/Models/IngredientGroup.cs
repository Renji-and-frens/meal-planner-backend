namespace MPBackEnd.Common.Models
{
    public class IngredientGroup :
                 BaseModel<string>
    {
        public string Name        { get; set; }
        public string Description { get; set; }
    }
}

namespace MPBackEnd.Common.Models
{
    public class IngredientDetails :
                 BaseModel<string>
    {
        public string     Details        { get; set; }
        public Ingredient Ingredient     { get; set; }
        public Recipe     Recipe         { get; set; }
    }
}

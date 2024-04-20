namespace MPBackEnd.Common.Models
{
    public class Author :
                 BaseModel<string>
    {
        public string     Name { get; set; }
        public AuthorType AuthorType { get; set; }
    }
}

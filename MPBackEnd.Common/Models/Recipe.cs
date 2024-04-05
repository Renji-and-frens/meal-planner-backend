namespace MPBackEnd.Common.Models
{
    public class Recipe :
                 BaseModel<string>
    {
        public string   Name             { get; set; }
        public string   Description      { get; set; }
        public DateTime DateCreated      { get; set; }
        public DateTime DateLastModified { get; set; }
        public string   ThumbnailId      { get; set; }
        public string   AuthorId         { get; set; }
        public string   OriginId         { get; set; }
        public int      PrepTime         { get; set; }
        public int      CookTime         { get; set; }
        public string   Instructions     { get; set; }
        public int      LikeCount        { get; set; }
        public int      Difficulty       { get; set; }
        public bool     IsDeleted        { get; set; }
        public bool     IsArchived       { get; set; }
        public bool     IsVariant        { get; set; }
    }
}

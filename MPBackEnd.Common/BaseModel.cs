namespace MPBackEnd.Common
{
    public class BaseModel<T> :
                 Identifiable<T>
    {
        public T Id { get; set; }
    }
}

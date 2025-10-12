namespace myapi_minimals.infra.Models
{
    public class NewsLetter : BaseModel<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
    }
}

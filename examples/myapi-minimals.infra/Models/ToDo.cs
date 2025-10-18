namespace myapi_minimals.infra.Models
{
    public class ToDo : BaseModel<Guid>
    {
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; } = false;
        public DateOnly? Date { get; set; }
        public string? EntityName { get; set; }
        public string? GroupName { get; set; }
        public string? Tags { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}

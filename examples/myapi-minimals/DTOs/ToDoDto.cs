namespace myapi_minimals.DTOs
{
    public class ToDoDto
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; } = false;
        public DateOnly? Date { get; set; }
        public string? EntityName { get; set; }
        public string? GroupName { get; set; }
        public string? Tags { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}

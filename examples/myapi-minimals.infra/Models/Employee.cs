namespace myapi_minimals.infra.Models
{
    public class Employee : BaseModel<Guid>
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal? Salary { get; set; } = 0;
    }
}

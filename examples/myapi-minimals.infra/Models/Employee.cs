namespace myapi_minimals.infra.Models
{
    public class Employee : BaseModel<string>
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal? Salary { get; set; } = 0;
    }
}

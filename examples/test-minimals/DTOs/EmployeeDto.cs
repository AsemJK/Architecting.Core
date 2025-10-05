namespace test_minimals.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal? Salary { get; set; } = 0;
    }
}

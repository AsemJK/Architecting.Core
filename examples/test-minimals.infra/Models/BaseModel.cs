namespace test_minimals.infra.Models
{
    internal class BaseModel<T>
    {
        public T Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

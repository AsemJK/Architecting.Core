namespace myapi_minimals.infra.Models.Identity
{
    public class Role : BaseModel<Guid>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<User>? Users { get; set; }
    }
}

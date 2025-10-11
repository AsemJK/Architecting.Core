namespace test_minimals.infra.Models.Identity
{
    public class User : BaseModel<Guid>
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Role>? Roles { get; set; }
    }
}

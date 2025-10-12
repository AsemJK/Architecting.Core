namespace myapi_minimals.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Token { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}

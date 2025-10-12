namespace myapi_minimals.DTOs
{
    public class GeneralResponseDto
    {
        //apply response RFC 7807
        public int Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Instance { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public object? Data { get; set; }
    }
}

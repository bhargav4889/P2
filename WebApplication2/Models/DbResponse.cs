namespace WebApplication2.Models
{
    public class DbResponse
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public object? Data { get; set; }
    }
}

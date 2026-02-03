namespace EShop.Models.Response;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public List<string> Details { get; set; } = new List<string>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
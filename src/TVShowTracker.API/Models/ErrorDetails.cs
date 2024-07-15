using System.Text.Json;

namespace TVShowTracker.API.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public override string ToString() => JsonSerializer.Serialize(this);
}

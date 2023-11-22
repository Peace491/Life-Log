namespace DomainModels;

public class Response
{
    public bool HasError { get; set; } = true;

    public string? ErrorMessage { get; set; }= null;

    public ICollection<Object>? Output { get; set; } = null;

    public int RetryAttempts { get; set; } = 0;

    public bool IsSafeToRetry { get; set; } = false;

    public long LogId { get; set; } = 0;
}
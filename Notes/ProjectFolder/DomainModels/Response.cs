public class Response
{
    public Response()
    {
        HasError = true;
        //...
    }
    public bool HasError { get; set; } = true;

    public string? ErrorMessage { get; set; }= null;

    public ICollection<object>? Output { get; set; } = null;

    public int RetryAttempts { get; set; } = 0;

    public bool isSafeToRetry { get; set; } = false;
}
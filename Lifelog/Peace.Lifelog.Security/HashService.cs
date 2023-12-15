using DomainModels;

namespace Peace.Lifelog.Security;

public class HashService : IHasher
{
    public async Task<Response> Hasher(string plaintext)
    {
        var response = new Response();
        try
        {
            // TODO
            // hash operation will be async, could have high cost on system resources.
            return response;
        }
        catch
        {
            response.HasError = true;
            return response;
        }
    }

}

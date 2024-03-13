namespace Peace.Lifelog.RE;

using System.Diagnostics;
using DomainModels;
public class REManager : IGetRecs
{
    public async Task<Response> getRecs(string userhash)
    {
        throw new NotImplementedException();
    }
    
    // Helper Functions
    private int validateNumRecs(int numRecs)
    {
        int validNumRecs = 0;
        return validNumRecs;
    }
    private List<Object> validateLLI(List<Object> recs)
    {
        List<object> validLLI = new List<Object>();
        return validLLI;
    }
    private bool timeOperation(Stopwatch timer)
    {
        bool timeOp = false;
        return timeOp;
    }
}

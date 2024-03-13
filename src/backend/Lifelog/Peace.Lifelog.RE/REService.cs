namespace Peace.Lifelog.RE;

using System.Reflection.Metadata.Ecma335;
using DomainModels;
public class ReService : IGetNumRecs
{
    public async Task<Response> getNumRecs(string userhash, int numRecs)
    {
        throw new NotImplementedException();
    }

    // Helper Functions
    private Dictionary<string, int> scoreInit(Dictionary<string, int> scoreDict)
    {
        Dictionary<string, int> userScores = new Dictionary<string, int>();
        return userScores;
        
    }

    private Dictionary<string, int> scoreLLI(Dictionary<string, int> scoreDict)
    {
        Dictionary<string, int> userScores = new Dictionary<string, int>();
        return userScores;
        
    }

    private List<Object> pullRecs() // Params for helper need discussion
    {
        List<object> recs = new List<Object>();
        return recs;
    }


    private List<Object> cleanRecs(List<Object> recs)
    {
        List<object> cleanRecs = new List<Object>();
        return cleanRecs;
    }
}


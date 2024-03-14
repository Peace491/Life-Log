namespace Peace.Lifelog.RE;

using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using DomainModels;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Logging;
using ZstdSharp.Unsafe;

public class ReService : IGetNumRecs
{
    public async Task<Response> getNumRecs(string userhash, int numRecs)
    {
        var response = new Response();
        // var logger = new Logger();
        var readDataOnlyDAO = new ReadDataOnlyDAO();
        string hash = "System";
        string sqlGetUserFormData = $"SELECT Category, Rating FROM LifelogDB.UserForm WHERE UserHash=\"{hash}\" ORDER BY Rating ASC;";
        string sqlGetUserLLI = "";

        Dictionary<string, int> userScores = new Dictionary<string, int>();

        var formResponse = await readDataOnlyDAO.ReadData(sqlGetUserFormData);
        userScores = scoreInit(formResponse);
        response.Output = (ICollection<object>?)userScores;
        return response;
        
    }

    // Helper Functions

    // Need to deal with warning here.
    private Dictionary<string, int> scoreInit(Response formResponse)
    {
        Dictionary<string, int> userScores = new Dictionary<string, int>();
        foreach (List<Object> pair in formResponse.Output)
        {
            if (pair[0] == null)
            {
                break;
            }
            int rank = Convert.ToInt32(pair[1]);
            if (rank > 10)
            {
                userScores.Add(pair[0].ToString(), 0);
            }
            else
            {
                switch(rank)
                {
                    case 1: 
                        userScores.Add(pair[0].ToString(), 25);
                        break;
                    case 2:
                        userScores.Add(pair[0].ToString(), 18);
                        break;
                    case 3:
                        userScores.Add(pair[0].ToString(), 15);
                        break;
                    case 4:
                        userScores.Add(pair[0].ToString(), 12);
                        break;
                    case 5:
                        userScores.Add(pair[0].ToString(), 10);
                        break;
                    case 6:
                        userScores.Add(pair[0].ToString(), 8);
                        break;
                    case 7:
                        userScores.Add(pair[0].ToString(), 6);
                        break;
                    case 8:
                        userScores.Add(pair[0].ToString(), 4);
                        break;
                    case 9:
                        userScores.Add(pair[0].ToString(), 2);
                        break;
                    case 10:
                        userScores.Add(pair[0].ToString(), 1);
                        break;
                    default:
                        break;
                }
            } 
        }    
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
        List<object> cleanedRecs = new List<Object>();
        return cleanedRecs;
    }
}


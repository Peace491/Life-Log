namespace Peace.Lifelog.RE;

using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using DomainModels;
using Org.BouncyCastle.Asn1.Cmp;
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
        string sqlGetUserLLI = "SELECT Category, Status FROM LLI JOIN LLICategories ON LLI.LLIId = LLICategories.LLIId WHERE LLI.UserHash = 'System';"; // get lli where userhash = userhash, only get category and status
        string sqlCommonPublic = "SELECT LLICategories.Category, COUNT(*) as Count FROM LLI JOIN LLICategories ON LLI.LLIId = LLICategories.LLIId WHERE LLI.Visibility = 'public' GROUP BY LLICategories.Category ORDER BY Count DESC LIMIT 1;";

        Dictionary<string, int> userScores = new Dictionary<string, int>();
        Dictionary<string, int> userScoresLLI = new Dictionary<string, int>();
        try
        {
            var formResponse = await readDataOnlyDAO.ReadData(sqlGetUserFormData);
            userScores = scoreInit(formResponse);

            var userLLIResponse = await readDataOnlyDAO.ReadData(sqlGetUserLLI);
            userScoresLLI = scoreLLI(userScores, userLLIResponse);

            var commonCategoryResponse = await readDataOnlyDAO.ReadData(sqlCommonPublic);

            
            response.Output = (ICollection<object>?)userScores;
            return response;
        }
        catch (Exception e)
        {
            // logger.Log(e.Message);
            response.ErrorMessage = e.Message;
            response.Output = null;
            return response;
        }

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
            switch (rank)
            // Score categories using f1 scoring system
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
                    userScores.Add(pair[0].ToString(), 0);
                    break;
            }
        }
        return userScores;
    }

    private Dictionary<string, int> scoreLLI(Dictionary<string, int> scoreDict, Response lliResponse)
    {
        foreach (List<Object>lli in lliResponse.Output)
        {
            int statusMultiplier;
            
            switch (lli[1]) // index by whereever status is in the lli response
            {
                case "Postponed":
                    statusMultiplier = (int)0.8;
                    break;
                case "Active":
                    statusMultiplier = 1;
                    break;
                case "Completed":
                    statusMultiplier = (int)1.2;
                    break;
                default:
                    statusMultiplier = 1;
                    break;
            }
            string currentCategory = lli[0].ToString();
            if (scoreDict.ContainsKey(currentCategory)) // contains key category
            {
                scoreDict[currentCategory] += 5 * statusMultiplier;
            }
        }
        return scoreDict;

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


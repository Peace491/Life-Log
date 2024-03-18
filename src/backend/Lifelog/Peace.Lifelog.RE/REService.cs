﻿

using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using DomainModels;
using Org.BouncyCastle.Asn1.Cmp;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;
using ZstdSharp.Unsafe;

// should this be like this ?? im not understanding
namespace Peace.Lifelog.RE
{
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

                // temp
                var topTwoKeys = userScoresLLI.OrderByDescending(kvp => kvp.Value)
                                    .Take(2)
                                    .Select(kvp => kvp.Key)
                                    .ToList();
                                    

                var recomendtaionQuery = pullRecs(numRecs);
                var recomendationResponse = await readDataOnlyDAO.ReadData(recomendtaionQuery);

                response.Output = cleanRecs(recomendationResponse);

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

        private Dictionary<string, int> scoreInit(Response formResponse)
        {
            Dictionary<string, int> userScores = new Dictionary<string, int>();
            foreach (List<Object> pair in formResponse.Output)
            {
                if (pair[0] != null && pair[1] != null)
                {
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
            
        // Responsible for dynamically constructing the SQL query to pull the records from the database
        private string pullRecs(int numberRecomendations) // Params for helper need discussion
        {
            var recomendationResponse = new Response();

            // TODO : Add logic to pull records from the database
            string query = @"
                -- define stuff
                SET @Category1 = 'CategoryName1';
                SET @Category2 = 'CategoryName2';
                SET @Category3 = 'CategoryName3';
                SET @ExcludedUserHash = 'specificUserHash';

                -- get stuff
                -- ideally, this part is dynamic. so we concat subqueries based on num recs
                (SELECT LLI.LLIId, LLI.Title, LC.Category
                FROM LLI
                JOIN LLICategories LC ON LLI.LLIId = LC.LLIId
                WHERE LC.Category = @Category1
                AND LLI.UserHash != @ExcludedUserHash
                LIMIT 2)

                UNION ALL

                (SELECT LLI.LLIId, LLI.Title, LC.Category
                FROM LLI
                JOIN LLICategories LC ON LLI.LLIId = LC.LLIId
                WHERE LC.Category = @Category2
                AND LLI.UserHash != @ExcludedUserHash
                LIMIT 1)

                UNION ALL

                (SELECT LLI.LLIId, LLI.Title, LC.Category
                FROM LLI
                JOIN LLICategories LC ON LLI.LLIId = LC.LLIId
                WHERE LC.Category = @Category3
                AND LLI.UserHash != @ExcludedUserHash
                LIMIT 1)

                UNION ALL

                (SELECT LLI.LLIId, LLI.Title, LC.Category
                FROM LLI
                JOIN LLICategories LC ON LLI.LLIId = LC.LLIId
                WHERE LC.Category NOT IN (@Category1, @Category2, @Category3)
                AND LLI.UserHash != @ExcludedUserHash
                LIMIT 1);
            ";
            
            return query;
        }

        // Parse the records and clean them up
        // TODO : idetifiy personally identifiable info and "clean" it
        private List<Object> cleanRecs(Response recomendationResposne)
        {
            List<object> cleanedRecs = new List<Object>();
            foreach (List<Object> LLI in recomendationResposne.Output)
            {
                int index = 0;
                var lli = new LLI.LLI();
                
                foreach (var attribute in LLI) {
                    if (attribute is null) continue;
                    
                    switch(index){
                        case 0:
                            lli.LLIID = attribute.ToString() ?? "";
                            break;
                        case 1:
                            lli.UserHash = attribute.ToString() ?? "";
                            break;
                        case 2:
                            lli.Title = attribute.ToString() ?? "";
                            break;
                        case 3:
                            lli.Category = attribute.ToString() ?? "";
                            break;
                        case 4:
                            lli.Description = attribute.ToString() ?? "";
                            break;
                        case 5:
                            lli.Status = attribute.ToString() ?? "";
                            break;
                        case 6:
                            lli.Visibility = attribute.ToString() ?? "";
                            break;
                        case 7:
                            lli.Deadline = attribute.ToString() ?? "";
                            break;
                        case 8:
                            lli.Cost = Convert.ToInt32(attribute);
                            break;
                        case 9:
                            lli.Recurrence.Status = attribute.ToString() ?? "";
                            break;
                        case 10:
                            lli.Recurrence.Frequency = attribute.ToString() ?? "";
                            break;
                        case 11:
                            lli.CreationDate = attribute.ToString() ?? "";
                            break;
                        case 12:
                            lli.CompletionDate = attribute.ToString() ?? "";
                            break;
                        default:
                            break;
                    }
                    index++;
                }
            
            }
        return cleanedRecs;
        }
    }
}

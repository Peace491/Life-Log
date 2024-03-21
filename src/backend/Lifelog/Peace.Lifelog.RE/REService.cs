﻿namespace Peace.Lifelog.RE;

using Peace.Lifelog.DataAccess;
using System.Diagnostics;
using DomainModels;
public class REService : IReService
{
    private readonly RecomendationEngineRepository recomendationEngineRepository;

    // Inject RecomendationEngineRepository through the constructor
    public REService(RecomendationEngineRepository recomendationEngineRepository)
    {
        this.recomendationEngineRepository = recomendationEngineRepository;
    }
    public async Task<Response> getNumRecs(string userhash, int numRecs)
    {
        if(!validateNumRecs(numRecs)) throw new ArgumentException("Number of recomendations must be between 1 and 10");

        var recomendationEngineRepository = new RecomendationEngineRepository();
        var recomendations = await recomendationEngineRepository.GetNumRecs(userhash, numRecs);

        var response = new Response();
        response.Output = recomendations.Output;

        return response;
    }
    
    // Helper Functions
    private bool validateNumRecs(int numRecs)
    {
        if (numRecs < 1 || numRecs > 10)
        {
            return false;
        }
        return true;
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


// // OLD RE SERVICE
// using DomainModels;
// using Org.BouncyCastle.Asn1.Cmp;
// using Peace.Lifelog.DataAccess;
// using Peace.Lifelog.LLI;
// using Peace.Lifelog.Logging;

// // should this be like this ?? im not understanding
// namespace Peace.Lifelog.RE
// {
//     public class ReService : IGetNumRecs
//     {
//         public async Task<Response> getNumRecs(string userhash, int numRecs)
//         {
//             var response = new Response();
//             // var logger = new Logger();
//             var recomendationEngineRepository = new RecomendationEngineRepository();

//             try
//             {
//                 // Constructs sql, based on re datamart then queries db
//                 var recomendations = await recomendationEngineRepository.GetNumRecs(userhash, numRecs);

//                 // Clean recomendations 
//                 var cleanedRecomendations = cleanRecs(recomendations, userhash);
                
//                 // Populate response
//                 response.Output = cleanedRecomendations;

//                 return response;
//             }
//             catch (Exception e)
//             {
//                 // logger.Log(e.Message);
//                 response.ErrorMessage = e.Message;
//                 response.Output = null;
//                 return response;
//             }

//         }

//         // Helper Functions
//         private Dictionary<string, int> scoreInit(Response formResponse)
//         {
//             Dictionary<string, int> userScores = new Dictionary<string, int>();
//             if (formResponse.Output == null) return userScores;
//             foreach (List<Object> pair in formResponse.Output)
//             {
//                 if (pair[0] == null || pair[1] == null || pair == null)
//                 {
//                     return userScores;
//                 }
                
//                 string category = pair[0].ToString();
//                 int rank = Convert.ToInt32(pair[1]);
//                 switch (rank)
//                 // Score categories using f1 scoring system
//                 {
//                     case 1:
//                         userScores.Add(category, 25);
//                         break;
//                     case 2:
//                         userScores.Add(category, 18);
//                         break;
//                     case 3:
//                         userScores.Add(category, 15);
//                         break;
//                     case 4:
//                         userScores.Add(category, 12);
//                         break;
//                     case 5:
//                         userScores.Add(category, 10);
//                         break;
//                     case 6:
//                         userScores.Add(category, 8);
//                         break;
//                     case 7:
//                         userScores.Add(category, 6);
//                         break;
//                     case 8:
//                         userScores.Add(category, 4);
//                         break;
//                     case 9:
//                         userScores.Add(category, 2);
//                         break;
//                     case 10:
//                         userScores.Add(category, 1);
//                         break;
//                     default:
//                         userScores.Add(category, 0);
//                         break;
//                 }
//             }
//             return userScores;
//         }

//         private REDataMart convertDataMartResponseToREDataMart(Response dataMartResponse, REDataMart dataMart)
//         {
//             foreach (List<Object> row in dataMartResponse.Output)
//             {
//                 dataMart.UserHash = row[0].ToString();
//                 dataMart.MostCommonUserCategory = row[1].ToString();
//                 dataMart.MostCommonUserSubCategory = row[2].ToString();
//                 dataMart.MostCommonPublicCategory = row[3].ToString();
//             }
//             return dataMart;
//         }

//         private Dictionary<string, int> scoreLLI(Dictionary<string, int> scoreDict, Response lliResponse)
//         {
//             if (lliResponse.Output != null)
//             {
//                 foreach (List<Object> lli in lliResponse.Output)
//                 {
//                     int statusMultiplier;
//                     switch (lli[1]) // index by whereever status is in the lli response
//                     {
//                         case "Postponed":
//                             statusMultiplier = (int)0.8;
//                             break;
//                         case "Active":
//                             statusMultiplier = 1;
//                             break;
//                         case "Completed":
//                             statusMultiplier = (int)1.2;
//                             break;
//                         default:
//                             statusMultiplier = 1;
//                             break;
//                     }
//                     string? currentCategory = lli[0]?.ToString();
//                     if (currentCategory != null && scoreDict.ContainsKey(currentCategory)) // contains key category
//                     {
//                         scoreDict[currentCategory] += 5 * statusMultiplier;
//                     }
//                 }
//             }
//                 return scoreDict; 
//             }

//         private List<string> getTopTwoCategories(Dictionary<string, int> scoreDict)
//         {
//             // https://stackoverflow.com/questions/22957537/dictionary-getting-top-most-elements
//             var topTwoKeys = scoreDict.OrderByDescending(kvp => kvp.Value)
//                                       .Take(2)
//                                       .Select(kvp => kvp.Key)
//                                       .ToList();
//             return topTwoKeys;
//         }
            
//         // Responsible for dynamically constructing the SQL query to pull the records from the database
//         private string constructRecSql(int numberRecomendations, REDataMart rEDataMart) // Params for helper need discussion
//         {
//             // string firstPart = $"SET @UsersMostCommon = \"{rEDataMart.MostCommonUserCategory}\"; SET @UserSubCategory = \"{rEDataMart.MostCommonUserSubCategory}\"; SET @MostCommonPublic = \"{rEDataMart.MostCommonUserCategory}\"; SET @ExcludedUserHash = \"{rEDataMart.UserHash}\";";
//             string firstPart = $"SET @UsersMostCommon = \"{rEDataMart.MostCommonUserCategory}\"; SET @UserSubCategory = 'Art'; SET @MostCommonPublic = 'Mental Health'; SET @ExcludedUserHash = 'System';";
//             string secondPart = "WITH LLIWithCategory AS (SELECT LLI.LLIId, LLI.Title, LLI.Description, LLI.RecurrenceStatus, LLI.RecurrenceFrequency, LC.Category FROM LLI JOIN LLICategories LC ON LLI.LLIId = LC.LLIId WHERE LLI.UserHash != @ExcludedUserHash)";

//             if(numberRecomendations < 1) throw new ArgumentException("Number of recomendations must be greater than 0");
            
//             string query = firstPart + secondPart;
//             int recsLeft = numberRecomendations;
//             int current;

//             while (recsLeft > 0)
//             {
//                 current = recsLeft % 5;
//                 // TODO : Add logic to construct the SQL query based on the number of recomendations requested
//                 if (current == 0)
//                 {
//                     // Add logic to pull 5 recomendations
//                     query += "SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category = @UsersMostCommon ORDER BY RAND() LIMIT 2 ) AS Cat1 UNION ALL SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category = @UserSubCategory ORDER BY RAND() LIMIT 1 ) AS Cat2 UNION ALL SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category = @MostCommonPublic ORDER BY RAND() LIMIT 1 ) AS Cat3 UNION ALL SELECT * FROM ( SELECT * FROM LLIWithCategory WHERE Category NOT IN (@UsersMostCommon, @UserSubCategory, @MostCommonPublic) ORDER BY RAND() LIMIT 1 ) AS OtherCats;";
//                     recsLeft -= 5;
//                 }
//                 if (current == 3)
//                 {
//                     // Add logic to pull 3 recomendations
//                     recsLeft -= 3;
//                 }
//                 if (current == 2)
//                 {
//                     // Add logic to pull 2 recomendations
//                     recsLeft -= 2;
//                 }
//                 if (current == 1)
//                 {
//                     // Add logic to pull 1 recomendation
//                     recsLeft--;
//                 }
//             }           
//             return query;
//         }

//         // Parse the records and clean them up
//         // TODO : idetifiy personally identifiable info and "clean" it
//         private List<Object> cleanRecs(Response recomendationResponse, string userHash)
//         {
//             List<object> cleanedRecs = new List<Object>();
//             if (recomendationResponse.Output != null)
//             {
//                 foreach (List<Object> LLI in recomendationResponse.Output)
//                 {
//                     int index = 0;
//                     var lli = new LLI.LLI();
                    
//                     foreach (var attribute in LLI) {
//                         if (attribute is null) continue;
                        
//                         switch (index)
//                         {
//                             case 0:
//                                 lli.LLIID = "";
//                                 break;
//                             case 1:
//                                 lli.UserHash = userHash ?? "";
//                                 break;
//                             case 2:
//                                 lli.Title = attribute.ToString() ?? "";
//                                 break;
//                             case 3:
//                                 lli.Description = "";
//                                 break;
//                             case 4:
//                                 lli.Status = "";
//                                 break;
//                             case 5:
//                                 lli.Visibility = "Public";
//                                 break;
//                             case 6:
//                                 lli.Deadline = "";
//                                 break;
//                             case 7:
//                                 lli.Cost = Convert.ToInt32(attribute);
//                                 break;
//                             case 8:
//                                 lli.Recurrence.Status = attribute.ToString() ?? "";
//                                 break;
//                             case 9:
//                                 lli.Recurrence.Frequency = attribute.ToString() ?? "";
//                                 break;
//                             case 10:
//                                 lli.CreationDate = "";
//                                 break;
//                             case 11:
//                                 lli.CompletionDate = "";
//                                 break;
//                             default:
//                                 break;
//                         }
//                         index++;
//                     }
//                     cleanedRecs.Add(lli);
//                 }
//             }
//             return cleanedRecs;
//         }
//     }
// }


// -- First, define the category names 
// SET @Category1 = 'Mental Health';
// SET @Category2 = 'Art';
// SET @Category3 = 'Food';
// -- Define the UserHash of the user to exclude
// SET @ExcludedUserHash = 'System';

// -- Create a table to hold the LLI records with their categories, so we only join once
// WITH LLIWithCategory AS (SELECT
// 	LLI.LLIId,
// 	LLI.Title,
//     LLI.Description,
//     LLI.RecurrenceStatus,
//     LLI.RecurrenceFrequency,
// 	LC.Category
// FROM LLI
// JOIN LLICategories LC ON LLI.LLIId = LC.LLIId
// WHERE LLI.UserHash != 'System' -- expand this condition to match brd soon
// )

// -- Subquery for most popular category
// SELECT * FROM (
//     SELECT *
//     FROM LLIWithCategory
//     WHERE Category = @Category1
//     ORDER BY RAND() 
//     LIMIT 2
// ) AS Cat1

// UNION ALL
// -- Subquery for second most popular category
// SELECT * FROM (
//     SELECT *
//     FROM LLIWithCategory
//     WHERE Category = @Category2
//     ORDER BY RAND()
//     LIMIT 1
// ) AS Cat2

// UNION ALL
// Subquery for systems most popular category
// SELECT * FROM (
//     SELECT *
//     FROM LLIWithCategory
//     WHERE Category = @Category3
//     ORDER BY RAND()
//     LIMIT 1
// ) AS Cat3

// UNION ALL
// -- Subquery for an item of another category
// SELECT * FROM (
//     SELECT *
//     FROM LLIWithCategory
//     WHERE Category NOT IN (@Category1, @Category2, @Category3)
//     ORDER BY RAND()
//     LIMIT 1
// ) AS OtherCats;


// string sqlGetUserFormData = $"SELECT Category, Rating FROM LifelogDB.UserForm WHERE UserHash=\"{hash}\" ORDER BY Rating ASC;";
            // string sqlGetUserLLI = $"SELECT Category, Status FROM LLI JOIN LLICategories ON LLI.LLIId = LLICategories.LLIId WHERE LLI.UserHash = \"{hash}\";"; // get lli where userhash = userhash, only get category and status
            // string sqlCommonPublic = "SELECT LLICategories.Category, COUNT(*) as Count FROM LLI JOIN LLICategories ON LLI.LLIId = LLICategories.LLIId WHERE LLI.Visibility = 'public' GROUP BY LLICategories.Category ORDER BY Count DESC LIMIT 1;";
            // string queryDataMart = $"SELECT UserHash, MostCommonUserCategory, MostCommonUserSubCategory FROM REDataMart WHERE UserHash = \"{hash}\";";
            // string queryDataMartSystem = $"SELECT UserHash, MostCommonUserCategory, MostCommonUserSubCategory FROM REDataMart WHERE UserHash = 'System';";

            // string query = $"SELECT r.userhash, r.CategoryOne, r.CategoryTwo, (SELECT CategoryOne FROM RecomendationDataMart WHERE userhash = 'system') AS systemCategoryOne FROM RecomendationDataMart r WHERE r.userhash = \"{userhash}\";";

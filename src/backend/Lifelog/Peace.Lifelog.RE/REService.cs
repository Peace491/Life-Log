using DomainModels;
using Org.BouncyCastle.Asn1.Cmp;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.LLI;
using Peace.Lifelog.Logging;

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

            // string sqlGetUserFormData = $"SELECT Category, Rating FROM LifelogDB.UserForm WHERE UserHash=\"{hash}\" ORDER BY Rating ASC;";
            // string sqlGetUserLLI = $"SELECT Category, Status FROM LLI JOIN LLICategories ON LLI.LLIId = LLICategories.LLIId WHERE LLI.UserHash = \"{hash}\";"; // get lli where userhash = userhash, only get category and status
            // string sqlCommonPublic = "SELECT LLICategories.Category, COUNT(*) as Count FROM LLI JOIN LLICategories ON LLI.LLIId = LLICategories.LLIId WHERE LLI.Visibility = 'public' GROUP BY LLICategories.Category ORDER BY Count DESC LIMIT 1;";
            // string queryDataMart = $"SELECT UserHash, MostCommonUserCategory, MostCommonUserSubCategory FROM REDataMart WHERE UserHash = \"{hash}\";";
            // string queryDataMartSystem = $"SELECT UserHash, MostCommonUserCategory, MostCommonUserSubCategory FROM REDataMart WHERE UserHash = 'System';";

            string query = $"SELECT r.userhash, r.CategoryOne, r.CategoryTwo, (SELECT CategoryOne FROM RecomendationDataMart WHERE userhash = 'system') AS systemCategoryOne FROM RecomendationDataMart r WHERE r.userhash = \"{userhash}\";";

            Dictionary<string, int> userScores = new Dictionary<string, int>();
            Dictionary<string, int> userScoresLLI = new Dictionary<string, int>();

            REDataMart systemDataMart = new REDataMart();
            REDataMart userDataMart = new REDataMart();
            try
            {
                // query datamart for user's personal data
                var dataMartResponse = await readDataOnlyDAO.ReadData(query);
                userDataMart = convertDataMartResponseToREDataMart(dataMartResponse, userDataMart);

                // // Get user's user form info
                // var formResponse = await readDataOnlyDAO.ReadData(sqlGetUserFormData);
                // userScores = scoreInit(formResponse);

                // // Get user's LLI info
                // var userLLIResponse = await readDataOnlyDAO.ReadData(sqlGetUserLLI);
                // userScoresLLI = scoreLLI(userScores, userLLIResponse);
                
                // // Get users top two categories from LLI info
                // var usersTopTwoCategories = getTopTwoCategories(userScoresLLI);

                // // Get the most common public category
                // var commonCategoryResponse = await readDataOnlyDAO.ReadData(sqlCommonPublic);

                // Construct the sql, based on the user's scores, the most common public category, and the number of recomendations requested byuser
                var recomendationQuery = constructRecSql(numRecs, userDataMart);

                // Get the recomendations
                var recomendationResponse = await readDataOnlyDAO.ReadData(recomendationQuery);

                // Clean the recomendations, removing info that could be used to identify users
                var cleanedRecomendations = cleanRecs(recomendationResponse, userhash);
                
                // Populate response
                response.Output = cleanedRecomendations;

                // logger.Log(TODO: log the response here);
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
            if (formResponse.Output == null) return userScores;
            foreach (List<Object> pair in formResponse.Output)
            {
                if (pair[0] == null || pair[1] == null || pair == null)
                {
                    return userScores;
                }
                
                string category = pair[0].ToString();
                int rank = Convert.ToInt32(pair[1]);
                switch (rank)
                // Score categories using f1 scoring system
                {
                    case 1:
                        userScores.Add(category, 25);
                        break;
                    case 2:
                        userScores.Add(category, 18);
                        break;
                    case 3:
                        userScores.Add(category, 15);
                        break;
                    case 4:
                        userScores.Add(category, 12);
                        break;
                    case 5:
                        userScores.Add(category, 10);
                        break;
                    case 6:
                        userScores.Add(category, 8);
                        break;
                    case 7:
                        userScores.Add(category, 6);
                        break;
                    case 8:
                        userScores.Add(category, 4);
                        break;
                    case 9:
                        userScores.Add(category, 2);
                        break;
                    case 10:
                        userScores.Add(category, 1);
                        break;
                    default:
                        userScores.Add(category, 0);
                        break;
                }
            }
            return userScores;
        }

        private REDataMart convertDataMartResponseToREDataMart(Response dataMartResponse, REDataMart dataMart)
        {
            foreach (List<Object> row in dataMartResponse.Output)
            {
                dataMart.UserHash = row[0].ToString();
                dataMart.MostCommonUserCategory = row[1].ToString();
                dataMart.MostCommonUserSubCategory = row[2].ToString();
                dataMart.MostCommonPublicCategory = row[3].ToString();
            }
            return dataMart;
        }

        private Dictionary<string, int> scoreLLI(Dictionary<string, int> scoreDict, Response lliResponse)
        {
            if (lliResponse.Output != null)
            {
                foreach (List<Object> lli in lliResponse.Output)
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
                    string? currentCategory = lli[0]?.ToString();
                    if (currentCategory != null && scoreDict.ContainsKey(currentCategory)) // contains key category
                    {
                        scoreDict[currentCategory] += 5 * statusMultiplier;
                    }
                }
            }
                return scoreDict; 
            }

        private List<string> getTopTwoCategories(Dictionary<string, int> scoreDict)
        {
            // https://stackoverflow.com/questions/22957537/dictionary-getting-top-most-elements
            var topTwoKeys = scoreDict.OrderByDescending(kvp => kvp.Value)
                                      .Take(2)
                                      .Select(kvp => kvp.Key)
                                      .ToList();
            return topTwoKeys;
        }
            
        // Responsible for dynamically constructing the SQL query to pull the records from the database
        private string constructRecSql(int numberRecomendations, REDataMart rEDataMart) // Params for helper need discussion
        {
            string firstPart = $"-- define stuff SET @UsersMostCommon = \"{rEDataMart.MostCommonUserCategory}\"; SET @UserSubCategory = \"{rEDataMart.MostCommonUserSubCategory}\"; SET @MostCommonPublic = \"{rEDataMart.MostCommonUserCategory}\"; SET @ExcludedUserHash = \"{rEDataMart.UserHash}\";";
            if(numberRecomendations < 1) throw new ArgumentException("Number of recomendations must be greater than 0");
            if(numberRecomendations == 1) return "SELECT * FROM LLI WHERE Visibility = 'Public' LIMIT 1;"; // return a single record
            int recsLeft = numberRecomendations;
            while (recsLeft > 0)
            {
                // TODO : Add logic to construct the SQL query based on the number of recomendations requested
                if (recsLeft % 5 == 0)
                {
                    // Add logic to pull 5 recomendations
                    recsLeft -= 5;
                }
                if (recsLeft % 3 == 0)
                {
                    // Add logic to pull 3 recomendations
                    recsLeft -= 3;
                }
                if (recsLeft % 2 == 0)
                {
                    // Add logic to pull 2 recomendations
                    recsLeft -= 2;
                }
                if (recsLeft == 1)
                {
                    // Add logic to pull 1 recomendation
                    recsLeft--;
                }
            }
            // TODO : Add logic to pull records from the database
            string query1 = @"
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

            string query = "SELECT * FROM LLI WHERE Visibility = 'Public';";
            
            return query;
        }

        // Parse the records and clean them up
        // TODO : idetifiy personally identifiable info and "clean" it
        private List<Object> cleanRecs(Response recomendationResponse, string userHash)
        {
            List<object> cleanedRecs = new List<Object>();
            if (recomendationResponse.Output != null)
            {
                foreach (List<Object> LLI in recomendationResponse.Output)
                {
                    int index = 0;
                    var lli = new LLI.LLI();
                    
                    foreach (var attribute in LLI) {
                        if (attribute is null) continue;
                        
                        switch (index)
                        {
                            case 0:
                                lli.LLIID = "";
                                break;
                            case 1:
                                lli.UserHash = userHash ?? "";
                                break;
                            case 2:
                                lli.Title = attribute.ToString() ?? "";
                                break;
                            case 3:
                                lli.Description = "";
                                break;
                            case 4:
                                lli.Status = "";
                                break;
                            case 5:
                                lli.Visibility = "Public";
                                break;
                            case 6:
                                lli.Deadline = "";
                                break;
                            case 7:
                                lli.Cost = Convert.ToInt32(attribute);
                                break;
                            case 8:
                                lli.Recurrence.Status = attribute.ToString() ?? "";
                                break;
                            case 9:
                                lli.Recurrence.Frequency = attribute.ToString() ?? "";
                                break;
                            case 10:
                                lli.CreationDate = "";
                                break;
                            case 11:
                                lli.CompletionDate = "";
                                break;
                            default:
                                break;
                        }
                        index++;
                    }
                    cleanedRecs.Add(lli);
                }
            }
            return cleanedRecs;
        }
    }
}


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


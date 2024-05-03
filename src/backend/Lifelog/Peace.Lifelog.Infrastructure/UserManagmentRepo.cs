namespace Peace.Lifelog.Infrastructure;

// public class UserManagmentRepo : IUserManagmentRepo
// {
//     private readonly ILogger<UserManagmentRepo> _logger;
//     private readonly LifelogDbContext _context;

//     public UserManagmentRepo(ILogger<UserManagmentRepo> logger, LifelogDbContext context)
//     {
//         _logger = logger;
//         _context = context;
//     }

//     public async Task<Response> DeletePersonalIdentifiableInformation(string userHash)
//     {
//         try
//         {
//             var user = await _context.Users.FirstOrDefaultAsync(x => x.UserHash == userHash);
//             if (user == null)
//             {
//                 return new Response { Status = "Error", Message = "User not found" };
//             }
//             _context.Users.Remove(user);
//             await _context.SaveChangesAsync();
//             return new Response { Status = "Success", Message = "User deleted successfully" };
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error in DeletePersonalIdentifiableInformation");
//             return new Response { Status = "Error", Message = "Error in DeletePersonalIdentifiableInformation" };
//         }
//     }

//     public async Task<Response> ViewPersonalIdentifiableInformation(string userHash)
//     {
//         try
//         {
//             var user = await _context.Users.FirstOrDefaultAsync(x => x.UserHash == userHash);
//             if (user == null)
//             {
//                 return new Response { Status = "Error", Message = "User not found" };
//             }
//             return new Response { Status = "Success", Message = "User found", Data = user };
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error in ViewPersonalIdentifiableInformation");
//             return new Response { Status = "Error", Message = "Error in ViewPersonalIdentifiableInformation" };
//         }
//     }
// }

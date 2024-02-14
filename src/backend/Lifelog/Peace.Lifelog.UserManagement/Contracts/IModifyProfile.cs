namespace Peace.Lifelog.UserManagement;

using DomainModels;
/// <summary>
/// Modify profile in DB
/// </summary>
public interface IModifyProfile
{
    public Task<Response> ModifyProfile(IUserProfileRequest userProfileRequest);
}

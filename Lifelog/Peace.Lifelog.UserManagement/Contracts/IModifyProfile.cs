namespace Peace.Lifelog.UserManagement;

using DomainModels;

public interface IModifyProfile
{
    public Task<Response> ModifyProfile(IUserProfileRequest userProfileRequest);
}

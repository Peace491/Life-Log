﻿namespace Peace.Lifelog.UserManagement;
/// <summary>
/// For all User Profile
/// </summary>
public interface IUserProfileRequest
{
    public string ModelName { get; }
    public (string Type, string Value) UserId { get; set; }

}

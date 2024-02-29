﻿using Peace.Lifelog.UserManagement;

namespace Peace.Lifelog.UserManagementTest;

public class LifelogProfileRequest : IUserProfileRequest
{
    public string ModelName { get; } = "LifelogProfile";
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) DOB { get; set; }
    public (string Type, string Value) ZipCode { get; set; }

}
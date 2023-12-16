﻿using System.Dynamic;
using Peace.Lifelog.UserManagement;

namespace Peace.Lifelog.UserManagementTest;

public class TestProfileRequest : IUserProfileRequest
{
    public string ModelName { get; } = "TestProfile";
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) DOB { get; set; }
    public (string Type, string Value) ZipCode { get; set; }

}

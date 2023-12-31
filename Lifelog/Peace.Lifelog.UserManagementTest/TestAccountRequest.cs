﻿namespace Peace.Lifelog.UserManagementTest;

using Peace.Lifelog.UserManagement;

public class TestAccountRequest : IUserAccountRequest, IMultifactorAccountRequest, IStatusAccountRequest
{
    public string ModelName { get; } = TestVariables.ACCOUNT_TABLE;
    public (string Type, string Value) UserId { get; set; }
    public (string Type, string Value) UserHash { get; set; }
    public (string Type, string Value) CreationDate { get; set; }
    public (string Type, string Value) Password { get; set; }
    public (string Type, string Value) MfaId { get; set; }
    public (string Type, string Value) AccountStatus { get; set; } = (TestVariables.STATUS_TYPE, "Enabled");
}
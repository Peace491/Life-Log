﻿namespace Peace.Lifelog.UserManagement;

using DomainModels;
using Peace.Lifelog.UserManagementTest;

/// <summary>
/// Recover account in DB using Status
/// </summary>
public interface IRecoverLifelogUser
{
    public Task<Response> RecoverLifelogUser(LifelogAccountRequest lifelogAccountRequest);

}
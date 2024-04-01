﻿using DomainModels;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.UserForm;

public interface IUserFormService
{
    public Task<Response> CreateUserForm(
        CreateUserFormRequest createUserFormRequest
    );
    public Task<Response> UpdateUserForm(
        UpdateUserFormRequest updateUserFormRequest
    );
}

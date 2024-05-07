namespace Peace.Lifelog.Email;

using DomainModels;

public interface IEmailService
{
   Task<Response> SendOTPEmail(string userHash);
   Task<Response> SendPIIEmail(string userHash, string logFilePath);
}



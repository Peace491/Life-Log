namespace Peace.Lifelog.LifelogReminder;

using DomainModels;

public interface ILifelogReminderService
{
   Task<Response> UpdateReminderConfiguration(ReminderFormData form);
   Task<Response> SendReminderEmail(AppPrincipal appPrincipal);
}


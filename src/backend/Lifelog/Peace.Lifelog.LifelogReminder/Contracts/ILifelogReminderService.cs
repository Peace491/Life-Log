namespace Peace.Lifelog.LifelogReminder;

using DomainModels;
using Peace.Lifelog.Security;

public interface ILifelogReminderService
{
   Task<Response> UpdateReminderConfiguration(ReminderFormData form);
   Task<Response> SendReminderEmail(ReminderFormData form);
}


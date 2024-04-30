using Peace.Lifelog.Security;
namespace Peace.Lifelog.MediaMementoWebService
{
    public class DeleteMediaMementoRequest
    {
        public int LliId { get; set; } = 1;
        public AppPrincipal? AppPrincipal { get; set; }
    }
}

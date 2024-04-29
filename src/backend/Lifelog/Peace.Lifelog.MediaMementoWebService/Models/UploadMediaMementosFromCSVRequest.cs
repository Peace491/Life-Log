using Peace.Lifelog.Security;
namespace Peace.Lifelog.MediaMementoWebService
{
    public class UploadMediaMementosFromCSVRequest
    {
        public List<List<string>> CSVMatrix { get; set; } = [];
        public AppPrincipal? AppPrincipal { get; set; }
    }
}

using Peace.Lifelog.Security;
namespace Peace.Lifelog.MediaMementoWebService
{
    public class UploadMediaMementoRequest
    {
        public required int LliId { get; init; } = 1;
        public required byte[] Binary { get; init; } = new byte[] { 0x48, 0x69 }; // 'Hi' in ASCII
        public required AppPrincipal? AppPrincipal { get; init; }

    }
}

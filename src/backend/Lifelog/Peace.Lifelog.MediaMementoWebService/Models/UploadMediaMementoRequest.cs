namespace Peace.Lifelog.MediaMementoWebService
{
    public class UploadMediaMementoRequest
    {
        public string UserHash { get; set; } = "System";
        public int LliId { get; set; } = 1;
        public byte[] Binary { get; set; } = new byte[] { 0x48, 0x69 }; // 'Hi' in ASCII
    }
}

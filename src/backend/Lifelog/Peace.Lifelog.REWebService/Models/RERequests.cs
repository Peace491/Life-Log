using Peace.Lifelog.Security;

namespace Peace.Lifelog.REWebService
{
    public class PostNumRecsRequest
    {
        public AppPrincipal ?AppPrincipal { get; set; }
        public int NumRecs { get; set; } = 5;
    }
}

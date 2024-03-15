namespace Peace.Lifelog.UserManagementWebService.Models
{
    public class RegisterNormalUserRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string DOB { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

    }

    public class PostOTPRequest
    {
        public string OTP { get; set; } = string.Empty;

    }

}
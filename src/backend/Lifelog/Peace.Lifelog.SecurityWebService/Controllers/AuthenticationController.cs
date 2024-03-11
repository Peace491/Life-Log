using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.Security;

namespace Peace.Lifelog.SecurityWebService;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    [HttpGet]
    [Route("getOTP")]
    public async Task<IActionResult> GetOTP(string UserId) {
        var otpService = new OTPService();
        var response = await otpService.generateOneTimePassword(UserId);

        string OTP = "";
        if (response.Output != null)
        {
            foreach (string output in response.Output) {
                OTP = output;
            }
        }
        

        return Ok(OTP);
    }

}

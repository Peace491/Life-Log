using Microsoft.AspNetCore.Mvc;
using Peace.Lifelog.UserManagementWebService.Models;

namespace Peace.Lifelog.UserManagementWebService.Controllers;
using Peace.Lifelog.RegistrationService;



    [ApiController]
    [Route("registration")]
    public class RegistrationController : ControllerBase
    {
        [HttpPost]
        [Route("postUserData")]
        public async Task<IActionResult> PostUserData([FromBody]PostUserDataRequest postUserDataRequest)
        {
            var registrationService = new RegistrationService();
            var checkInputResponse = await registrationService.CheckInputValidation(postUserDataRequest.UserId, postUserDataRequest.DOB, postUserDataRequest.ZipCode);
            
            
        
            return Ok();
        }

        [HttpPost]
        [Route("postOTP")]
        public IActionResult PostOTP([FromBody]PostOTPRequest postOTPRequest)
        {

            return Ok();
        }
    }


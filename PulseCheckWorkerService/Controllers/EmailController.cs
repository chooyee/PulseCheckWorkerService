using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Factory.EmailService;

namespace PulseCheckWorkerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpGet("Test/{email}")]
        public ActionResult<string> Test(string email)
        {
            var result = SmtpEmailService.SendMail(new List<string>() { email }, "Test", "Test 1 2 3");
            return Ok($"Send result: {result}");

        }
    }
}

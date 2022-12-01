﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseCheckWorkerService.Model;
using System.Security.Claims;

namespace PulseCheckWorkerService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PulseController : ControllerBase
    {
        [HttpGet("IAmAlive")]
        public ActionResult<string> IAmAlive()
        {           
            return Ok($"I am Alive");

        }

        [HttpPost("account")]
        public async ValueTask<ActionResult<string>> RegisterAccount(AccountSignup accountSignup)
        {
            var result = await Factory.Account.Instance.CreateAccount(accountSignup);
            return new JsonResult(result);
        }

        [HttpGet("account/{accountName}")]
        public async ValueTask<ActionResult<string>> GetAccount(string accountName)
        {
            var result = await Factory.Account.Instance.GetAccountInfo(accountName);
            if (result!=null)
                return new JsonResult(result);
            else
                return NotFound();
        }

        [HttpGet("alive/{accountName}")]
        public async ValueTask<ActionResult<string>> RegisterPulse(string accountName)
        {
            var result = await Factory.Pulse.Instance.RegisterPulse(accountName);
            return new JsonResult(result);
        }

        [HttpGet("check")]
        public async ValueTask<ActionResult<List<PulseCheckResult>>> PulseCheck()
        {
            var result = await Factory.Pulse.Instance.CheckPulse();
            return new JsonResult(result);
        }


    }
}

using System;
using System.Linq;
using Lumberjack.Server.Models;
using Lumberjack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumberjack.Server.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogReceiver _logReceiver;

        public LogsController(ILogReceiver logReceiver)
        {
            _logReceiver = logReceiver;
        }

        [HttpPost]
        public IActionResult Receive([FromBody] LogRequestInput input)
        {
            if (!Request.Headers.ContainsKey("x-api-key") || string.IsNullOrEmpty(Request.Headers["x-api-key"].First()))
                return Unauthorized("API KEY IS REQUIRED");
            if (!Request.Headers.ContainsKey("x-instance") || string.IsNullOrEmpty(Request.Headers["x-instance"].First()))
                return Unauthorized("INSTANCE NAME IS REQUIRED");
            var apiKey = Request.Headers["x-api-key"].First();
            var instance = Request.Headers["x-instance"].First();
            var result = _logReceiver.ValidateAndDispatch(apiKey, instance, input.Data);
            return (result) switch
            {
                InputValidationResult.OK => Ok(),
                InputValidationResult.INVALID_API_KEY => Unauthorized("INVALID API KEY"),
                InputValidationResult.LOG_EMPTY => BadRequest("LOGS EMPTY"),
                _ => BadRequest()
            };
        }

        public class LogRequestInput
        {
            public RawLogInput[] Data { get; set; } = Array.Empty<RawLogInput>();
        }
    }
}

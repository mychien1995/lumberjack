using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lumberjack.Server.Models;
using Lumberjack.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
// ReSharper disable MethodSupportsCancellation

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

        [HttpGet("source")]
        public async Task GetSource(CancellationToken token)
        {
            var response = Response;
            response.StatusCode = 200;
            response.Headers.Add("Content-Type", "text/event-stream");

            async void OnLogReceived(object sender, MessageReceivedEventArg arg)
            {
                var log = arg.Log;
                await response.WriteAsync($"data:{JsonConvert.SerializeObject(log)}\n\n");
                await response.Body.FlushAsync();
            }

            _logReceiver.OnMessageReceived += OnLogReceived;
            while (!token.IsCancellationRequested)
                await Task.Delay(1000);
            _logReceiver.OnMessageReceived -= OnLogReceived;
        }

        private void _logReceiver_OnMessageReceived(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public class LogRequestInput
        {
            public RawLogInput[] Data { get; set; } = Array.Empty<RawLogInput>();
        }
    }
}

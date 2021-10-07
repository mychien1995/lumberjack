using Lumberjack.Server.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Lumberjack.Server.Framework.Filters
{
    public class LogExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<LogExceptionFilter> _logger;

        public LogExceptionFilter(ILogger<LogExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, $"Error on executing request {context.HttpContext.Request.Path}");
            context.Result = new ObjectResult(new ErrorResponse(context.Exception));
        }
    }
}

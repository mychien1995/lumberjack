using System.Text;
using Microsoft.AspNetCore.Http;

namespace Lumberjack.Serilog.Sink
{
    public static class RequestExtensions
    {
        public static string GetRawBodyString(this HttpContext httpContext, Encoding encoding)
        {
            var body = "";
            if (httpContext.Items.ContainsKey("lb_rq_bd"))
                body = httpContext.Items["lb_rq_bd"]?.ToString() ?? string.Empty;
            return body;
        }
    }
}

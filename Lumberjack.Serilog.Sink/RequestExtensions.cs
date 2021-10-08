﻿using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Lumberjack.Serilog.Sink
{
    public static class RequestExtensions
    {
        public static string GetRawBodyString(this HttpContext httpContext, Encoding encoding)
        {
            var body = "";
            if (httpContext.Request.ContentLength == null || !(httpContext.Request.ContentLength > 0) ||
                !httpContext.Request.Body.CanSeek) return body;
            httpContext.Request.EnableBuffering();
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(httpContext.Request.Body, encoding, true, 1024, true))
            {
                body = reader.ReadToEnd();
            }
            httpContext.Request.Body.Position = 0;
            return body;
        }
    }
}

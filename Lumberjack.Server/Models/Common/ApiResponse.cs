using System;

namespace Lumberjack.Server.Models.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; } = true;
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string Exception { get; set; } = string.Empty;
    }

    public class DataResponse<T> : ApiResponse
    {
        public T Data { get; }

        public DataResponse(T data)
        {
            Data = data;
        }
    }

    public class ErrorResponse : ApiResponse
    {
        public ErrorResponse(Exception ex)
        {
            Success = false;
            Errors = new[] { ex.Message };
            Exception = ex.ToString();
        }
    }
}

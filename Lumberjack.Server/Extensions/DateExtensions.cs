using System;

namespace Lumberjack.Server.Extensions
{
    public static class DateExtensions
    {
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }
    }
}

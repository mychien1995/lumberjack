// ReSharper disable InconsistentNaming
namespace Lumberjack.Server
{
    public static class EventChannels
    {
        public const string ApplicationChanged = "APPLICATION_CHANGED";
    }

    public enum SystemLogLevel
    {
        DEBUG = 1,
        INFO = 2,
        WARNING = 3,
        ERROR = 4,
    }

    public enum InputValidationResult
    {
        INVALID_API_KEY = 1,
        LOG_EMPTY = 2,
        OK = 3
    }
}

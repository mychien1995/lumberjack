using Nelibur.ObjectMapper;

namespace Lumberjack.Server.Extensions
{
    public static class ObjectExtensions
    {
        public static TTo MapTo<TTo>(this object from)
            => TinyMapper.Map<TTo>(from);
    }
}

using System;

namespace SP.Messenger.Common.Helpers
{
    public static class EnumHelper
    {
        public static T ConvertFromString<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value);
        }
    }
}
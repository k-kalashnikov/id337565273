using Newtonsoft.Json;

namespace SP.Messenger.Common.Extensions
{
    public static class JsonExtension
    {
        public static string ToJson<T>(this T item) =>
            JsonConvert.SerializeObject(item);


        public static T FromJson<T>(this string item) =>
            JsonConvert.DeserializeObject<T>(item);
    }
}

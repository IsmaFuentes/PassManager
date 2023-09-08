using Newtonsoft.Json;

namespace PassManager.Helpers
{
    public class Json
    {
        public static T Parse<T>(string content) => JsonConvert.DeserializeObject<T>(content);
        public static string Stringify(object data) => JsonConvert.SerializeObject(data);
    }
}

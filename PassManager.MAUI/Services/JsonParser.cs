using Newtonsoft.Json;

namespace PassManager.MAUI.Services
{
  public interface IJsonParser
  {
    T? Parse<T>(string content);
    string Stringify(object data);
  }

  public class JsonParser : IJsonParser
  {
    public T? Parse<T>(string content)
    {
      return JsonConvert.DeserializeObject<T>(content);
    }

    public string Stringify(object data)
    {
      return JsonConvert.SerializeObject(data);
    }
  }
}

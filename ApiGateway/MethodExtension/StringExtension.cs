using Newtonsoft.Json;

namespace ApiGateway.MethodExtension
{
    public static class StringExtension
    {
        public static T Deserialize<T>(this string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}

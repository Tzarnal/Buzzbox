using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

//Source: http://stackoverflow.com/questions/6288660/net-ensuring-json-keys-are-lowercases

namespace Buzzbox_Common
{
    public class LowercaseJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver()
        };

        public static string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
        }

        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                if (propertyName == "CardClass")
                    return "cardClass";

                return propertyName.ToLower();
            }
        }
    }
}

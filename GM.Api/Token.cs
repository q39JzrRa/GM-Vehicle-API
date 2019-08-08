using JWT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Api
{
    public class CustomJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return JsonUtility.NormalizeJsonString(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public class JsonUtility
    {
        public static string NormalizeJsonString(string json)
        {
            // Parse json string into JObject.
            var parsedObject = JObject.Parse(json);

            // Sort properties of JObject.
            var normalizedObject = SortPropertiesAlphabetically(parsedObject);

            // Serialize JObject .
            return JsonConvert.SerializeObject(normalizedObject);
        }

        private static JObject SortPropertiesAlphabetically(JObject original)
        {
            var result = new JObject();

            foreach (var property in original.Properties().ToList().OrderBy(p => p.Name))
            {
                var value = property.Value as JObject;

                if (value != null)
                {
                    value = SortPropertiesAlphabetically(value);
                    result.Add(property.Name, value);
                }
                else
                {
                    result.Add(property.Name, property.Value);
                }
            }

            return result;
        }
    }



    public class LoginPayload
    {
        public string client_id { get; set; } //from config
        public string device_id { get; set; } //random guid
        public string grant_type { get; set; } //"password"
        public string nonce { get; set; } //return new BigInteger(130, new SecureRandom()).toString(32);
        public string password { get; set; }
        public string scope { get; set; } // onstar gmoc commerce user_trailer msso
        public string timestamp { get; set; } //ISO_8601_DATE_FORMAT.format(new date());
        public string username { get; set; }
    }



    public class UpgradeTokenPayload
    {
        public string client_id { get; set; }
        public string credential { get; set; }
        public string credential_type { get; set; }
        public string device_id { get; set; }
        public string nonce { get; set; }
        public string timestamp { get; set; }
    }



    public class RefreshTokenPayload
    {
        public string assertion { get; set; }
        public string client_id { get; set; }
        public string device_id { get; set; }
        public string grant_type { get; set; }
        public string nonce { get; set; }
        public string scope { get; set; }
        public string timestamp { get; set; }
    }


}

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens.ComponentBasedTokens.ComponentSet
{
    public interface JsonComponentSetToken : JsonToken
    {
        public void AddComponent<T>(T component);
        public T GetComponent<T>();
    }

    public class JCST : JsonComponentSetToken
    {
        private Dictionary<string, object> com = new Dictionary<string, object>();
        private byte[] key;

        public byte[] Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }
        public string KeyString
        {
            get
            {
                return Base64Converter.ToBase64String(key);
            }
            set
            {
                this.key = Base64Converter.FromBase64String(value);
            }
        }

        public JCST(byte[] key)
        {
            this.key = key;
        }

        public void AddComponent<T>(T component)
        {
            com.Add(typeof(T).FullName, component);
        }
        public T GetComponent<T>()
        {
            if (com.ContainsKey(typeof(T).FullName))
            {
                if(com[typeof(T).FullName] is T)
                {
                    return (T)com[typeof(T).FullName];
                }
                else
                {
                    return ((JObject)com[typeof(T).FullName]).ToObject<T>();
                }
            }
            else return default;
        }
        public (JObject header, JObject payload) GetTokenJObjects()
        {
            JObject header = GetHeader();
            JObject payload = GetPayload();

            return (header, payload);
        }

        public JObject GetPayload()
        {
            JObject payload;

            payload = new JObject();
            payload.Add("com", GetCom());

            return payload;
        }

        private JObject GetCom()
        {
            JObject jcom = new JObject();
            foreach (var x in com)
            {
                jcom.Add(x.Key, JObject.FromObject(x.Value));
            }
            return jcom;
        }

        public JObject GetHeader()
        {
            JObject header = new JObject();
            header.Add("alg", "HS256");
            header.Add("typ", "JT-SC");
            return header;
        }

        public string GetJsonFormatString()
        {
            (JObject header, JObject payload) = GetTokenJObjects();

            return $"{JsonConvert.SerializeObject(header, Formatting.None)}.{JsonConvert.SerializeObject(payload, Formatting.None)}";
        }

        public override string ToString()
        {
            (JObject header, JObject payload) = GetTokenJObjects();

            string parts = $"{Base64Converter.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None)))}.{Base64Converter.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None)))}";
            return $"{parts}.{Base64Converter.ToBase64String(System.Security.Cryptography.HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(parts)))}";
        }
        public static JCST FromString(string tokenString, string secret)
        {
            var parts = tokenString.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid token string", nameof(tokenString));

            var header = JObject.Parse(Encoding.UTF8.GetString(Base64Converter.FromBase64String(parts[0])));
            var payload = JObject.Parse(Encoding.UTF8.GetString(Base64Converter.FromBase64String(parts[1])));
            var key = Base64Converter.FromBase64String(parts[2]);

            var token = new JCST(Base64Converter.FromBase64String(secret));

            foreach (JProperty component in payload["com"])
            {
                var type = Type.GetType(component.Name);
                var value = JsonConvert.DeserializeObject(component.Value.ToString(), type);
                token.com[component.Name] = value;
            }

            return token;
        }
    }
}

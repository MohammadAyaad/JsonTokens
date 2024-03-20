using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Cryptography;


namespace Jwt
{
    public class JwtToken
    {


        private Dictionary<string, object> com;
        public byte[] key;

        public JwtToken()
        {
            com = new Dictionary<string, object>();
        }
        
        public void AddComponent<T>(T component)
        {
            com.Add(typeof(T).FullName, component);
        }
        public T GetComponent<T>()
        {
            if (com.ContainsKey(typeof(T).FullName))
                return (T)com[typeof(T).FullName];
            else return default;
        }
        public (JObject header, JObject payload) GetTokenJObjects()
        {
            JObject header = GetHeader();
            JObject payload = GetPayload();

            return (header, payload);
        }

        private JObject GetPayload()
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

        private JObject GetHeader()
        {
            JObject header = new JObject();
            header.Add("alg", "HS256");
            header.Add("typ", "JWT");
            return header;
        }

        public string GetJsonFormatString()
        {
            (JObject header, JObject payload) = GetTokenJObjects();

            return $"{header}.{payload}";
        }

        public override string ToString()
        {
            (JObject header, JObject payload) = GetTokenJObjects();

            string parts = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(header.ToString()))}.{Convert.ToBase64String(Encoding.UTF8.GetBytes(payload.ToString()))}";
            return $"{parts}.{Convert.ToBase64String(System.Security.Cryptography.HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(parts)))}";
        }
        public static JwtToken FromString(string tokenString,string secret)
        {
            var parts = tokenString.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid token string", nameof(tokenString));

            var header = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(parts[0])));
            var payload = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(parts[1])));
            var key = Convert.FromBase64String(parts[2]);

            var token = new JwtToken();

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

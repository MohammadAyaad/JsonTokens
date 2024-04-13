using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Cryptography;


namespace JsonTokens
{
    /*
     {public header}.{payload}.{hash}
     */
    public interface JsonToken 
    {
        public JObject GetHeader(); //contains public meta data about the token it self 
        public JObject GetPayload(); //contains public/private token data
    }

    

}

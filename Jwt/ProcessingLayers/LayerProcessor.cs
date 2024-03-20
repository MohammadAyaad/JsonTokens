using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.ProcessingLayers
{
    public class JwtTokenProcessor
    {
        private Stack<IJwtTokenProcessorLayer> layers = new Stack<IJwtTokenProcessorLayer>();
        public void AddLayer(IJwtTokenProcessorLayer layer)
        {
            layers.Push(layer);
        }
        public (string token, string secret) ToString(JwtToken token)
        {
            string t = token.ToString();
            string s = Convert.ToBase64String(token.key);

            foreach (var layer in layers)
            {
                (t, s) = layer.ToString(t, s);
            }
            return (t, s);
        }

        public JwtToken FromString(string token, string secret)
        {
            foreach (var layer in layers.Reverse())
            {
                (token, secret) = layer.FromString(token, secret);
            }

            return JwtToken.FromString(token, secret);
        }
    }
}

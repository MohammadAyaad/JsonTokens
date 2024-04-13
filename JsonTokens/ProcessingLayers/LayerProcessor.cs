using JsonTokens.ComponentBasedTokens;
using JsonTokens.ComponentBasedTokens.ComponentSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens.ProcessingLayers
{
    public class JsonTokenProcessor
    {
        private Stack<ITokenProcessorLayer> layers = new Stack<ITokenProcessorLayer>();
        public JsonTokenProcessor(
            Stack<ITokenProcessorLayer> layers
            ) {
            this.layers = layers;
        }
        public void AddLayer(ITokenProcessorLayer layer)
        {
            layers.Push(layer);
        }

        public (string token, string secret) ToString(JCST token)
        {
            string t = token.ToString();
            string s = token.KeyString;

            foreach (var layer in layers)
            {
                (t, s) = layer.ToString(t, s);
            }
            return (t, s);
        }
        /*
        public (string token, string secret) ToString(JCST token, int layersToProcess)
        {
            string t = token.ToString();
            string s = Convert.ToBase64String(token.key);


            for (int i = 0; i < layers.Count && i <= layersToProcess; i++)
            {
                (t, s) = layers.ElementAt(i).ToString(t, s);
            }
            return (t, s);
        }
        public (string token,string secret) FromStringToLayer<T>(string token,string secret) where T : ITokenProcessorLayer
        {
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers.ElementAt(i).GetType() == typeof(T))
                    break;
                (token, secret) = layers.ElementAt(i).FromString(token, secret);
            }
            return (token, secret);
        }
        public (string token, string secret) FromStringAtLayer<T>(string token, string secret) where T : ITokenProcessorLayer
        {
            bool facedLayer = false;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers.ElementAt(i).GetType() != typeof(T) && !facedLayer)
                {
                    continue;
                }
                else
                {
                    facedLayer = true;
                }
                (token, secret) = layers.ElementAt(i).FromString(token, secret);
            }
            return (token, secret);
        }

        public (string token,string secret) FromString(string token, string secret,int layersToProcess)
        {
            for(int i = layers.Count - 1; i >= 0 && (layers.Count - i) < layersToProcess;i--)
            {
                (token, secret) = layers.ElementAt(i).FromString(token, secret);
            }

            return (token, secret);
        }
        */
        public JCST FromString(string token, string secret)
        {
            foreach (var layer in layers.Reverse())
            {
                (token, secret) = layer.FromString(token, secret);
            }

            return JCST.FromString(token, secret);
        }
    }
}

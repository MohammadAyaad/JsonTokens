using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens.ProcessingLayers
{
    public interface ITokenProcessorLayer
    {
        public (string token, string secret) ToString(string input, string secret);
        public (string token, string secret) FromString(string token, string secret);
    }
}

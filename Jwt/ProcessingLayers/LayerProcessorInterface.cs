using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.ProcessingLayers
{
    public interface IJwtTokenProcessorLayer
    {
        (string token, string secret) ToString(string input, string secret);
        (string token, string secret) FromString(string token, string secret);
    }
}

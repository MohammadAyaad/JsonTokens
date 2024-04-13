using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens.ProcessingLayers
{
    public interface IIdentifiableTokenProcessorLayer<T> : ITokenProcessorLayer
    {
        public T GetId();
    }
}

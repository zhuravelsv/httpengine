using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public interface IHeaderParametersParser
    {
        HttpHeaderParameter[] Parse(Memory<char> content, char separator = ';');
    }
}

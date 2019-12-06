using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public interface IStartLineParser
    {
        HttpMessageStartLine Parse(Memory<char> content);
    }
}

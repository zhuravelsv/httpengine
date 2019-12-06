using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public interface IHeaderParser
    {
        HttpHeader Parse(Memory<char> rawContent, Memory<char> headerName, Memory<char> headerValue);
    }
}

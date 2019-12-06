using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    internal readonly ref struct HttpMessageData
    {
        public Memory<char> Raw { get; }
        public HttpMessageStartLine StartLine { get; }
        public HttpHeader[] Headers { get; }
        
        public HttpMessageData(Memory<char> rawHeader, HttpMessageStartLine startLine, HttpHeader[] headerFields)
        {
            Raw = rawHeader;
            StartLine = startLine;
            Headers = headerFields;
        }
    }
}

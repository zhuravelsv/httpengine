using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    public class HttpMessage
    {
        public Memory<char> Raw { get; }
        public HttpMessageStartLine StartLine { get; }
        public HttpHeader[] Headers { get; }

        public HttpMessage(Memory<char> rawHeader, HttpMessageStartLine startLine, HttpHeader[] headerFields)
        {
            Raw = rawHeader;
            StartLine = startLine;
            Headers = headerFields;
        }
    }

    public class HttpMessage<T> : HttpMessage
    {
        public T Content { get; }

        public HttpMessage(Memory<char> raw, HttpMessageStartLine startLine, HttpHeader[] headers, T content)
            : base(raw, startLine, headers)
        {
            Content = content;
        }
    }
}

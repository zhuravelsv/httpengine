using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    public class HttpResponseStartLine : HttpMessageStartLine
    {
        public Memory<char> Code { get; }
        public Memory<char> Content { get; }

        public HttpResponseStartLine(HttpMessageType type, Memory<char> rawContent, Memory<char> protocolVersion, Memory<char> code, Memory<char> content) 
            : base(type, rawContent, protocolVersion)
        {
            Code = code;
            Content = content;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    public class HttpRequestStartLine : HttpMessageStartLine
    {
        public Memory<char> Method { get; }
        public Memory<char> Addressee { get; }

        public HttpRequestStartLine(HttpMessageType type, Memory<char> rawContent, Memory<char> protocolVersion, Memory<char> method, Memory<char> addressee) 
            : base(type, rawContent, protocolVersion)
        {
            Addressee = addressee;
            Method = method;
        }
    }
}

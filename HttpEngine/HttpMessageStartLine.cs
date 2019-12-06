using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    public abstract class HttpMessageStartLine
    {
        public Memory<char> RawContent { get; }
        public Memory<char> ProtocolVersion { get; }
        public HttpMessageType Type { get; set; }

        public HttpMessageStartLine(HttpMessageType type, Memory<char> rawContent, Memory<char> protocolVersion)
        {
            Type = type;
            RawContent = rawContent;
            ProtocolVersion = protocolVersion;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    public class HttpHeaderValue
    {
        public Memory<char> RawContent { get; }

        public HttpHeaderValue(Memory<char> rawContent)
        {
            RawContent = rawContent;
        }

        public override string ToString()
        {
            return RawContent.ToString();
        }
    }
}

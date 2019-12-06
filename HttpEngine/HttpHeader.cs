using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine
{
    public class HttpHeader
    {
        public Memory<char> RawContent { get; }
        public Memory<char> Name { get; }
        public HttpHeaderValue[] Values { get; }

        public HttpHeader(Memory<char> rawContent, Memory<char> name, HttpHeaderValue[] values)
        {
            RawContent = rawContent;
            Name = name;
            Values = values;
        }

        public override string ToString()
        {
            return RawContent.ToString();
        }
    }
}

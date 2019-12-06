using System;
using System.Collections.Generic;
using System.Text;
using static System.MemoryExtensions;

namespace HttpEngine
{
    public class HttpHeaderParameter : HttpHeaderValue
    {
        public Memory<char> Value { get; }
        public Memory<char> Name { get; }

        public HttpHeaderParameter(Memory<char> rawContent, Memory<char> name, Memory<char> value) : base(rawContent)
        {
            Value = value;
            Name = name;
        }
    }
}

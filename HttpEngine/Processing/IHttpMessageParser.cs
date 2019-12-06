using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpEngine.Processing
{
    public interface IHttpMessageParser
    {
        HttpMessage ParseMessage(byte[] data);
    }

    public interface IHttpMessageParser<TContent> : IHttpMessageParser
    {
        HttpMessage<TContent> ParseMessageWithContent(byte[] data);
    }
}

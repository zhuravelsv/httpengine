using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Buffers;
using System.Reflection;
using System.IO;

namespace HttpEngine.Processing
{
    public class HttpMessageParser : IHttpMessageParser
    {
        protected readonly IHeaderReader HttpHeaderReader;
        protected readonly IHeaderParser DefaultHeaderFieldParser;
        protected readonly IStartLineParser StartLineParser;
        protected readonly IHttpHeaderSlicer HttpHeaderSlicer;
        protected readonly Dictionary<string, IHeaderParser> HeaderFieldParsers;

        public HttpMessageParser(IHeaderReader httpHeaderReader, IHeaderParser defaultSipHeaderFieldParser,
            IHttpHeaderSlicer httpHeaderSlicer, IStartLineParser startLineParser,
            Dictionary<string, IHeaderParser> headerFieldParsers = null)
        {
            HttpHeaderReader = httpHeaderReader ?? throw new ArgumentNullException(nameof(httpHeaderReader));
            DefaultHeaderFieldParser = defaultSipHeaderFieldParser
                ?? throw new ArgumentNullException(nameof(defaultSipHeaderFieldParser));
            HttpHeaderSlicer = httpHeaderSlicer ?? throw new ArgumentNullException(nameof(httpHeaderSlicer));
            StartLineParser = startLineParser ?? throw new ArgumentNullException(nameof(startLineParser));
            HeaderFieldParsers = headerFieldParsers ?? new Dictionary<string, IHeaderParser>();
        }

        internal HttpMessageData ParseMessageData(Memory<char> data)
        {
            int offset = 0;
            var span = data.Span;
            for (int i = 0; i < data.Length; i++)
            {
                if (span[i] != '\0')
                {
                    offset = i;
                    break;
                }
            }
            data = data.Slice(offset);
            span = data.Span;
            var unprocessedContent = data.Slice(0);
            if (!char.IsLetter(span[0]))
                throw new FormatException("Invalid message format. First word in first line must describe method or protocol version");
            var startLine = StartLineParser.Parse(unprocessedContent);
            unprocessedContent = unprocessedContent.Slice(startLine.RawContent.Length + 2);
            var headers = ProcessHeader(unprocessedContent);
            return new HttpMessageData(data, startLine, headers);
        }

        private unsafe HttpHeader[] ProcessHeader(Memory<char> content)
        {
            int offset = 0;
            int bufferLength = 10;
            const int diapasonSizeInBytes = 8;
            Diapason * buffer = stackalloc Diapason[bufferLength];
            int bufferNextIndex = 0;
            while (true)
            {
                var headerDiapason = Extensions.GetHeader(content.Slice(offset));
                if(bufferNextIndex == bufferLength)
                {
                    int newBufferLength = bufferLength * 2;
                    Diapason* newBuffer = stackalloc Diapason[newBufferLength];
                    Buffer.MemoryCopy(buffer, newBuffer, diapasonSizeInBytes * newBufferLength, diapasonSizeInBytes * bufferLength);
                    buffer = newBuffer;
                    bufferLength = newBufferLength;
                }
                if (headerDiapason.From == 0 && headerDiapason.To == 0) break;
                offset += headerDiapason.To;
                buffer[bufferNextIndex] = headerDiapason;
                bufferNextIndex++;
            }
            HttpHeader[] result = new HttpHeader[bufferNextIndex];
            offset = 0;
            for(int i = 0; i < bufferNextIndex; i++)
            {
                var headerDiapason = buffer[i];
                var rawHeader = content.Slice(offset + headerDiapason.From, (headerDiapason.To - headerDiapason.From));
                offset += headerDiapason.To;
                result[i] = ParseHeader(rawHeader);
            }
            return result;
        }

        private unsafe HttpHeader ParseHeader(Memory<char> rawContent)
        {
            var diapason = HttpHeaderReader.GetNextHeaderInfo(rawContent.Span);
            var name = rawContent.Slice(diapason.Name.From, (diapason.Name.To - diapason.Name.From));
            var value = rawContent.Slice(diapason.Value.From, (diapason.Value.To - diapason.Value.From) + 1);
            var parser = GetParser(name);
            return parser.Parse(rawContent, name, value);
        }

        private IHeaderParser GetParser(Memory<char> name)
        {
            int count = HeaderFieldParsers.Count;
            for (int i = 0; i < count; i++)
            {
                var p = HeaderFieldParsers.ElementAt(i);
                if (MemoryExtensions.Equals(name.Span, p.Key.AsSpan(), StringComparison.OrdinalIgnoreCase))
                    return p.Value;
            }
            return DefaultHeaderFieldParser;
        }

        public HttpMessage ParseMessage(byte[] data)
        {
            var diapason = HttpHeaderSlicer.GetHttpHeaderDiapason(data);
            var rawMemoryContent = Encoding.UTF8.GetChars(data, diapason.From, (diapason.To - diapason.From) - 1).AsMemory();
            var message = ParseMessageData(rawMemoryContent);
            return new HttpMessage(message.Raw, message.StartLine, message.Headers);
        }
    }

    public class HttpMessageParser<TContent> : HttpMessageParser, IHttpMessageParser<TContent>
    {
        public HttpMessageParser(IHeaderReader httpHeaderReader, IHeaderParser defaultSipHeaderFieldParser,
            IHttpHeaderSlicer httpHeaderSlicer, IStartLineParser startLineParser,
            Dictionary<string, IHeaderParser> headerFieldParsers = null)
            : base(httpHeaderReader, defaultSipHeaderFieldParser, httpHeaderSlicer, startLineParser, headerFieldParsers)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HttpMessage<TContent> ParseMessageWithContent(byte[] data)
        {
            var diapason = HttpHeaderSlicer.GetHttpHeaderDiapason(data);
            var rawMemoryContent = Encoding.UTF8.GetChars(data, diapason.From, (diapason.To - diapason.From) - 1).AsMemory();
            var message = ParseMessageData(rawMemoryContent);
            return new HttpMessage<TContent>(message.Raw, message.StartLine, message.Headers, default);
        }
    }
}

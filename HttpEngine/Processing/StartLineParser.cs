using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public class StartLineParser : IStartLineParser
    {
        public HttpMessageStartLine Parse(Memory<char> content)
        {
            var span = content.Span;
            var lineLength = span.IndexOf("\r\n".AsSpan());
            var line = content.Slice(0, lineLength);
            span = line.Span;
            lineLength = span.Length;

            HttpMessageStartLine startLine = null;
            HttpMessageType messageType;

            if (MemoryExtensions.Equals(span.Slice(0, 4), "HTTP".AsSpan(), StringComparison.OrdinalIgnoreCase))
                messageType = HttpMessageType.Response;
            else
                messageType = HttpMessageType.Request;

            if (messageType == HttpMessageType.Response)
            {
                int pointer = 0;

                Memory<char> msgProtocolVersion = default;
                Memory<char> msgCode = default;
                Memory<char> msgContent = default;

                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] == ' ')
                    {
                        msgProtocolVersion = content.Slice(pointer, i);
                        pointer = i;
                        break;
                    }
                }
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] != ' ')
                    {
                        pointer = i;
                        break;
                    }
                }
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] == ' ')
                    {
                        msgCode = content.Slice(pointer, i - (pointer));
                        pointer = i;
                        break;
                    }
                }
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] != ' ')
                    {
                        msgContent = content.Slice(i, lineLength - i);
                        break;
                    }
                }
                startLine = new HttpResponseStartLine(messageType, content.Slice(0, lineLength), msgProtocolVersion, msgCode, msgContent);
            }
            else
            {
                int pointer = 0;

                Memory<char> msgProtocolVersion = default;
                Memory<char> msgMethod = default;
                Memory<char> msgAddressee = default;

                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] == ' ')
                    {
                        msgMethod = content.Slice(0, i);
                        pointer = i;
                        break;
                    }
                }
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] != ' ')
                    {
                        pointer = i;
                        break;
                    }
                }
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] == ' ')
                    {
                        msgAddressee = content.Slice(pointer, i - (pointer));
                        pointer = i;
                        break;
                    }
                }
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] != ' ')
                    {
                        pointer = i;
                        break;
                    }
                }
                int end = lineLength;
                for (int i = pointer; i < lineLength; i++)
                {
                    if (span[i] == ' ')
                    {
                        end = i - 1;
                    }
                }
                msgProtocolVersion = content.Slice(pointer, end - pointer);
                startLine = new HttpRequestStartLine(messageType, content.Slice(0, lineLength), msgProtocolVersion, msgMethod, msgAddressee);
            }

            content = content.Slice(lineLength);

            return startLine;
        }
    }
}

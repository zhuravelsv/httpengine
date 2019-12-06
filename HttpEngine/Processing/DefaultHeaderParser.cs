using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public class DefaultHeaderParser : IHeaderParser
    {
        private readonly IHeaderParametersParser _parametersParser;

        public DefaultHeaderParser(IHeaderParametersParser parametersParser = null)
        {
            _parametersParser = parametersParser;
        }

        public HttpHeader Parse(Memory<char> rawContent, Memory<char> headerName, Memory<char> headerValue)
        {
            if(_parametersParser == null)
            {
                return new HttpHeader(rawContent, headerName, new HttpHeaderValue[] { new HttpHeaderValue(headerValue) });
            }
            else
            {
                var valueSpan = headerValue.Span;
                int index = valueSpan.IndexOf(';');
                if(index == -1)
                    return new HttpHeader(rawContent, headerName, new HttpHeaderValue[] { new HttpHeaderValue(headerValue) });
                else
                {
                    var rawParamContent = headerValue.Slice(index + 1);
                    var parameters = _parametersParser.Parse(rawParamContent, ';');
                    HttpHeaderValue[] values = new HttpHeaderValue[parameters.Length + 1];
                    values[0] = new HttpHeaderValue(headerValue.Slice(0, index));
                    for(int i = 1; i < values.Length; i++)
                    {
                        values[i] = parameters[i - 1];
                    }
                    return new HttpHeader(rawContent, headerName, values);
                }
            }
        }
    }
}

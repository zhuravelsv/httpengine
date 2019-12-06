using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public class HeaderReader : IHeaderReader
    {
        public unsafe HeaderInfo GetNextHeaderInfo(Span<char> content)
        {
            int length = content.Length;
            int headerNameStart = 0;
            int headerNameEnd = -1;
            int headerValueStart = -1;
            int headerValueEnd = -1;
            int position = 0;
            char pSpanValue;
            for (int i = position; i < length; i++)
            {
                pSpanValue = content[i];
                if (pSpanValue == ' ')
                {
                    headerNameEnd = i;
                    position = i;
                    break;
                }
            }
            for (int i = position; i < length; i++)
            {
                pSpanValue = content[i];
                if (pSpanValue == ':')
                {
                    position = i + 1;
                    break;
                }
            }
            for (int i = position; i < length; i++)
            {
                pSpanValue = content[i];
                if (pSpanValue != ' ' && pSpanValue != '\r' && pSpanValue != '\n')
                {
                    headerValueStart = i;
                    position = i;
                    break;

                }
            }
            for (int i = length - 1; i >= headerValueStart; i--)
            {
                pSpanValue = content[i];
                if (pSpanValue != ' ' && pSpanValue != '\r' && pSpanValue != '\n')
                {
                    headerValueEnd = i;
                }
            }
            return new HeaderInfo(new Diapason(headerNameStart, headerNameEnd), new Diapason(headerValueStart, headerValueEnd));
        }
    }
}

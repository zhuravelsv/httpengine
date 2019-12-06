using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public class HeaderParametersParser : IHeaderParametersParser
    {
        public unsafe HttpHeaderParameter[] Parse(Memory<char> content, char separator = ';')
        {
            var span = content.Span;
            void* separatorPointer = &separator;
            var count = content.CountOf(new Span<char>(separatorPointer, 1));
            HttpHeaderParameter[] parameters = new HttpHeaderParameter[count + 1];
            int offset = 0;
            var next = GetNextParameter(content.Slice(offset), separator);
            int index = 0;
            while(!next.Null)
            {
                offset += next.RawData.Length + next.Offset;
                parameters[index] = new HttpHeaderParameter(next.RawData, next.Name, next.Value);
                if (offset >= span.Length)
                    break;
                var slice = content.Slice(offset);
                next = GetNextParameter(slice, separator);
                index++;
            }
            return parameters;
        }

        private unsafe (bool Null, Memory<char> RawData, Memory<char> Name, Memory<char> Value, int Offset) 
            GetNextParameter(Memory<char> content, char separator = ';')
        {
            var span = content.Span;
            int length = span.IndexOf(separator);
            length = length == -1 ? span.Length : length;
            fixed (char* pSpan = span)
            {
                int nameStart = -1;
                int nameEnd = -1;
                int valueStart = -1;
                int valueEnd = -1;
                int position = 0;
                bool canContainSpace = false;
                bool newLine = false;
                int offset = 0;
                char currentPSpan = *pSpan;
                for (int i = position; i < length; i++)
                {
                    currentPSpan = pSpan[i];
                    if (currentPSpan != ' ' && currentPSpan != '\r' && currentPSpan != '\n')
                    {
                        nameStart = i;
                        position = i;
                        break;
                    }
                }
                for(int i = position; i < length; i++)
                {
                    currentPSpan = pSpan[i];
                    if (currentPSpan == ' ')
                        throw new InvalidOperationException("Incorrect parameter format");
                    else 
                        if(currentPSpan == '=')
                    {
                        nameEnd = i;
                        position = i + 1;
                        valueStart = position;
                        if (pSpan[valueStart] == '\"')
                            canContainSpace = true;
                        position++;
                        break;
                    }
                }
                for(int i = position; i < length; i++)
                {
                    currentPSpan = pSpan[i];
                    if (currentPSpan == '\r' || currentPSpan == '\n')
                    {
                        if (!newLine && !canContainSpace)
                            valueEnd = i;
                        newLine = true;
                        continue;
                    }
                    else if (currentPSpan == separator)
                    {
                        valueEnd = i;
                        break;
                    }
                    else if(currentPSpan == '\"')
                    {
                        valueEnd = i + 1;
                        break;
                    }
                    else if (!canContainSpace)
                    {
                        if (currentPSpan == ' ')
                        {
                            if (!newLine)
                            {
                                valueEnd = i;
                                break;
                            }
                            else newLine = false;
                        }
                    }
                }
                if (valueEnd == -1)
                    valueEnd = length;
                if(nameStart != -1 && nameEnd != -1 && valueStart != -1 && valueEnd != -1)
                {
                    return (false, content.Slice(nameStart, valueEnd - nameStart), content.Slice(nameStart, nameEnd - nameStart), 
                        content.Slice(valueStart, valueEnd - valueStart), (length - valueEnd) + nameStart + 1 + offset);
                }
                else
                {
                    return (true, default, default, default, default);
                }
            }
        }
    }
}

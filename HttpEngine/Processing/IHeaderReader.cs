using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public interface IHeaderReader
    {
        HeaderInfo GetNextHeaderInfo(Span<char> content);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public readonly ref struct HeaderInfo
    {
        /// <summary>
        /// Header name diapason
        /// </summary>
        public Diapason Name { get; }
        /// <summary>
        /// Header value(s) diapason
        /// </summary>
        public Diapason Value { get; }

        public HeaderInfo(Diapason name, Diapason value)
        {
            Name = name;
            Value = value;
        }
    }
}

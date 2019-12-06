using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public readonly struct Diapason
    {
        /// <summary>
        /// Inclusive
        /// </summary>
        public int From { get; }
        /// <summary>
        /// Exclusive
        /// </summary>
        public int To { get; }

        public Diapason(int from, int to)
        {
            From = from;
            To = to;
        }
    }
}

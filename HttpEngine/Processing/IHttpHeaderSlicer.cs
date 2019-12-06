using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public interface IHttpHeaderSlicer
    {
        Diapason GetHttpHeaderDiapason(byte[] rawMessage);
    }
}

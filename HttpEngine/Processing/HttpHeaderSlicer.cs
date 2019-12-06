using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.Processing
{
    public class HttpHeaderSlicer : IHttpHeaderSlicer
    {
        public unsafe Diapason GetHttpHeaderDiapason(byte[] rawMessage)
        {
            int length = rawMessage.Length - 4;
            for(int i = 0; i < length; i++)
            {
                if(rawMessage[i] == 0x0D && rawMessage[i+1] == 0x0A
                    && rawMessage[i+2] == 0x0D && rawMessage[i+3] == 0x0A)
                {
                    return new Diapason(0, i + 2);
                }
            }
            return new Diapason(0, rawMessage.Length);
        }
    }
}

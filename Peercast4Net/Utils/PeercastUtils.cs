using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.Peercast4Net.Commons.Utils;
using Progressive.Peercast4Net.Commons;
using System.Net;
using System.Threading.Tasks;

namespace Progressive.Peercast4Net.Utils
{
    static class PeercastUtils
    {
        public static string PercentEncode(string text)
        {
            if (text == null)
                return "";
            if (ShiftJisEncodings.CanEncodeShiftJis(text))
            {
                return HttpUtils.ToRfc3986(text, ShiftJisEncodings.ShiftJis);
            }
            return HttpUtils.ToRfc3986(text, Encoding.UTF8);
        }
    }
}

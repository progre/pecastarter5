using System.Text;
using Progressive.Peercast4Net.Commons;
using Progressive.Peercast4Net.Commons.Utils;

namespace Progressive.Peercast4Net.Utils
{
    public static class PeercastUtils
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

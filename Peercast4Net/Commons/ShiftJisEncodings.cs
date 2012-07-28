using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net.Commons
{
    public static class ShiftJisEncodings
    {
        public static Encoding ShiftJis { get; private set; }

        static ShiftJisEncodings()
        {
            ShiftJis = Encoding.GetEncoding("sjis");
        }

        /// <remarks>半角カタカナ、外字は変換できない</remarks>
        public static bool CanEncodeShiftJis(string str)
        {
            foreach (var oneChar in str)
            {
                var bytes = ShiftJis.GetBytes(new char[] { oneChar });
                switch (bytes.Length)
                {
                    case 1:
                        // 半角
                        var b = bytes[0];
                        if (0x80 <= b && b <= 0xff) // 半角カタカナ
                        {
                            return false;
                        }
                        if (b == '?' && oneChar != '?')
                        {
                            return false;
                        }
                        break;
                    case 2:
                        // 全角
                        var c = bytes[0] * 0x100 + bytes[1];
                        if (0x8740 <= c && c <= 0x889e // 機種依存文字
                            || 0xed40 <= c) // 機種依存文字
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }
    }
}

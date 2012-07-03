using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Progressive.Peercast4Net.Commons.Utils
{
    public class HttpUtils
    {
        public static string ToRfc3986(string str, Encoding encoding)
        {
            var rt = new StringBuilder();
            foreach (byte i in encoding.GetBytes(str))
            {
                rt.Append(ToRfc3986Char(i));
            }
            return rt.ToString();
        }

        private static string ToRfc3986Char(byte c)
        {
            if (c == (byte)' ')
            {
                return "+";
            }
            if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z'
                || c == '-' || c == '.' || c == '_' || c == '~')
            {
                return ((char)c).ToString();
            }
            return "%" + c.ToString("X2");
        }
    }
}

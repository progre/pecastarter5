using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Progressive.Peercast4Net.Commons.Utils;
using System.Diagnostics;
using Progressive.Peercast4Net.Commons;

namespace Progressive.Peercast4NetTest.Commons.Utils
{
    public class HttpUtilsTest
    {
        [Fact]
        public void ToRfc3986Test()
        {
            Assert.Equal("test", HttpUtils.ToRfc3986("test", Encoding.GetEncoding("sjis")));
            Assert.Equal("%83e%83X%83g", HttpUtils.ToRfc3986("テスト", Encoding.GetEncoding("sjis")));
        }

        [Fact]
        public void CanEncodeShiftJisTest()
        {
            Assert.True(ShiftJisEncodings.CanEncodeShiftJis("test"));
            Assert.True(ShiftJisEncodings.CanEncodeShiftJis("てすと"));
            Assert.True(ShiftJisEncodings.CanEncodeShiftJis("手洲堵"));
            Assert.True(ShiftJisEncodings.CanEncodeShiftJis("⇔"));
            Assert.True(ShiftJisEncodings.CanEncodeShiftJis(" "));
            Assert.False(ShiftJisEncodings.CanEncodeShiftJis("①"));
            Assert.False(ShiftJisEncodings.CanEncodeShiftJis("ﾃｽﾄ"));
            Assert.False(ShiftJisEncodings.CanEncodeShiftJis("↷"));
        }
    }
}

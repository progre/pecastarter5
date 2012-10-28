using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Plugins.Twitter
{
    class TwitterModel
    {
        public void Tweet(string message, IEnumerable<string> hashtags)
        {
            var sb = new StringBuilder("https://twitter.com/intent/tweet?text=" + message);
            var hashtagsLength = 0;
            foreach (var hashtag in hashtags ?? Enumerable.Empty<string>())
            {
                sb.Append((hashtagsLength <= 0 ? "&hashtags=" : ",") + hashtag);
                hashtagsLength += hashtag.Length + 2;
            }
            ClipTo140(message, hashtagsLength);
            Process.Start(sb.ToString());
        }

        private void ClipTo140(string message, int othersLength)
        {
            var len = message.Length;
            if (len + othersLength > 140)
            {
                var startIndex = len - 2;
                if (startIndex < 0)
                    return;
                message.Remove(startIndex);
                message += "…";
            }
        }
    }
}

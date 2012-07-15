using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Models
{
    public interface IYellowPages
    {
        string Name { get; }
        bool IsCheckNoticeUrl { get; }
        string NoticeUrl { get; }
        IEnumerable<string> Components { get; }
        string GetPrefix(Dictionary<string, string> parameters);
        Dictionary<string, string> Parse(string value);
    }
}

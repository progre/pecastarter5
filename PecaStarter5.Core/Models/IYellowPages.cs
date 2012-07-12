using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Models
{
    public interface IYellowPages
    {
        string Name { get; }
        string NoticeUrl { get; }
        IEnumerable<string> Components { get; }
        string GetPrefix(Dictionary<string, string> parameters);
        Tuple<Dictionary<string, string>, string> Parse(string value);
    }
}

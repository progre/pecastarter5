using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models
{
    public interface IYellowPages
    {
        string Name { get; }
        string Host { get; }
        bool IsCheckNoticeUrl { get; }
        string NoticeUrl { get; }
        IEnumerable<string> Components { get; }
        string GetPrefix(Dictionary<string, string> parameters);
        Dictionary<string, string> Parse(string value);
        Task OnBroadcastAsync();
        Task<int> GetNoticeHashAsync();
    }
}

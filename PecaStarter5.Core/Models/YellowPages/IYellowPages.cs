using System.Collections.Generic;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models.YellowPages
{
    public interface IYellowPages
    {
        string Name { get; }
        string Host { get; }
        bool IsCheckNoticeUrl { get; }
        string NoticeUrl { get; }
        IEnumerable<string> Components { get; }
        bool IsExternal { get; }
        string GetPrefix(Dictionary<string, string> parameters);
        Dictionary<string, string> Parse(string value);
        Task<int> GetNoticeHashAsync();
    }
}

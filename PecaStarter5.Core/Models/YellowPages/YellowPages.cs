using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Models
{
    class YellowPages : IYellowPages
    {
        #region IYellowPages メンバー

        public string Name { get; set; }

        public bool IsCheckNoticeUrl { get; set; }

        public string NoticeUrl { get; set; }

        public IEnumerable<string> Components { get; set; }

        public virtual string GetPrefix(Dictionary<string, string> parameters) { return ""; }

        public Dictionary<string, string> Parse(string value)
        {
            return new Dictionary<string, string>();
        }

        #endregion
    }
}

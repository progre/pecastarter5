using System;

namespace CreteLib
{
    public class HtmlTextNode : HtmlNode
    {
        //テキスト
        private String _text = null;

        public HtmlTextNode(bool ignoreCase) : base(ignoreCase) { }

        /// <summary>
        /// テキスト
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}

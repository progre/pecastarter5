using System;
using System.Collections.Generic;
using System.Text;

namespace CreteLib
{
    /// <summary>
    /// HTMLのノードの情報を持つクラス
    /// </summary>
    public class HtmlNode
    {
        //タグ名
        private String _tagName = null;

        //ノードの種類
        private NodeType _nodeType = NodeType.Tag;

        //子ノードのリスト
        private HtmlNodeList _childNodes = null;

        //親ノード
        private HtmlNode _parentNode = null;

        //属性
        private Dictionary<String, String> _attributes = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HtmlNode(bool ignoreCase)
        {
            _childNodes = new HtmlNodeList();
            _attributes = ignoreCase ?
               new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase) :
               new Dictionary<String, String>();
        }

        /// <summary>
        /// タグ名
        /// </summary>
        public String TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        /// <summary>
        /// ノードの種類
        /// </summary>
        public NodeType NodeType
        {
            get { return _nodeType; }
            set { _nodeType = value; }
        }

        /// <summary>
        /// ノードのすべての子ノードを取得します
        /// </summary>
        public HtmlNodeList ChildNodes
        {
            get { return _childNodes; }
            set { _childNodes = value; }
        }

        /// <summary>
        /// 親のノード
        /// </summary>
        public HtmlNode ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<String, String> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        /// <summary>
        /// タグにはさまれた文字列を返す
        /// タグの子ノードのうちHtmlTextNode型のノード
        /// のTextを連結した値を返す
        /// </summary>
        public String InnerText
        {
            get
            {
                StringBuilder text = new StringBuilder();

                if (NodeType == NodeType.Tag)
                {

                    foreach (HtmlNode child in ChildNodes)
                    {
                        if (child is HtmlTextNode)
                        {
                            text.Append((child as HtmlTextNode).Text);
                        }
                        else
                        {
                            if (child.NodeType == NodeType.SelfComplete)
                            {
                                text.Append(String.Format("<{0}/>", child.TagName));
                            }
                            else if (child.NodeType == NodeType.Tag)
                            {
                                if (child.ChildNodes.Count == 0)
                                {
                                    text.Append(String.Format("<{0}>", child.TagName));
                                }
                            }
                        }
                    }
                }
                else if (NodeType == NodeType.Text)
                {
                    text.Append((this as HtmlTextNode).Text);
                }

                return text.ToString();
            }
        }

        /// <summary>
        /// インデクサ
        /// nameに対応する属性を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[String name]
        {
            get
            {
                foreach (String key in _attributes.Keys)
                {
                    if (key.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return _attributes[key];
                    }
                }

                return null;
            }
        }

    }
}

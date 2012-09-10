using System;
using System.Collections.Generic;

namespace CreteLib
{
    /// <summary>
    /// Htmlドキュメントを表すクラス
    /// </summary>
    internal class HtmlDocument : HtmlNode
    {
        //全てのノード
        private HtmlNodeList _allNodes = null;

        public HtmlDocument(bool ignoreCase = false) : base(ignoreCase) { }

        /// <summary>
        /// 全てのノード
        /// </summary>
        public HtmlNodeList AllNodes
        {
            get { return _allNodes; }
            set { _allNodes = value; }
        }

        /// <summary>
        /// テキストからHTMLドキュメントを読み込む
        /// </summary>
        /// <param name="html"></param>
        public void LoadHtml(String html, bool ignoreCase = false)
        {
            HtmlParser parser = new HtmlParser(html, ignoreCase);
            AllNodes = parser.Parse();

            List<HtmlNode> nodeList = new List<HtmlNode>();

            foreach (HtmlNode node in AllNodes)
            {
                nodeList.Add(node);
            }

            BuildHierarchy(nodeList, this);

            //Console.WriteLine(this);
        }

        /// <summary>
        /// 指定されたタグ名のコレクションを返す
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public HtmlNodeList GetNodesByTagName(String tagName)
        {
            HtmlNodeList nodeList = new HtmlNodeList();

            foreach (HtmlNode node in _allNodes)
            {
                if ((node.NodeType == NodeType.SelfComplete || node.NodeType == NodeType.Tag)
                    && node.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                {
                    nodeList.Add(node);
                }
            }

            return nodeList;
        }

        /// <summary>
        /// 指定されたテキストを含む直近の親のタグのコレクションを返す
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public HtmlNodeList GetNodesByText(String text)
        {
            HtmlNodeList nodeList = new HtmlNodeList();

            foreach (HtmlNode node in _allNodes)
            {
                if (node.NodeType == NodeType.Tag)
                {
                    if (node.InnerText.Contains(text))
                    {
                        nodeList.Add(node);
                    }
                }
            }

            return nodeList;
        }

        /// <summary>
        /// 属性の値に指定された文字を含むタグのコレクションを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlNodeList GetNodesByAttribute(String text)
        {
            HtmlNodeList nodeList = new HtmlNodeList();

            foreach (HtmlNode node in _allNodes)
            {
                if (node.NodeType == NodeType.Tag || node.NodeType == NodeType.SelfComplete)
                {
                    foreach (String value in node.Attributes.Values)
                    {
                        if (value != null && value.Contains(text))
                        {
                            nodeList.Add(node);
                            break;
                        }
                    }
                }
            }

            return nodeList;

        }

        /// <summary>
        /// HTMLの階層を構築する
        /// </summary>
        private void BuildHierarchy(List<HtmlNode> nodeList, HtmlNode node)
        {
            //Console.WriteLine("\r\nBuildHierarchy:{0}-{1}", node != null ? node.TagName : "doc", nodeList.Count);

            int addedText = 0;

            HtmlNodeList parentList = new HtmlNodeList();

            while (true)
            {
                //Console.WriteLine("{0} begin loop:{1}",node != null ? node.TagName : "doc",nodeList.Count);

                HtmlNode startNode = null;
                HtmlNode endNode = null;

                List<HtmlNode> childList = new List<HtmlNode>();

                int startTagPos = -1;
                int endTagPos = -1;

                int endTagCount = 0;

                String endTagName = null;

                //リストの後方から最初にある終了タグを探す
                for (int i = nodeList.Count - 1; i >= 0; i--)
                {
                    //終了タグがまだ見つかっていない場合
                    if (endTagPos == -1)
                    {
                        if (nodeList[i].NodeType == NodeType.EndTag)
                        {
                            endTagPos = i;
                            endTagName = nodeList[i].TagName;
                            endNode = nodeList[i];
                            //Console.WriteLine("{0} endTagName:{1}", node != null ? node.TagName : "doc", endTagName);
                        }
                        else
                        {
                            if (addedText <= nodeList.Count - 1 - i)
                            {
                                parentList.Insert(0, nodeList[i]);
                                addedText++;
                            }
                        }
                    }
                    //終了タグが見つかっている場合
                    else
                    {
                        //同一タグ名の終了タグが見つかった場合
                        if (nodeList[i].NodeType == NodeType.EndTag && nodeList[i].TagName.Equals(endTagName, StringComparison.OrdinalIgnoreCase))
                        {
                            //終了タグを発見してから同一タグの終了タグが見つかった場合、カウントアップする
                            endTagCount++;
                        }
                        //開始タグが見つかった場合
                        else if (nodeList[i].NodeType == NodeType.Tag && nodeList[i].TagName.Equals(endTagName, StringComparison.OrdinalIgnoreCase))
                        {
                            //カウンターが1以上の場合、他の終了タグに対応する開始タグを表す
                            if (endTagCount <= 0)
                            {
                                startTagPos = i;
                                startNode = nodeList[i];

                                //開始タグと終了タグの間を切りとり、子供のノードに渡す
                                for (int x = startTagPos + 1; x < endTagPos; x++)
                                {
                                    nodeList[x].ParentNode = startNode;
                                    childList.Add(nodeList[x]);
                                    //Console.WriteLine("{0}-{1}-{2} childlist:{3} - {4}", startTagPos, endTagPos, node != null ? node.TagName : "doc", nodeList[x].NodeType,nodeList[x].NodeType == NodeType.Text ? nodeList[x].Text : nodeList[x].TagName);
                                }

                                //開始タグ、終了タグを含む、間のタグを親のリストから削除する
                                nodeList.Remove(startNode);
                                nodeList.Remove(endNode);

                                foreach (HtmlNode childNode in childList)
                                {
                                    nodeList.Remove(childNode);
                                }

                                startNode.ParentNode = node;
                                BuildHierarchy(childList, startNode);

                                break;
                            }
                            else
                            {
                                endTagCount--;
                            }
                        }
                    }
                }

                if (endTagPos != -1)
                {
                    if (startTagPos == -1)
                    {
                        //throw new Exception("開始タグがない");
                        nodeList.Remove(endNode);
                        parentList.Insert(0, endNode);
                    }
                    else
                    {
                        //先頭に追加
                        parentList.Insert(0, startNode);
                    }
                }
                else
                {
                    break;
                }

            }

            node.ChildNodes = parentList;

            //Console.WriteLine("終了:BuildHierarchy:{0}-{1}\r\n", node != null ? node.TagName : "doc", nodeList.Count);

        }
    }
}

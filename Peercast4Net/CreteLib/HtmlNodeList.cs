using System.Collections;
using System.Collections.Generic;

namespace CreteLib
{
    /// <summary>
    /// 順序の付いたノードのコレクションを表します
    /// </summary>
    internal class HtmlNodeList : IEnumerable
    {
        //ノードのリスト
        private List<HtmlNode> _nodeList = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HtmlNodeList()
        {
            _nodeList = new List<HtmlNode>();
        }

        /// <summary>
        /// ノードを追加する
        /// </summary>
        /// <param name="htmlNode"></param>
        public void Add(HtmlNode htmlNode)
        {
            _nodeList.Add(htmlNode);
        }

        /// <summary>
        /// 指定された位置にノードを挿入する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="htmlNode"></param>
        public void Insert(int index,HtmlNode htmlNode)
        {
            _nodeList.Insert(index, htmlNode);
        }

        /// <summary>
        /// リストのサイズ
        /// </summary>
        public int Count
        {
            get { return _nodeList.Count; }
        }

        /// <summary>
        /// インデクサ
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HtmlNode this[int index]
        {
            get { return _nodeList[index]; }
        }

        /// <summary>
        /// "foreach" スタイルの反復処理を行うクラスのインスタンスを返します
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new HtmlNodeEnumerator(_nodeList);
        }

        /// <summary>
        /// HtmlNodeListの反復処理を行います
        /// </summary>
        internal class HtmlNodeEnumerator : IEnumerator
        {
            //現在位置
            private int _pointer = -1;

            //ノードのリスト
            private List<HtmlNode> _nodeList = null;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="nodeList">ノードのリスト</param>
            public HtmlNodeEnumerator(List<HtmlNode> nodeList)
            {
                _nodeList = nodeList;
            }

            /// <summary>
            /// 反復の現在位置のノードを返す
            /// </summary>
            public object Current
            {
                get {  return _nodeList[_pointer]; }
            }

            /// <summary>
            /// 反復の次の位置に移動する
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _pointer++;

                if (_pointer <= _nodeList.Count - 1)
                {
                    return true; 
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// 反復をリセットする
            /// </summary>
            public void Reset()
            {
                _pointer = 0;
            }
        }
    }
}

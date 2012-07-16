using System;
using System.Text;

namespace CreteLib
{
    public class HtmlParser
    {
        private char[] _source = null;
        private bool _ignoreCase;


        private int _index = -1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="htmlText">解析対象のHTML</param>
        public HtmlParser(String html, bool ignoreCase)
        {
            _source = html.ToCharArray();
            _ignoreCase = ignoreCase;
        }

        /// <summary>
        /// HTMLを解析し、ノードのリストを生成する
        /// </summary>
        /// <returns></returns>
        public HtmlNodeList Parse()
        {
            HtmlNodeList nodeList = new HtmlNodeList();

            bool isScript = false;

            //HTMLを終端まで移動していく
            while (!MoveNext())
            {
                //タグの開始の場合
                if (Current == '<')
                {
                    if (IsComment)
                    {
                        MoveToCommentEnd();
                    }
                    else
                    {
                        HtmlNode node = ParseTag();

                        if (node != null)
                        {
                            //Console.WriteLine("<{0}>", node.NodeType == NodeType.EndTag ? "/" +  node.TagName : node.TagName);

                            //foreach (String key in node.Attributes.Keys)
                            //{
                            //    Console.WriteLine("属性:{0}={1}", key, node.Attributes[key]);
                            //}

                            nodeList.Add(node);

                            if (node.NodeType == NodeType.Tag && node.TagName.Equals("script", StringComparison.OrdinalIgnoreCase))
                            {
                                isScript = true;
                            }
                            else if (node.NodeType == NodeType.EndTag && node.TagName.Equals("script", StringComparison.OrdinalIgnoreCase))
                            {
                                isScript = false;
                            }
                        }
                    }
                }
                //それ以外の場合
                else
                {
                    HtmlNode node = ParseText(isScript);

                    if (node != null)
                    {
                        //Console.WriteLine((node as HtmlTextNode).Text);
                        nodeList.Add(node);
                    }
                }
            }

            return nodeList;
        }

        //HTMLの終端に達しているか
        private bool Eof
        {
            get
            {
                return _index >= _source.Length;
            }
        }

        //次の文字に進む
        private bool MoveNext()
        {
            _index++;
            return Eof;            
        }

        //指定された数、移動する
        private bool Move(int peek)
        {
            _index += peek;
            return Eof;
        }

        //現在位置のcharを返す
        private char Current
        {
            get
            {
                if (Eof)
                {
                    throw new IndexOutOfRangeException();
                }

                return _source[_index];
            }
        }

        //現在の位置からpeekの値移動した位置がEofに達しているか
        private bool EofAt(int peek)
        {
            if (_index + peek >= _source.Length - 1)
            {
                return true;
            }

            if (_index + peek < 0)
            {
                return true;
            }

            return false;

        }

        //現在の位置からpeekの値移動した位置の文字を返す
        private char CharAt(int peek)
        {
            if (_index + peek >= _source.Length - 1)
            {
                throw new IndexOutOfRangeException();
            }

            if (_index + peek < 0)
            {
                throw new IndexOutOfRangeException();
            }

            return _source[_index + peek];
        }

        //コメントの終端まで移動する
        //終端が見つかる前にEOFに達した場合,falseを返す
        private bool MoveToCommentEnd()
        {
            while (!MoveNext())
            {
                if (Current == '-')
                {
                    if (!EofAt(1) && !EofAt(2))
                    {
                        if (CharAt(1) == '-' && CharAt(2) == '>')
                        {
                            MoveNext();
                            MoveNext();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //コメントか否かを判定する
        private bool IsComment
        {
            get
            {
                if (!EofAt(1) && !EofAt(2) && !EofAt(3))
                {
                    if (CharAt(1) == '!' && CharAt(2) == '-' && CharAt(3) == '-')
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        //タグを解析し、タグの終端まで移動する
        private HtmlNode ParseTag()
        {
            HtmlNode node = null;

            bool isEndTag = false;

            //終了タグか否かを判定する
            if (!EofAt(1))
            {
                if (CharAt(1) == '/')
                {
                    isEndTag = true;
                    if (MoveNext()) return null;
                }
            }

            //タグ名を取得する
            StringBuilder tagName = new StringBuilder();

            //タグ名の終了位置まで移動する
            while (!MoveNext())
            {
                if (Current == ' ' || Current == '/' || Current == '>')
                {
                    break;
                }

                tagName.Append(Current);
            }

            if (tagName.Length != 0)
            {
                node = new HtmlNode(_ignoreCase);
                node.TagName = tagName.ToString();

                if (isEndTag)
                {
                    node.NodeType = NodeType.EndTag;
                }
                else
                {
                    node.NodeType = NodeType.Tag;
                }

                if (Current == '/')
                {
                    node.NodeType = NodeType.SelfComplete;
                }
            }
            else
            {
                return null;
            }

            //属性が有る場合
            if (!isEndTag && Current != '/' && Current != '>')
            {
                if (node != null)
                {
                    if (!tagName.ToString().Equals("!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                    {
                        //属性を解析する
                        ParseAttribute(node);
                    }
                }
            }

            //タグの終端まで移動
            while(Current != '>')
            {
                if (MoveNext()) return null;
            }

            return node;
        }

        //属性を解析する
        private void ParseAttribute(HtmlNode node)
        {
            while (true)
            {
                bool isName = true; //名前=値形式の属性か否か falseの場合、名前=値形式
                char delimitter = ' ';  //値を区切るデリミタ ' 'を入れているのはコンパイルを通すため
                bool valueStarted = false;  //現在位置が値の中に入っているか
                bool delimitterUsed = false;    //デリミタが使用されているか

                StringBuilder name = new StringBuilder();
                StringBuilder value = new StringBuilder();

                while (!MoveNext())
                {
                    //=が見つかった場合
                    if (Current == '=')
                    {
                        isName = false;
                        break;
                    }
                    else if (Current == '>')
                    {
                        if (name.ToString().Trim().Length != 0)
                        {
                            node.Attributes.Add(name.ToString(), null);
                        }

                        return;
                    }
                    //スペースが見つかった場合
                    else if (Current == ' ' || Current == '/')
                    {
                        //次の文字が=以外の場合(A=B形式の属性で無い場合)
                        if (!EofAt(1) && CharAt(1) != '=')
                        {
                            if (name.ToString().Trim().Length != 0)
                            {
                                node.Attributes.Add(name.ToString(), null);
                            }

                            break;
                        }

                        continue;

                    }
                    // =、またはスペース以外の場合
                    else
                    {
                        name.Append(Current);
                    }
                }

                if (isName) continue;

                while (!MoveNext())
                {
                    //現在位置が値の中に入っていない場合
                    if (!valueStarted)
                    {
                        if (Current == ' ')
                        {
                            continue;
                        }
                        else if (Current == '\'' || Current == '"')
                        {
                            delimitter = Current;
                            delimitterUsed = true;
                            valueStarted = true;
                            //value.Append(Current);
                        }
                        else
                        {
                            value.Append(Current);
                            valueStarted = true;
                        }                        
                    }
                    //現在位置が値の中に入っている場合
                    else
                    {
                        if (delimitterUsed)
                        {
                            if (Current == delimitter)
                            {
                                //value.Append(Current);

                                if (name.Length != 0 && value.Length != 0)
                                {
                                    node.Attributes.Add(name.ToString(), value.ToString());
                                }

                                break;
                            }
                            else
                            {
                                value.Append(Current);
                            }
                        }
                        else
                        {
                            if (Current == ' ' || Current == '/' || Current == '>')
                            {
                                if (name.Length != 0 && value.Length != 0)
                                {
                                    node.Attributes.Add(name.ToString(), value.ToString());
                                }

                                if (Current == '>')
                                {
                                    return;
                                }
                                else if (Current == '/')
                                {
                                    node.NodeType = NodeType.SelfComplete;
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                value.Append(Current);
                            }
                        }

                    }
                }
            }
        }

        //テキストを解析し、テキストの終端まで移動する
        private HtmlNode ParseText(bool isScript)
        {
            HtmlTextNode node = null;

            StringBuilder text = new StringBuilder();

            do
            {
                text.Append(Current);

                if (!EofAt(1))
                {
                    if (CharAt(1) == '<')
                    {
                        if (isScript)
                        {
                            if (!EofAt(2) && !EofAt(3) && !EofAt(4) && !EofAt(5) && !EofAt(6) && !EofAt(7) && !EofAt(8))
                            {
                                if ( CharAt(2) == '/' &&
                                    (CharAt(3) == 's' || CharAt(3) == 'S') &&
                                    (CharAt(4) == 'c' || CharAt(4) == 'C') &&
                                    (CharAt(5) == 'r' || CharAt(5) == 'R') &&
                                    (CharAt(6) == 'i' || CharAt(6) == 'I') &&
                                    (CharAt(7) == 'p' || CharAt(7) == 'P') &&
                                    (CharAt(8) == 't' || CharAt(8) == 'T'))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            while (!MoveNext());

            if (text.Length != 0)
            {
                node = new HtmlTextNode(_ignoreCase);
                node.NodeType = NodeType.Text;
                node.Text = text.ToString();
            }

            return node;
        }
    }
}

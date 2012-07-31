using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CreteLib;

namespace Progressive.Peercast4Net.Utils
{
    class SettingsHtmlParser
    {
        public IList<KeyValuePair<string, string>> Parse(string source)
        {
            var html = new HtmlDocument();
            html.LoadHtml(source, true);
            var nvc = html.GetNodesByTagName("form")[0].GetParameters();
            var cmd = new KeyValuePair<string, string>();
            foreach (var item in nvc)
            {
                if (item.Key == "cmd")
                {
                    cmd = item;
                    break;
                }
            }
            if (cmd.Key == "")
            {
                return null;
            }
            nvc.Remove(cmd);
            return nvc;
        }
    }

    static class HtmlNodeExtension
    {
        static public List<KeyValuePair<string, string>> GetParameters(this HtmlNode form)
        {
            var dictionary = new List<KeyValuePair<string, string>>();
            foreach (HtmlNode node in form.ChildNodes)
            {
                if ("input".EqualsIgnoreCase(node.TagName))
                {
                    dictionary.AddFromInputNode(node);
                    continue;
                }
                if ("select".EqualsIgnoreCase(node.TagName))
                {
                    dictionary.AddFromSelectNode(node);
                    continue;
                }
                dictionary.AddRange(node.GetParameters());
            }
            return dictionary;
        }
    }

    static class ExtensionsOnSettingsHtml
    {
        internal static void AddFromSelectNode(this List<KeyValuePair<string, string>> list, HtmlNode node)
        {
            string value = "";
            foreach (HtmlNode option in node.ChildNodes)
            {
                if (option.Attributes.ContainsKey("selected"))
                {
                    value = option.Attributes["value"];
                    break;
                }
            }
            list.Add(Create(node.Attributes["name"], value));
        }

        internal static void AddFromInputNode(this List<KeyValuePair<string, string>> list, HtmlNode node)
        {
            if (!node.Attributes.ContainsKey("type"))
            {
                list.AddParameter(node);
                return;
            }
            var type = node.Attributes["type"];
            if (!"radio".EqualsIgnoreCase(type) && !"checkbox".EqualsIgnoreCase(type))
            {
                list.AddParameter(node);
                return;
            }
            if (node.Attributes.ContainsKey("checked"))
            {
                list.AddParameter(node);
                return;
            }
        }

        static private void AddParameter(this List<KeyValuePair<string, string>> List, HtmlNode node)
        {
            if (!node.Attributes.ContainsKey("value"))
            {
                List.Add(Create(node.Attributes["name"], ""));
                return;
            }
            List.Add(Create(node.Attributes["name"], node.Attributes["value"].Replace(' ', '+')));
        }

        private static KeyValuePair<string, string> Create(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }
    }

    static class StringExtension
    {
        internal static bool EqualsIgnoreCase(this string source, string target)
        {
            return source.Equals(target, StringComparison.OrdinalIgnoreCase);
        }
    }
}

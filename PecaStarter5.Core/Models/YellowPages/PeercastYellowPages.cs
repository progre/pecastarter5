using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Models
{
    class PeercastYellowPages : YellowPages
    {
        public override string GetPrefix(Dictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append(Header);
            foreach (var component in parameters.Keys)
            {
                sb.Append(GetPrefixParameter(component, parameters[component]));
            }
            return sb.ToString();
        }

        public Tuple<Dictionary<string, string>, string> Parse(string value)
        {
            var dictionary = new Dictionary<string, string>();
            int index = 0;
            if (!Check(value, Header, ref index))
            {
                return Tuple.Create(dictionary, value);
            }
            foreach (var param in Components)
            {
                dictionary[param] = GetParam(value, param, ref index);
            }
            return Tuple.Create(dictionary, value.Substring(index));
        }

        public string Header { get; set; }

        private string GetPrefixParameter(string component, string value)
        {
            switch (component)
            {
                case "namespace":
                    if (String.IsNullOrEmpty(value))
                    {
                        return "";
                    }
                    return value + ':';
                case "port_bandwidth_check":
                    if (string.IsNullOrEmpty(value))
                    {
                        return "";
                    }
                    switch (value)
                    {
                        case "1":
                            return "@";
                        case "2":
                            return "@@";
                        case "3":
                            return "@@@";
                        default:
                            throw new NotSupportedException();
                    }
                case "outside_display":
                    if (string.IsNullOrEmpty(value))
                    {
                        return "";
                    }
                    switch (value)
                    {
                        case "1":
                            return "#";
                        case "2":
                            return "##";
                        default:
                            throw new NotSupportedException();
                    }
                case "listeners_invisibility":
                    if (value != "True")
                    {
                        return "";
                    }
                    return "?";
                case "listeners_visibility":
                    if (value != "True")
                    {
                        return "";
                    }
                    return "?";
                case "no_log":
                    if (value != "True")
                    {
                        return "";
                    }
                    return "+";
                case "time_invisibility":
                    if (value != "True")
                    {
                        return "";
                    }
                    return "~";
                default:
                    return value;
            }
        }

        private string GetParam(string value, string param, ref int index)
        {
            switch (param)
            {
                case "namespace":
                    int end = value.IndexOf(':', index);
                    if (end < 0)
                    {
                        return "";
                    }
                    var result = value.Substring(index, end - index);
                    index = end + 1;
                    return result;
                case "listeners_visibility":
                    if (!Check(value, "?", ref index))
                    {
                        return "False";
                    }
                    return "True";
                case "listeners_invisibility":
                    if (!Check(value, "?", ref index))
                    {
                        return "False";
                    }
                    return "True";
                case "time_invisibility":
                    if (!Check(value, "~", ref index))
                    {
                        return "False";
                    }
                    return "True";
                case "outside_display":
                    if (!Check(value, "#", ref index))
                    {
                        return "0";
                    }
                    if (!Check(value, "#", ref index))
                    {
                        return "1";
                    }
                    return "2";
                case "port_bandwidth_check":
                    if (!Check(value, "@", ref index))
                    {
                        return "0";
                    }
                    if (!Check(value, "@", ref index))
                    {
                        return "1";
                    }
                    if (!Check(value, "@", ref index))
                    {
                        return "2";
                    }
                    return "3";
                case "no_log":
                    if (!Check(value, "+", ref index))
                    {
                        return "False";
                    }
                    return "True";
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// valueの先頭にsignがあるかを返します。signがあった場合はindexをその分だけ移動させます
        /// </summary>
        private bool Check(string value, string sign, ref int index)
        {
            if (!EqualsOnIndex(value, index, sign))
            {
                return false;
            }
            index += sign.Length;
            return true;
        }

        /// <summary>
        /// valueのindex番目がsignか
        /// </summary>
        private bool EqualsOnIndex(string value, int index, string sign)
        {
            return sign == value.Substring(index, sign.Length);
        }
    }
}

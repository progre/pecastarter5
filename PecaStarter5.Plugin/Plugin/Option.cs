using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Plugin
{
    public class Option
    {
        public Option(OptionType optionType)
        {
            OptionType = optionType;
        }

        public OptionType OptionType { get; private set; }
        public object Value { get; set; }
    }

    public enum OptionType
    {
        Directory, File, Boolean
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Plugin;

namespace Progressive.PecaStarter5.Models
{
    public class ExternalPlugin
    {
        public ExternalPlugin(IPlugin plugin)
        {
            Instance = plugin;
        }

        public bool IsEnabled { get; set; }
        public IPlugin Instance { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Plugins;

namespace Progressive.PecaStarter5.Models
{
    public class ExternalPlugin
    {
        public event EventHandler IsEnabledChanged;

        public ExternalPlugin(string assemblyName, IPlugin plugin)
        {
            this.assemblyName = assemblyName;
            this.instance = plugin;
        }

        private readonly string assemblyName;
        public string AssemblyName { get { return assemblyName; } }

        private readonly IPlugin instance;
        public IPlugin Instance { get { return instance; } }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                IsEnabledChanged(this, new EventArgs());
            }
        }
    }
}

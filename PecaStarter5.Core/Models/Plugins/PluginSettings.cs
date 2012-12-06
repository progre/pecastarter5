using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Progressive.PecaStarter5.Models.Plugins
{
    [DataContractAttribute]
    class PluginSettingsList
    {
        [DataMember]
        public Dictionary<string, PluginSettings> PluginSettings { get; set; }
    }

    [DataContractAttribute]
    class PluginSettings
    {
        [DataMember]
        public bool IsEnabled { get; set; }
        [DataMember]
        public Dictionary<string, object> Repository { get; set; }
    }
}

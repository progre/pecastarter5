using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Models.Plugins;

namespace Progressive.PecaStarter5.Models.ExternalYellowPages
{
    public interface IExternalYellowPages : IPlugin
    {
        string Name { get; }
    }
}

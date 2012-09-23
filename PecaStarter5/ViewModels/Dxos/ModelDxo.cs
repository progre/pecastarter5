using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Plugin;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.ViewModels.Dxos
{
    static class ModelDxo
    {
        public static InterruptedParameter ToInterruptedParameter(IChannel channel)
        {
            return new InterruptedParameter()
            {
                Name = channel.Name,
                Genre = channel.Genre,
                Description = channel.Description,
                Contact = channel.ContactUrl,
                Comment = channel.Comment
            };
        }
    }
}

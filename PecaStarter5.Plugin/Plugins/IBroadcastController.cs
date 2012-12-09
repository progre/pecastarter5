using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Plugins
{
    interface IBroadcastController
    {
        void Update(UpdateParameter updateParameter);
        void Stop();
    }
}

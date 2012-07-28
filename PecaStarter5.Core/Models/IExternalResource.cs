using System.Collections.Generic;
using System.IO;

namespace Progressive.PecaStarter5.Models
{
    public interface IExternalResource
    {
        Stream GetConfigurationInputStream();
        Stream GetConfigurationOutputStream();
        IEnumerable<Stream> GetYellowPagesDefineInputStream();
    }
}

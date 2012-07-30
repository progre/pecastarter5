using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net
{
    public class PeercastException : Exception
    {
        public PeercastException()
            : base()
        {
        }

        public PeercastException(string message)
            : base(message)
        {
        }
    }
}

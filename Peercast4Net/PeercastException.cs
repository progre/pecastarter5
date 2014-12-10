using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net
{
    public class PeerCastException : Exception
    {
        public PeerCastException()
            : base()
        {
        }

        public PeerCastException(string message)
            : base(message)
        {
        }
    }
}

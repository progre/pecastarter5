using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Progressive.Peercast4Net.Commons.Dao
{
    class HttpDao
    {
        private static HttpDao instance = new HttpDao();
        public static HttpDao Instance { get { return instance; } }

        private HttpDao()
        {
        }
    }
}

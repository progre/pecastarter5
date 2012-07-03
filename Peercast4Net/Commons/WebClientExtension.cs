using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Progressive.Peercast4Net.Commons
{
    static class WebClientExtension
    {
        /// <exception cref="WebException"></exception>
        public static async Task AccessAsync(this WebClient client, string address)
        {
            using (await client.OpenReadTaskAsync(address)) { }
        }

        public static async Task<string> DownloadAsync(this WebClient client, string address)
        {
            using (var sr = new StreamReader(
                await client.OpenReadTaskAsync(address), Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Codeplex.Data;

namespace Progressive.PecaStarter5.Models
{
    class WebApiYellowPages : YellowPages
    {
        public string BroadcastUrl { get; set; }
        public string UpdateUrl { get; set; }
        public string StopUrl { get; set; }

        public async Task<string> Broadcast(NameValueCollection parameters)
        {
            return await Post(BroadcastUrl, parameters);
        }

        public async Task<string> Update(NameValueCollection parameters)
        {
            return await Post(UpdateUrl, parameters);
        }

        public async Task<string> Stop(NameValueCollection parameters)
        {
            return await Post(StopUrl, parameters);
        }

        private async Task<string> Post(string url, NameValueCollection parameters)
        {
            string response;
            using (var client = new WebClient())
            {
                response = await Task.Factory.StartNew(() => Encoding.UTF8.GetString(client.UploadValues(url, parameters)));
            }
            dynamic json = DynamicJson.Parse(response);
            if (json.result != "successful")
            {
                throw new YellowPagesException(json.message);
            }
            return json.message;
        }
    }

    class YellowPagesException : ApplicationException
    {
        public string Message { get; private set; }
        public YellowPagesException(string message)
        {
            Message = message;
        }
    }
}

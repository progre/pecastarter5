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

        public Task OnBroadcastAsync(NameValueCollection parameters)
        {
            return Post(BroadcastUrl, parameters);
        }

        public Task UpdateAsync(NameValueCollection parameters)
        {
            return Post(UpdateUrl, parameters);
        }

        public Task StopAsync(NameValueCollection parameters)
        {
            return Post(StopUrl, parameters);
        }

        private async Task Post(string url, NameValueCollection parameters)
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

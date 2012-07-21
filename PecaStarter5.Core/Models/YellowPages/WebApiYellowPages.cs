using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Codeplex.Data;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Models.Plugins;

namespace Progressive.PecaStarter5.Models
{
    class WebApiYellowPages : YellowPages, IExternalYellowPages
    {
        public override bool IsExternal { get { return true; } }
        public string BroadcastUrl { get; set; }
        public string UpdateUrl { get; set; }
        public string StopUrl { get; set; }
        public string[] BroadcastParameters { get; set; }
        public string[] UpdateParameters { get; set; }
        public string[] StopParameters { get; set; }

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

        #region IPlugin メンバー

        public Task OnBroadcastedAsync(BroadcastedParameter parameter)
        {
            var nvc = new NameValueCollection();
            foreach (var param in BroadcastParameters)
            {
                nvc.Add(param, GetParameterValue(param, parameter));
            }
            return Post(BroadcastUrl, nvc);
        }

        private string GetParameterValue(string parameterKey, BroadcastedParameter parameter)
        {
            switch (parameterKey)
            {
                case "name":
                    return parameter.BroadcastParameter.Name;
                case "bitrate":
                    return parameter.Bitrate.ToString();
                case "tags":
                    return parameter.BroadcastParameter.Genre;
                case "description":
                    return parameter.BroadcastParameter.Description;
                case "comment":
                    return parameter.BroadcastParameter.Comment;
                case "contact_url":
                    return parameter.BroadcastParameter.ContactUrl;
                case "protocol":
                    return "Peercast";
                case "stream_url":
                    return "http://localhost:7144/pls/" + parameter.Id;
                case "type":
                    return parameter.BroadcastParameter.Type;
                case "password":
                    return parameter.YellowPagesParameters.Single(x => x.Key == "password").Value;
                case "result_format":
                    return "json";
                default:
                    throw new ArgumentException();
            }
        }

        public Task OnUpdatedAsync(UpdateParameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public Task OnStopedAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task OnTickedAsync(string name, int relays, int listeners)
        {
            throw new System.NotImplementedException();
        }

        #endregion
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

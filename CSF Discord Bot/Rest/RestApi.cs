using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CSFCloud.DiscordCore.Rest {

    public abstract class RestApi {

        protected string baseurl = "https://discordapp.com/api/";
        protected string api_name = "";
        protected RestApiMethod method = RestApiMethod.GET;
        public string token;

        public RestApi(string token) {
            this.token = token;
        }

        protected T Execute<T>(NameValueCollection data) {
            string response = "";

            if (method == RestApiMethod.GET) {
                string uri = baseurl + api_name + "?" + ToQueryString(data);

                WebRequest request = WebRequest.Create(uri);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bot " + token);

                WebResponse resp = request.GetResponse();
                Stream st = resp.GetResponseStream();
                StreamReader reader = new StreamReader(st);
                response = reader.ReadToEnd();

            } else if (method == RestApiMethod.POST) {
                string uri = baseurl + api_name;
                string postData = ToQueryString(data);

                WebRequest request = WebRequest.Create(uri);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bot " + token);
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse resp = request.GetResponse();
                Stream st = resp.GetResponseStream();
                StreamReader reader = new StreamReader(st);
                response = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(response);
        }

        private static string ToQueryString(NameValueCollection nvc) {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return string.Join("&", array);
        }

        protected enum RestApiMethod {
            GET, POST
        }
    }

}
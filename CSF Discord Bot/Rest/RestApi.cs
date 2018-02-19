using CSFCloud.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public string token;

        public RestApi(string token) {
            this.token = token;
            Logger.Debug($"RestApi token: {token}");
        }

        protected T Execute<T>(RestApiMethod method, string api_name, NameValueCollection data = null) {
            string response = "";
            if (data == null) {
                data = new NameValueCollection();
            }

            if (method == RestApiMethod.GET) {
                string uri = baseurl + api_name + "?" + ToQueryString(data);

                Logger.Debug($"GET {uri}");

                WebRequest request = WebRequest.Create(uri);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bot " + token);

                WebResponse resp = request.GetResponse();
                Stream st = resp.GetResponseStream();
                StreamReader reader = new StreamReader(st);
                response = reader.ReadToEnd();

            } else {
                string uri = baseurl + api_name;
                string postData = ToJsonString(data);

                Logger.Debug($"POST {uri}");

                WebRequest request = WebRequest.Create(uri);
                request.Method = method.ToString();
                request.Headers.Add("Authorization", "Bot " + token);
                request.ContentType = "application/json";
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

        private static string ToJsonString(NameValueCollection nvc) {
            Dictionary<string, string> dictdata = new Dictionary<string, string>();

            foreach (string key in nvc.AllKeys) {
                dictdata[key] = nvc[key];
            }

            return JsonConvert.SerializeObject(dictdata);
        }

        protected enum RestApiMethod {
            GET, POST, PUT, PATCH, DELETE
        }
    }

}
using CSFCloud.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;

namespace CSFCloud.DiscordCore.Rest
{
    public class MessageCreate : RestApi {

        public MessageCreate(string token, string channel_id) : base(token) {
            api_name = $"channels/{channel_id}/messages";
            method = RestApiMethod.POST;
        }

        public JObject SendMessage(string message) {
            NameValueCollection data = new NameValueCollection() {
                { "content", message }
            };

            JObject j;

            try {
                j = Execute<JObject>(data);
            } catch (Exception e) {
                Logger.Error($"Message sending exception: {e.Message}");
                return null;
            }

            return j;
        }

    }
}

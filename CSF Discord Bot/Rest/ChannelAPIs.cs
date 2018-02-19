using CSFCloud.DiscordCore.Management;
using CSFCloud.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;

namespace CSFCloud.DiscordCore.Rest
{
    public class ChannelAPIs : RestApi {

        private string channel_id;

        public ChannelAPIs(string token, string channel_id) : base(token) {
            this.channel_id = channel_id;
        }

        public Message SendMessage(string message) {
            NameValueCollection data = new NameValueCollection() {
                { "content", message }
            };
            return Execute<Message>(RestApiMethod.POST, $"channels/{channel_id}/messages", data);
        }

        public Invite CreateInvite(int max_age = 86400) {
            NameValueCollection data = new NameValueCollection() {
                { "max_age", $"{max_age}" }
            };
            return Execute<Invite>(RestApiMethod.POST, $"channels/{channel_id}/invites", data);
        }

        public void DeleteMessage(Message message) {
            DeleteMessage(message.id);
        }

        public void DeleteMessage(string messageId) {
            Execute<JObject>(RestApiMethod.DELETE, $"channels/{channel_id}/messages/{messageId}");
        }

        public Message[] GetMessages(int limit = 50) {
            NameValueCollection data = new NameValueCollection() {
                { "limit", $"{limit}" }
            };
            return Execute<Message[]>(RestApiMethod.GET, $"channels/{channel_id}/messages", data);
        }

        public Message EditMessage(Message message, string content) {
            return EditMessage(message.id, content);
        }

        public Message EditMessage(string messageId, string content) {
            NameValueCollection data = new NameValueCollection() {
                { "content", content }
            };
            return Execute<Message>(RestApiMethod.PATCH, $"channels/{channel_id}/messages/{messageId}", data);
        }

        public void StartTyping() {
            Execute<JObject>(RestApiMethod.POST, $"channels/{channel_id}/typing");
        }

        public Message[] GetPinnedMessages() {
            return Execute<Message[]>(RestApiMethod.GET, $"channels/{channel_id}/pins");
        }

        public void PinMessage(Message message) {
            PinMessage(message.id);
        }

        public void PinMessage(string messageId) {
            Execute<JObject>(RestApiMethod.PUT, $"channels/{channel_id}/pins/{messageId}");
        }

        public void UnpinMessage(Message message) {
            UnpinMessage(message.id);
        }

        public void UnpinMessage(string messageId) {
            Execute<JObject>(RestApiMethod.DELETE, $"channels/{channel_id}/pins/{messageId}");
        }

    }
}

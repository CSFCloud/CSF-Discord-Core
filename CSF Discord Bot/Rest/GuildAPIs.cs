using CSFCloud.DiscordCore.Management;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace CSFCloud.DiscordCore.Rest {
    public class GuildAPIs : RestApi {

        private string guild_id;

        public GuildAPIs(string token, string guild_id) : base(token) {
            this.guild_id = guild_id;
        }

        public Emoji[] GetEmojis() {
            return Execute<Emoji[]>(RestApiMethod.GET, $"guilds/{guild_id}/emojis");
        }

        public Emoji CreateEmoji(string name, string base64_image) {
            NameValueCollection data = new NameValueCollection() {
                { "name", name },
                { "image", base64_image }
            };
            return Execute<Emoji>(RestApiMethod.POST, $"guilds/{guild_id}/emojis", data);
        }

        public void DeleteEmoji(Emoji emoji) {
            DeleteEmoji(emoji.id);
        }

        public void DeleteEmoji(string emojiId) {
            Execute<JObject>(RestApiMethod.DELETE, $"guilds/{guild_id}/emojis/{emojiId}");
        }

        public void LeaveGuild() {
            Execute<JObject>(RestApiMethod.DELETE, $"users/@me/guilds/{guild_id}");
        }

    }
}

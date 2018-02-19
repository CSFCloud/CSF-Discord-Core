using CSFCloud.DiscordCore.Rest;

namespace CSFCloud.DiscordCore.Management {

    public class Guild : IToken {

        public int verification_level;
        public bool unavailable;
        public string system_channel_id;
        public Role[] roles;
        public string region;
        public string owner_id;
        public string name;
        public int mfa_level;
        public Member[] members;
        public bool large;
        public string joined_at;
        public string id;
        public string icon;
        public Channel[] channels;

        private string token;
        public GuildAPIs API;

        public void SetToken(string token) {
            this.token = token;
            API = new GuildAPIs(token, id);
        }
    }

}

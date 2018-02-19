using CSFCloud.DiscordCore.Rest;

namespace CSFCloud.DiscordCore.Management {

    public class Channel : IToken {

        public int user_limit;
        public ChannelType type;
        public int position;
        public string name;
        public string id;
        public int bitrate;

        private string token;
        public ChannelAPIs API = null;

        public void SetToken(string token) {
            this.token = token;
            API = new ChannelAPIs(token, id);
        }
    }

}
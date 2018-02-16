using DiscordCore.Management;

namespace DiscordCore.Processors {
    public abstract class Processor {

        protected string token;
        protected Client client;

        public void SetToken(string token) {
            this.token = token;
        }

        public void SetClient(Client client) {
            this.client = client;
        }

        public virtual string GetGameName() {
            return null;
        }

        public virtual string GetPrefix() {
            return null;
        }

        public virtual void OnReady(string username) { }
        public virtual void OnMessage(Message message, Channel channel) { }
        public virtual void OnCommand(Message message, Channel channel, string command, string[] args) { }
        public virtual void OnTypingStarted(string channelId, string userId) { }
        public virtual void OnStatusChange(string userId, string newStatus, string gameName) { }
        public virtual void OnGuildCreate(Guild guild) { }
        public virtual void OnGuildDelete(string guildId) { }
        public virtual void OnUserVoiceStatusChange(string userId, UserVoiceStatus status) { }

    }
}

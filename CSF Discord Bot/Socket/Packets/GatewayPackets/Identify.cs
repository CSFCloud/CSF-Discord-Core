using System.Collections.Generic;

namespace DiscordCore.Socket.Packets.GatewayPackets {

    internal class Identify : GatewayPacket {

        public Identify(string token, string gameName = null, int shardId = 0, int shardNumber = 1) : base(GatewayPacketType.Identify) {
            IdentityData id = new IdentityData() {
                token = token,
                shard = new int[] { shardId, shardNumber }
            };
            if (gameName != null) {
                id.presence.game = new IdentityGame() {
                    name = gameName
                };
            }

            this.data = id;
        }

        private class IdentityData {
            public string token;
            public Dictionary<string, string> properties = new Dictionary<string, string> {
                ["$os"] = "linux",
                ["$browser"] = "CSFCloud",
                ["$device"] = "CSFCloud"
            };
            public bool compress = false;
            public int large_threshold = 250;
            public int[] shard = new int[2] { 0, 1 };
            public IdentityPresence presence = new IdentityPresence();
        }

        public class IdentityPresence {
            public IdentityGame game = null;
            public string status = "online";
            public bool afk = false;
        }

        public class IdentityGame {
            public string name = null;
            public int type = 0;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Socket.Packets.VoicePackets {

    class Identify : VoicePacket {

        public Identify(string serverId, string sessionId, string token, string userId) : base(VoicePacketType.Identify) {
            IdentityData id = new IdentityData() {
                server_id = serverId,
                session_id = sessionId,
                token = token,
                user_id = userId
            };

            this.data = id;
        }

        private class IdentityData {

            public string server_id;
            public string session_id;
            public string token;
            public string user_id;
            public bool video = false;

        }

    }

}

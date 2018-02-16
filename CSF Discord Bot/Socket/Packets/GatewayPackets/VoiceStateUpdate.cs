using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCore.Socket.Packets.GatewayPackets {

    class VoiceStateUpdate : GatewayPacket {

        public VoiceStateUpdate(string guildId = null, string channelId = null) : base(GatewayPacketType.VoiceStateUpdate) {
            StateData sd = new StateData() {
                channel_id = channelId,
                guild_id = guildId
            };

            this.data = sd;
        }

        private class StateData {
            public string channel_id = null;
            public string guild_id = null;
            public bool self_deaf = false;
            public bool self_mute = false;
            public bool self_video = false;
        }

    }

}

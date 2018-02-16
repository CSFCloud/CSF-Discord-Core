using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCore.Socket.Packets.VoicePackets {

    class HeartBeat : VoicePacket {

        public HeartBeat() : base(VoicePacketType.Heartbeat) { }

    }

}

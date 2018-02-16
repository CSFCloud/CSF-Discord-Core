using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Socket.Packets.VoicePackets
{
    class VoicePacket : BasicPacket {

        public VoicePacket(VoicePacketType type) : base((int)type) { }

        public VoicePacket(string s) : base(s) { }

        public VoicePacketType GetPacketType() {
            return (VoicePacketType)type;
        }

    }
}

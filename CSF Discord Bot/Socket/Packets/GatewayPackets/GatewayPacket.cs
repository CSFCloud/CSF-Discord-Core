using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Socket.Packets.GatewayPackets {
    internal class GatewayPacket : BasicPacket {

        public GatewayPacket(GatewayPacketType type) : base((int)type) { }

        public GatewayPacket(string s) : base(s) { }

        public GatewayPacketType GetPacketType() {
            return (GatewayPacketType)type;
        }

    }
}

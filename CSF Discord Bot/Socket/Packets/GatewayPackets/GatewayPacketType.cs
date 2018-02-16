using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Socket.Packets.GatewayPackets {
    internal enum GatewayPacketType {

        Dispatch = 0,
        Heartbeat = 1,
        Identify = 2,
        StatusUpdate = 3,
        VoiceStateUpdate = 4,
        VoiceServerPing = 5,
        Resume = 6,
        Reconnect = 7,
        RequestGuildMembers = 8,
        InvalidSession = 9,
        Hello = 10,
        HeartbeatACK = 11

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Socket.Packets.VoicePackets {
    class SelectProtocol : VoicePacket {

        public SelectProtocol(string address, int port) : base(VoicePacketType.SelectProtocol) {
            ProtocolSelectorData psd = new ProtocolSelectorData() {
                address = address,
                port = port
            };

            this.data = psd;
        }

        private class ProtocolSelectorData {
            public string address;
            public int port;
            public string mode = "xsalsa20_poly1305";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Socket.Packets.GatewayPackets {

    internal class StatusUpdate : GatewayPacket {

        public StatusUpdate(Status status = Status.online, string game = null) : base(GatewayPacketType.StatusUpdate) {
            StatusData d = new StatusData() {
                status = status.ToString()
            };
            if (game != null) {
                d.game = new StatusGame() {
                    name = game
                };
            }

            this.data = d;
        }

        private class StatusData {
            public string status = "online";
            public bool afk = false;
            public int since = 0;
            public StatusGame game = null;
        }

        private class StatusGame {
            public string name = null;
            public int type = 0;
        }

        public enum Status {
            online, offline, idle, dnd
        }

    }

}

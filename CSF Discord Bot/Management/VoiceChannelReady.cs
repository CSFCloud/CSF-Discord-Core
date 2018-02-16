using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCore.Management {

    public class VoiceChannelReady {

        public int ssrc;
        public int port;
        public string[] modes;
        public string ip;
        public int heartbeat_interval;

    }

}

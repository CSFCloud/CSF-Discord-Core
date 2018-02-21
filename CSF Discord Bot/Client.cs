using CSFCloud.DiscordCore.Processors;
using CSFCloud.DiscordCore.Rest;
using CSFCloud.DiscordCore.Socket;
using CSFCloud.DiscordCore.Socket.Packets;
using CSFCloud.DiscordCore.Socket.Packets.GatewayPackets;
using CSFCloud.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSFCloud.DiscordCore {
    public class Client {

        private string token;
        private List<Processor> processors = new List<Processor>();
        private List<MainSocket> shards = new List<MainSocket>();
        private bool started = false;

        public Client(string token = null) {
            if (token != null) {
                this.token = token;
            } else {
                this.token = Environment.GetEnvironmentVariable("BOT_TOKEN");
            }
        }

        public void AddProcessor(Processor p) {
            processors.Add(p);
        }

        public async void Start() {
            Logger.Warning("The CSF Discord Bot Core is a really unstable early version!");
            GatewayGetter gg = new GatewayGetter(token);
            try {
                Logger.Info("Requesting Gateway data...");
                GatewayGetter.GatewayResponse response = gg.GetGateway();
                Uri u = new Uri(response.url + "?format=json");

                int shard_count = response.shards;

                for (int i = 0; i < shard_count; i++) {
                    MainSocket socket = new MainSocket(u, token, i, shard_count);
                    foreach (Processor p in processors) {
                        p.SetToken(token);
                        p.SetClient(this);
                        socket.AddProcessor(p);
                    }
                    shards.Add(socket);
                }

                foreach (MainSocket ms in shards) {
                    await ms.Connect();
                }

                started = true;
            } catch (Exception e) {
                Logger.Error($"Server startup failed: {e.Message}");
            }
        }

        public int GetServerCount() {
            int count = 0;

            foreach (MainSocket ms in shards) {
                count += ms.GetGuildCount();
            }

            return count;
        }

        public void Stop() {
            foreach (MainSocket ms in shards) {
                ms.Disconnect();
            }
        }

        public void ConnectToChannel(string guildId, string channelId) {
            BasicPacket packet = new VoiceStateUpdate(guildId, channelId);
            foreach (MainSocket ms in shards) {
                ms.Send(packet);
            }
        }

        public bool IsEverythingOK() {
            if (!started) {
                return true;
            }
            foreach (MainSocket ms in shards) {
                if (!ms.IsOk()) {
                    return false;
                }
            }
            return true;
        }

    }
}

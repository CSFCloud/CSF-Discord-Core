﻿using DiscordCore.Processors;
using DiscordCore.Rest;
using DiscordCore.Socket;
using DiscordCore.Socket.Packets;
using DiscordCore.Socket.Packets.GatewayPackets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordCore {
    public class Client {

        private string token;
        private List<Processor> processors = new List<Processor>();
        private List<MainSocket> shards = new List<MainSocket>();

        public Client(string token) {
            this.token = token;
        }

        public void AddProcessor(Processor p) {
            processors.Add(p);
        }

        public async void Start() {
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

        public async Task ConnectToChannel(string guildId, string channelId) {
            BasicPacket packet = new VoiceStateUpdate(guildId, channelId);
            foreach (MainSocket ms in shards) {
                await ms.Send(packet);
            }
        }

    }
}
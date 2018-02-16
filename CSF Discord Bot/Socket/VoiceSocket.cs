﻿using CSFCloud.DiscordCore.Audio;
using CSFCloud.DiscordCore.Loopers;
using CSFCloud.DiscordCore.Management;
using CSFCloud.DiscordCore.Socket.Packets;
using CSFCloud.DiscordCore.Socket.Packets.VoicePackets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSFCloud.DiscordCore.Socket {

    internal class VoiceSocket : DiscordSocket {

        private string serverId;
        private string sessionId;
        private string token;
        private string userId;
        private VoiceChannelReady readyData;

        private AudioStream stream;

        public VoiceSocket(Uri uri, string serverId, string sessionId, string token, string userId) : base(uri) {
            this.serverId = serverId;
            this.sessionId = sessionId;
            this.token = token;
            this.userId = userId;
        }

        protected override async void PacketRecieved(string packetstr) {
            VoicePacket packet = new VoicePacket(packetstr);
            VoicePacketType type = packet.GetPacketType();

            if (type == VoicePacketType.Hello) {
                Logger.Info("Voice server says hello!");
                await SendIdentityPacket();
            } else if (type == VoicePacketType.Ready) {
                readyData = packet.GetData<VoiceChannelReady>();

                try {
                    stream = new AudioStream(readyData);
                    stream.Connect();

                    BasicPacket spacket = new SelectProtocol(readyData.ip, readyData.port);
                    await Send(spacket);
                } catch (Exception e) {
                    Logger.Error($"AudioStream connection error: {e.Message}");
                    Disconnect();
                }
            } else if (type == VoicePacketType.ClientDisconnect) {
                Disconnect();
            } else if (type == VoicePacketType.CodecInformation) {
                CodecInformation codecinfo = packet.GetData<CodecInformation>();
                Logger.Info($"Audio codec is {codecinfo.audio_codec}");
                stream.SetCodec(codecinfo.audio_codec);
            } else {
                Logger.Debug($"Unchecked type: {type}");
            }
        }

        protected override async Task SendHeartBeat() {
            BasicPacket packet = new HeartBeat();
            await Send(packet);
        }

        public void ChangeChannel(string channelID) {

        }

        private async Task SendIdentityPacket() {
            BasicPacket packet = new Identify(serverId, sessionId, token, userId);
            await Send(packet);
        }

    }

}

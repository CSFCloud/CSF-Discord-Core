﻿using CSFCloud.DiscordCore.Socket.Packets;
using CSFCloud.Utils;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSFCloud.DiscordCore.Socket {
    internal abstract class DiscordSocket {

        private ClientWebSocket client;
        private Uri serverUri;
        private List<Looper> loops = new List<Looper>();
        private List<BasicPacket> sendBuffer = new List<BasicPacket>();
        protected int HeartBeatInterval = 30000;

        public DiscordSocket(Uri url) {
            serverUri = url;
        }

        public async Task Connect() {
            client = new ClientWebSocket();

            ClearLoops();

            Logger.Info($"Connecting to {serverUri}");
            try {
                await client.ConnectAsync(serverUri, CancellationToken.None);
            } catch (Exception e) {
                Logger.Error($"Connection error: {e.Message}");
                return;
            }
            Logger.Info($"Connected!");

            Looper l1 = new Looper(0, 10);
            l1.SetLoopFunction(ListenerLoop);
            loops.Add(l1);

            Looper l2 = new Looper(HeartBeatInterval, HeartBeatInterval);
            l2.SetLoopAction(SendHeartBeat);
            loops.Add(l2);

            foreach (Looper loop in loops) {
                loop.Start();
            }
        }

        public void Disconnect() {
            ClearLoops();
            try {
                client.Abort();
            } catch {}
        }

        public async Task Reconnect() {
            Disconnect();
            await Connect();
        }

        private void ClearLoops() {
            foreach (Looper loop in loops) {
                loop.Stop();
            }

            loops.Clear();
        }

        private async Task RealSend(BasicPacket packet) {
            if (client.State != WebSocketState.Open) {
                Logger.Error($"[Send] Invalid socket state: {client.State}");
                Disconnect();
                return;
            }

            string str = packet.Serialize();
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);
            await client.SendAsync(bufferSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            Logger.Debug($"Packet sent {str}");
        }

        public void Send(BasicPacket packet) {
            sendBuffer.Add(packet);
        }

        public async Task<string> Receive() {
            if (client.State != WebSocketState.Open) {
                Logger.Error($"[Receive] Invalid socket state: {client.State}");
                Disconnect();
                return null;
            }

            int l = sendBuffer.Count;
            for (int i = 0; i < l; i++) {
                await RealSend(sendBuffer[0]);
                sendBuffer.RemoveAt(0);
            }

            string data = "";
            WebSocketReceiveResult result;

            Logger.Debug($"Receiving...");

            do {
                var buffer = new byte[1024 * 32];
                try {
                    result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                } catch (Exception e) {
                    Logger.Error($"[Receive] ReceiveAsync error: {e.Message} {e.Source}");
                    Disconnect();
                    return null;
                }
                data += Encoding.UTF8.GetString(buffer, 0, result.Count);
            } while (!result.EndOfMessage && result.Count > 0);

            if (data.Length == 0) {
                Logger.Debug($"No data received");
                return null;
            }

            Logger.Debug($"Packet received {data}");

            return data;
        }

        private async Task ListenerLoop() {
            string data = await Receive();
            if (data != null) {
                PacketRecieved(data);
            }
        }

        public bool IsOk() {
            return (client.State == WebSocketState.Open) || (client.State == WebSocketState.Connecting);
        }

        protected abstract void SendHeartBeat();

        protected abstract void PacketRecieved(string packetstr);

    }
}

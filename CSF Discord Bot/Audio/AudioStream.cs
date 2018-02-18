using CSFCloud.DiscordCore.Management;
using CSFCloud.DiscordCore.Socket.Packets;
using CSFCloud.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSFCloud.DiscordCore.Audio {

    internal class AudioStream {

        private string ip;
        private int port;
        private UdpClient socket;
        private List<Looper> loops = new List<Looper>();
        private string audioCodec = "opus";

        public AudioStream(VoiceChannelReady readyData) {
            ip = readyData.ip;
            port = readyData.port;
        }

        public void Connect() {
            Logger.Info($"Connecting to Audio Stream... [{ip}:{port}]");
            socket = new UdpClient();
            socket.Connect(ip, port);
            Logger.Info($"Audio stream connected [{socket.Client.SocketType}]");

            Looper l1 = new Looper(0, 10);
            l1.SetLoopFunction(ListenerLoop);
            loops.Add(l1);

            foreach (Looper l in loops) {
                l.Start();
            }
        }

        ~AudioStream() {
            foreach (Looper l in loops) {
                l.Stop();
            }
        }

        private async Task SendPacket(BasicPacket packet) {
            Logger.Debug("Sending packet...");
            string packetstr = packet.Serialize();
            byte[] buffer = Encoding.UTF8.GetBytes(packetstr);
            await socket.SendAsync(buffer, buffer.Length);
            Logger.Debug($"Packet sent: {packetstr}");
        }

        private async Task ListenerLoop() {
            Logger.Debug("Udp Receiving...");

            UdpReceiveResult res = await socket.ReceiveAsync();
            byte[] buffer = res.Buffer;
            string strres = Encoding.UTF8.GetString(buffer);

            Logger.Debug($"Udp Data: {strres}");
        }

        public void SetCodec(string c) {
            audioCodec = c;
        }

        /*public async void SendProtocolSelection() {
            SelectProtocol packet = new SelectProtocol(ip, port);
            await SendPacket(packet);
        }*/

    }

}

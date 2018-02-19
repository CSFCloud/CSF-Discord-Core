using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSFCloud.DiscordCore.Management;
using CSFCloud.DiscordCore.Processors;
using CSFCloud.DiscordCore.Socket.Packets;
using CSFCloud.DiscordCore.Socket.Packets.GatewayPackets;
using CSFCloud.Utils;
using Newtonsoft.Json.Linq;

namespace CSFCloud.DiscordCore.Socket {

    internal class MainSocket : DiscordSocket {

        private int shardId;
        private int shardCount;
        private string token;
        private string sessionId;
        private string botUserId;
        private List<Processor> processors = new List<Processor>();
        private List<Guild> guilds = new List<Guild>();
        private Dictionary<string, UserVoiceStatus> userStatus = new Dictionary<string, UserVoiceStatus>();
        private Dictionary<string, VoiceSocket> voiceSockets = new Dictionary<string, VoiceSocket>();

        public MainSocket(Uri uri, string token, int shardId, int shardCount) : base(uri) {
            this.shardId = shardId;
            this.shardCount = shardCount;
            this.token = token;
        }

        public void AddProcessor(Processor p) {
            Logger.Debug("Processor added");
            processors.Add(p);
        }

        protected override async void PacketRecieved(string packetstr) {
            GatewayPacket packet = new GatewayPacket(packetstr);
            GatewayPacketType type = packet.GetPacketType();

            if (type == GatewayPacketType.Hello) {
                Logger.Info("Server says hello!");
                SendIdentityPacket();
            } else if (type == GatewayPacketType.Heartbeat) {
                SendHeartBeat();
            } else if (type == GatewayPacketType.HeartbeatACK) {

            } else if (type == GatewayPacketType.InvalidSession) {
                Logger.Error("Invalid session");
                Disconnect();
            } else if (type == GatewayPacketType.Reconnect) {
                await Reconnect();
            } else if (type == GatewayPacketType.Dispatch) {
                string EventType = packet.GetEventType();
                JObject data = packet.GetData<JObject>();

                if (EventType == "READY") {
                    string bot_name = (string)data["user"]["username"];

                    botUserId = (string)data["user"]["id"];
                    sessionId = (string)data["session_id"];

                    foreach (Processor p in processors) {
                        p.OnReady(bot_name);
                    }
                } else if (EventType == "MESSAGE_CREATE") {
                    Message message = packet.GetData<Message>();
                    (Guild guild, Channel channel) = GetGuildAndChannel(message.channel_id);

                    string[] words = SplitString(message.content);

                    Logger.Info($"New message [{message.author.username} in {guild.name} -> {channel.name}] {message.content}");

                    if (!message.author.bot) {
                        foreach (Processor p in processors) {
                            string prefix = p.GetPrefix();

                            if (prefix != null) {
                                if (words.Length >= 2 && prefix == words[0]) {
                                    List<string> w2 = new List<string>(words);

                                    Logger.Debug("Command detected!");

                                    w2.RemoveAt(0);
                                    string command = w2[0];
                                    w2.RemoveAt(0);
                                    p.OnCommand(message, channel, command, w2.ToArray());
                                }
                            }

                            p.OnMessage(message, channel);
                        }
                    } else {
                        Logger.Info("Bot messeges are ignored");
                    }
                } else if (EventType == "TYPING_START") {
                    string channel_id = (string)data["channel_id"];
                    string user_id = (string)data["user_id"];

                    foreach (Processor p in processors) {
                        p.OnTypingStarted(channel_id, user_id);
                    }
                } else if (EventType == "PRESENCE_UPDATE") {
                    string status = (string)data["status"];
                    string game = null;
                    try {
                        game = (string)data["game"]["name"];
                    } catch { }
                    string user_id = (string)data["user"]["id"];

                    foreach (Processor p in processors) {
                        p.OnStatusChange(user_id, status, game);
                    }
                } else if (EventType == "GUILD_CREATE") {
                    Guild guild = packet.GetData<Guild>();
                    guilds.Add(guild);

                    Logger.Info($"Guild created: {guild.name} #{guild.id}");

                    foreach (Processor p in processors) {
                        p.OnGuildCreate(guild);
                    }
                } else if (EventType == "GUILD_DELETE") {
                    string guild_id = (string)data["id"];

                    Logger.Info($"Guild removed: #{guild_id}");

                    for (int i = guilds.Count - 1; i >= 0; i--) {
                        if (guilds[i].id == guild_id) {
                            guilds.RemoveAt(i);
                        }
                    }

                    foreach (Processor p in processors) {
                        p.OnGuildDelete(guild_id);
                    }
                } else if (EventType == "VOICE_STATE_UPDATE") {
                    string user_id = (string)data["user_id"];

                    userStatus[user_id] = packet.GetData<UserVoiceStatus>();

                    if (user_id != botUserId) {
                        foreach (Processor p in processors) {
                            p.OnUserVoiceStatusChange(user_id, userStatus[user_id]);
                        }
                    }
                } else if (EventType == "VOICE_SERVER_UPDATE") {
                    string guildId = (string)data["guild_id"];
                    string endpoint = (string)data["endpoint"];
                    string token = (string)data["token"];

                    if (endpoint != null) {
                        if (!voiceSockets.ContainsKey(guildId) || !voiceSockets[guildId].IsOk()) {
                            string prefix = "wss://";
                            if (endpoint.Contains(":80")) {
                                prefix = "ws://";
                            }
                            voiceSockets[guildId] = new VoiceSocket(new Uri(prefix + endpoint), guildId, sessionId, token, botUserId);
                            await voiceSockets[guildId].Connect();
                        }
                    }
                } else {
                    Logger.Debug($"Unknown event: {EventType}");
                }
            }
        }

        public Channel GetChannel(string channel_id) {
            (Guild g, Channel c) = GetGuildAndChannel(channel_id);
            return c;
        }

        public (Guild, Channel) GetGuildAndChannel(string channel_id) {
            Guild g = new Guild();
            Channel c = new Channel() {
                id = channel_id,
                type = ChannelType.DM
            };

            foreach (Guild guild in guilds) {
                foreach (Channel channel in guild.channels) {
                    if (channel.id == channel_id) {
                        g = guild;
                        c = channel;
                    }
                }
            }

            g.SetToken(token);
            c.SetToken(token);

            return (g, c);
        }

        private string[] SplitString(string str) {
            string[] words = str.Split(new string[] { " " }, StringSplitOptions.None);
            return words.ToArray();
        }

        private void SendIdentityPacket() {
            string game = null;
            foreach (Processor p in processors) {
                string t = p.GetGameName();
                if (t != null) {
                    game = t;
                }
            }

            BasicPacket packet = new Identify(this.token, game, shardId, shardCount);
            Send(packet);
        }

        public int GetGuildCount() {
            return guilds.Count;
        }

        protected override void SendHeartBeat() {
            BasicPacket packet = new HeartBeat();
            Send(packet);
        }

    }

}

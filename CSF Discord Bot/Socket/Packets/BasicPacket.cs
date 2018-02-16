using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordCore.Socket.Packets {

    internal abstract class BasicPacket {

        protected int type;
        private static int SequenceNumber = 0;
        protected object data = null;
        protected string eventName = null;

        public BasicPacket(int type) {
            this.type = type;
        }

        public BasicPacket(string str) {
            Payload p = JsonConvert.DeserializeObject<Payload>(str);
            type = (int)p.op;
            data = p.d;
            eventName = (string)p.t;
        }

        public string Serialize() {
            string s = ToString();
            SequenceNumber++;
            return s;
        }

        public override string ToString() {
            Payload p = new Payload() {
                op = (int)type,
                d = data,
                t = eventName
            };

            return JsonConvert.SerializeObject(p, Formatting.None);
        }

        private class Payload {
            public int op;
            public object d = null;
            public object s = SequenceNumber;
            public object t = null;
        }

        public string GetEventType() {
            return eventName;
        }

        public T GetData<T>() {
            if (typeof(T) == typeof(JObject)) {
                return (T) data;
            } else {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
            }
        }

    }

}

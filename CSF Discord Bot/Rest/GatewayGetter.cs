using System.Collections.Specialized;

namespace CSFCloud.DiscordCore.Rest {
    public class GatewayGetter : RestApi{

        public GatewayGetter(string token) : base(token) {}

        public GatewayResponse GetGateway() {
            NameValueCollection data = new NameValueCollection();
            return Execute<GatewayResponse>(RestApiMethod.GET, "gateway/bot", data);
        }

        public class GatewayResponse {
            public string url;
            public int shards;
        }

    }

}

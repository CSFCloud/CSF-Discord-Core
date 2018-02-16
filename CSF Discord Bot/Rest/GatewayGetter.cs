using System.Collections.Specialized;

namespace CSFCloud.DiscordCore.Rest {
    public class GatewayGetter : RestApi{

        public GatewayGetter(string token) : base(token) {
            api_name = "gateway/bot";
            method = RestApiMethod.GET;
        }

        public GatewayResponse GetGateway() {
            NameValueCollection data = new NameValueCollection();
            return Execute<GatewayResponse>(data);
        }

        public class GatewayResponse {
            public string url;
            public int shards;
        }

    }

}

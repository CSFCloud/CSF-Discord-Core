using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.DiscordCore.Management {

    public class Emoji {

        public string id;
        public string name;
        public string[] roles;
        public User user;
        public bool require_colons = false;
        public bool managed = false;
        public bool animated = false;

    }

}

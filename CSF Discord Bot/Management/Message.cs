using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCore.Management {

    public class Message {

        public MessageType type;
        public bool tts;
        public string timestamp;
        public bool pinned;
        public string nonce;
        // mentions
        // mention_roles
        public bool mention_everyone;
        public string id;
        // embeds
        public string edited_timestamp;
        public string content;
        public string channel_id;
        public User author;
        // attachments

    }

}

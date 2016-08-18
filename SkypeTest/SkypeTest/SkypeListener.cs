using com.samczsun.skype4j.chat;
using com.samczsun.skype4j.events;
using com.samczsun.skype4j.events.chat.message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkypeTest
{
    public class SkypeListener : Listener
    { 
        public void OnMessage(MessageReceivedEvent e)
        {
            var msg = e.getMessage();
            var from = msg.getSender();
            var txt = msg.getContent();

            var chat = msg.getChat();
            string cap = "";
            var ndividualChat = chat as IndividualChat;
            if (ndividualChat != null) cap = ndividualChat.getPartner().getUsername();

            var groupChat = chat as GroupChat;
            if (groupChat != null)
            {
                cap = groupChat.getTopic();
                if (string.IsNullOrEmpty(cap)) cap = "...list of users here";
            }

            Console.WriteLine("From: {0} To:{1} Text:{2}", from.getUsername(), cap, txt);
        }

    }
}

using com.samczsun.skype4j;
using com.samczsun.skype4j.@internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkypeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string username = "giggas3";
            string password = "nelly418390";

            var skypeBuilder = new SkypeBuilder(username, password).withAllResources();
            Skype skype = skypeBuilder.build();
            skype.login();
            skype.getEventDispatcher().registerListener(new SkypeListener());
            skype.subscribe();
            //send message
            var chat = ChatImpl.createChat(skype, "8:" + "MyFriendSharikoff") as ChatIndividual;
            chat.sendMessage(com.samczsun.skype4j.formatting.Message.fromHtml("Hello my dear friend. Bye-bye!"));

            //do whatever you want
            //грабь корованы

            skype.logout();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SKYPE4COMLib;
using System.Windows.Forms;


namespace Parser
{
    class SkypeSender
    {
        private Skype skype;

        public SkypeSender()
        {   
            skype = new Skype();
           if (!skype.Client.IsRunning)
            {
              
              skype.Client.Start(true, true);
              MessageBox.Show("Войдите в скайп");
           }

           skype.Attach();
        }

        public void Send(string message,string to)
        {
            skype.SendMessage(to, message);
        }

    
    }
}

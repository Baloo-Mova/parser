using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class Mails
    {
        public string mail { get; set; }
        public string mpas { get; set; }
        public string smtp { get; set; }
        public int port { get; set; }

        public Mails(string mail, string pas, string smtp = "", int port = 0)
        {
            this.mail = mail;
            this.mpas = pas;
            this.smtp = smtp;
            this.port = port;
        }
        public Mails()
        {

        }
    }
}

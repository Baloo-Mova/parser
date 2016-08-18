using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public static class MailSender
    {

        public static List<SmtpClient> clients = new List<SmtpClient>();

        public static void Initialize(List<Mails> mails)
        {
            foreach (Mails mail in mails)
            {
                SmtpClient Smtp = new SmtpClient();

                if (mail.smtp != "" && mail.port != 0)
                {
                    Smtp = new SmtpClient(mail.smtp, mail.port);
                }
                else if (mail.mail.Contains("gmail.com"))
                {
                    Smtp = new SmtpClient("smtp.gmail.com", 25);
                }
                else if (mail.mail.Contains("mail.ru") || mail.mail.Contains("bk.ru") || mail.mail.Contains("inbox.ru") || mail.mail.Contains("list.ru") || mail.mail.Contains("mail.ua"))
                {
                    Smtp = new SmtpClient("smtp.mail.ru", 25);
                }
                else if (mail.mail.Contains("yandex.ru"))
                {
                    Smtp = new SmtpClient("smtp.yandex.ru", 25);
                    
                }
                else if (mail.mail.Contains("rambler.ru") || mail.mail.Contains("lenta.ru") || mail.mail.Contains("autorambler.ru") || mail.mail.Contains("myrambler.ru") || mail.mail.Contains("ro.ru"))
                {
                    Smtp = new SmtpClient("smtp.rambler.ru ", 465);
                }

                Smtp.Credentials = new NetworkCredential(mail.mail, mail.mpas);
                Smtp.EnableSsl = true;
               

                clients.Add(Smtp);
            }
        }

        static Random rand = new Random((int) DateTime.Now.Ticks); 

        public static SmtpClient getSmtp(){
            return clients[rand.Next(0, clients.Count-1)];
        }

        public static void SendMail(string to,string title, string body,string [] attachments = null)
        {

            SmtpClient client = getSmtp();
            var t = client.Credentials.GetCredential("", 25, "");
                MailMessage Message = new MailMessage();
                Message.From = new MailAddress(t.UserName);
                Message.To.Add(new MailAddress(to));
                Message.Subject = title;
                Message.Body = body;
                if (attachments != null)
                {
                    foreach(string s in attachments){
                        Message.Attachments.Add(new Attachment(s));
                    }
                }

                try
                {
                    client.Send(Message);
                }
                catch (Exception e )
                {
                   
                }
        }
    }
}

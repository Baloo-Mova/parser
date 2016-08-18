using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WhatsAppApi;
using WhatsAppApi.Account;

namespace Parser
{
    public partial class Send : Form
    {
        Account acc;
        public string[] attach;
        public Send()
        {
            InitializeComponent();
        }
        statisticEntities1 entity;
        users user;
        statistica s;
        public Send(Account acc,users user,WhatsApp wats)
        {
            this.acc = acc;
            this.user = user;

            entity = new statisticEntities1();

            if (s == null)
            {
                var index = entity.statistica.Where(et => et.userid == user.Id&&et.request=="").ToList();

                foreach (statistica stat in index)
                {
                    if (stat.data == DateTime.Now.Date)
                    {
                        s = stat;
                    }
                }

                if (s == null)
                {
                    s = new statistica()
                    {
                        userid = user.Id,
                        data = DateTime.Now,
                        request = ""
                    };
                    entity.statistica.Add(s);
                    entity.SaveChanges();
                }
            }

            //try
            //{
            //    send = new ICQSender(acc.icq, acc.icqm);
            //}
            //catch
            //{
            //    MessageBox.Show("Заполните поле ICQ в контактах!!!");
            //}
            wa = wats;
            InitializeComponent();
        }

        private void Send_Load(object sender, EventArgs e)
        {

        }

        WhatsApp wa;
        WhatsUserManager usrMan = new WhatsUserManager();
       
       

        public void SendWatsAppMessage(string user, string message)
        {
            var tmpUser = usrMan.CreateUser(user, "User");
            wa.SendMessage(tmpUser.GetFullJid(), message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in dlg.FileNames)
                {
                    listBox1.Items.Add(s);

                }
                attach = dlg.FileNames;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                DataSender data = new DataSender();
                data.ICQ = richTextBox4.Text;
                data.skypes = richTextBox3.Text;
                data.mails = richTextBox5.Text;
                data.wazaap = richTextBox6.Text;
                data.skype_icq_text = richTextBox2.Text;


                data.mail_text = richTextBox1.Text;
                data.mail_title = textBox1.Text;

                backgroundWorker1.RunWorkerAsync(data);
            }
            else
            {
                MessageBox.Show("Предыдущая задача еще выполняется!");
            }
           
        }
        SkypeSender sen = new SkypeSender();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            

            DataSender data = e.Argument as DataSender;

            string[] mails = data.mails.Split(',');
            if (mails.Length!=0)
            {
                foreach (string mail in mails)
                {
                    if (mail != string.Empty)
                    {
                       string to = mail.Replace("\n", "");
                        if (attach != null)
                        {
                            MailSender.SendMail(to, data.mail_title, data.mail_text, attach);
                            backgroundWorker1.ReportProgress(1);
                        }
                        else
                        {
                            MailSender.SendMail(to, data.mail_title, data.mail_text);
                            backgroundWorker1.ReportProgress(1);
                        }
                    }
                }
            }

            string [] skypes = data.skypes.Split(',');
            if (skypes.Length != 0)
            {
                foreach (string skype in skypes)
                {
                    if (skype != string.Empty)
                    {
                        sen.Send(data.skype_icq_text, skype);
                        backgroundWorker1.ReportProgress(1);
                    }
                }
            }

            string[] ICQS = data.ICQ.Split(',');
            if (ICQS.Length != 0)
            {
                foreach (string icq in ICQS)
                {
                    if (icq != string.Empty)
                    {
                        send.SendMessage(data.skype_icq_text, icq);
                        backgroundWorker1.ReportProgress(1);
                    }
                }
            }

            string[] watsApp = data.wazaap.Split(',');
            if (watsApp.Length != 0)
            {
                foreach (string wats in watsApp)
                {
                    if (wats != string.Empty)
                    {
                        Thread.Sleep(1000);
                        SendWatsAppMessage(wats, data.skype_icq_text);
                        backgroundWorker1.ReportProgress(1);
                    }
                }
            }
          
        }
        ICQSender send;
        int counter = 1;
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label10.Text = "Отправлено сообщений: "+counter++;
            s.countS++;
            entity.SaveChanges();

        }
    }

    public class DataSender
    {
     
        public string skypes { get; set; }
        public string mails { get; set; }
        public string ICQ { get; set; }
        public string skype_icq_text { get; set; }
        public string wazaap { get; set; }
        public string mail_title { get; set; }
        public string mail_text { get; set; }
    }
}

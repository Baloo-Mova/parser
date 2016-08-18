using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViKing.Engine;
using HtmlAgilityPack;
using System.Threading;
using System.Collections;
using System.IO;
using System.Net.Mail;
using OfficeOpenXml;
using System.Xml.Serialization;
using WhatsAppApi;
using WhatsAppApi.Account;


namespace Parser
{
    public partial class Form1 : Form
    {

        public delegate void RunWork(string s);
        public delegate void Rize(object o);
        public bool find = true;

        public Queue send = new Queue();
        private Queue okPeople = new Queue();
        public OK senders;
        Mails pas;
        public List<SleepingAcount> sleep = new List<SleepingAcount>();
        public Account acc;
        OK okMessenger;
        SkypeSender skyp = new SkypeSender();
        int sendMessages = 0;
        int countOK = 0;
        
        VK vk;

        statistica s = null;

        SendMessageData smd = new SendMessageData();
        users user;
        statisticEntities1 entity = new statisticEntities1();

        public void inListBox(object o)
        {
            ContactData data = o as ContactData;

                    if (data.type == ParserType.OK)
                    {
                        okPeople.Enqueue(data.link);
                        dataGridView1.Rows.Add(new object[] { counter++, "", data.link, "", "", "", "", "" });
                        s.countR++;
                        countOK++;
                        okSendInfo.Text = "OK: " + countOK;
                        return;
                    }
             
            dataGridView1.Rows.Add(new object[] { counter++, data.FIO, data.link, data.mails, data.city, data.phones, data.skypes, data.query });
            send.Enqueue(data);

            if (s != null)
            {
                s.countR++;
                if (data.phones != null)
                    s.countICQ += data.phones.Split(',').Length - 1;
                if (data.mails != null)
                    s.countMail += data.mails.Split(',').Length - 1;
                if (data.skypes != null)
                    s.countSkype += data.skypes.Split(',').Length - 1;
            }

            vkSendInfo.Text = "Vkontakte: " + vk.Count;
            googleSendInfo.Text = "Google: "+search.countSites;


            try
            {
                entity.SaveChanges();
            }
            catch
            {
                
            }
        }

       
        public Form1(users users)
        {
            try
            {
                this.user = users;
                InitializeComponent();
                sw.AutoFlush = true;
                search.onDataGet += vk_onContactRecieve;
                search.onExceptionCatch += ExceptionCatched;

                acc = Account.Load();

                

                //if (acc.icq == null || acc.icqm == null || acc.mails.Count<= 0|| acc.vk == null || acc.vkm == null || acc.ok.Count<=0)
                //{
                //    button1.Enabled = false;
                //    MessageBox.Show("Заполните данные аккаунтов и перезагрузите программу");
                //}

                //if (acc.mails.Count > 0)
                //{
                //    MailSender.Initialize(acc.mails);
                //}

                //if (acc.vk != null && acc.vkm != null)
                //{
                //    vk = new VK(acc.vk, acc.vkm);

                //    if (!vk.Login())
                //    {
                //        MessageBox.Show("Не вошли в ВК");
                //    }
                   
                //    vk.onExceptionCatch += ExceptionCatched;
                //    vk.onContactRecieve += vk_onContactRecieve;
                //}

                //if (acc.watsappLogin != null && acc.watsappPassword != null && acc.wazapNik != null)
                //{
                //    InitWatsApp();
                //}


            }
               catch (TypeInitializationException ex) {
    MessageBox.Show(ex.InnerException.StackTrace);
    }   
            catch
            {
                MessageBox.Show("Проблема с инициализацией");
            }
        }
        WhatsApp wa;
        string getDatFileName(string pn)
        {
            string filename = string.Format("{0}.next.dat", pn);
            return Path.Combine(Directory.GetCurrentDirectory(), filename);
        }
        WhatsUserManager usrMan = new WhatsUserManager();
        public void SendWatsAppMessage(string user, string message)
        {
            var tmpUser = usrMan.CreateUser(user, "User");
            wa.SendMessage(tmpUser.GetFullJid(), message);
        }

        public void InitWatsApp()
        {
            //wa = new WhatsApp(acc.watsappLogin, acc.watsappPassword, acc.wazapNik);
            //wa.Connect();

            //string datFile = getDatFileName(acc.watsappLogin);
            //byte[] nextChallenge = null;
            //if (File.Exists(datFile))
            //{
            //    try
            //    {
            //        string foo = File.ReadAllText(datFile);
            //        nextChallenge = Convert.FromBase64String(foo);
            //    }
            //    catch (Exception) { };
            //}

            //wa.Login(nextChallenge);
        }
        void search_ChangeProxy1()
        {
            this.Invoke(new Change(search_ChangeProxy));
        }

        void search_ChangeProxy()
        {


        }

        

     
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            needRunSendOK = true;
            if (checkBox3.Checked)
            {
                if (richTextBox2.Text != string.Empty && textBox2.Text != string.Empty && richTextBox4.Text != string.Empty)
                {
                    smd.body_mail = richTextBox2.Text;
                    smd.title_mail = textBox2.Text;
                    smd.body_icq = richTextBox4.Text;
                    googlevkSENDER.RunWorkerAsync(smd);
                    okSender.RunWorkerAsync(smd);

                    progressBar5.Style = ProgressBarStyle.Marquee;
                    progressBar3.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    checkBox3.Checked = false;
                    MessageBox.Show("Нельзя рассылать пустую информацию. Заполните данные и нажмите Поиск");
                    button1.Enabled = true;
                    return;
                }
            }
            


            if (textBox1.Text != String.Empty)
            {
                    var index = entity.statistica.Where(et => et.userid == user.Id && et.request == textBox1.Text).ToList();

                    foreach (statistica stat in index)
                    {
                        if (stat.data == DateTime.Now.Date)
                        {
                            if (stat.request == textBox1.Text)
                            {
                                s = stat;
                            }
                        }
                    }

                    if (s == null)
                    {
                        s = new statistica()
                        {
                            userid = user.Id,
                            data = DateTime.Now,
                            request = textBox1.Text
                        };
                        entity.statistica.Add(s);
                        entity.SaveChanges();
                    }

                   searchGroups.RunWorkerAsync(textBox1.Text);
                   GOOGLE.RunWorkerAsync(HttpUtility.UrlEncode(textBox1.Text));
                   ok.RunWorkerAsync(textBox1.Text);

                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar2.Style = ProgressBarStyle.Marquee;
                    progressBar4.Style = ProgressBarStyle.Marquee;
                }
            else if (richTextBox1.Lines.Count() != 0)
            {
                this.needRunSolo = true;
                if (s == null)
                {
                    var index = entity.statistica.Where(et => et.userid == user.Id && et.request == "").ToList();

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


                List<string> list = new List<string>(richTextBox1.Lines);
                SearchListBOX.RunWorkerAsync(list);
            }
           
        }

        StreamWriter sw = new StreamWriter("Exception.txt",true);
        void vk_onContactRecieve(object e)
        {
            this.Invoke(new Show(inListBox), new object[] { e });
        }

        int counter = 0;
       
     
        public string generateString(SortedSet<string> list)
        {
            string s = "";
            foreach (string ss in list)
            {
                s += ss + " ,";
            }
            return s;
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ExceptionCatched(object sender, ExceptionArgs exception)
        {
            sw.WriteLine(DateTime.Now.ToShortTimeString()+"  "+exception.Message+"     function: "+exception.Function);
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate() {
                    Exceptions.AppendText(exception.Function + "  :" + exception.Message);
                    }));
            }
        }
        

        private void searchGroups_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                vk.needRun = true;
                vk.SearchInGroups(e.Argument.ToString());
            }
            catch(Exception ex)
            {
                ExceptionCatched(this, new ExceptionArgs(ex.Message, "vk"));
            }
        }

        private void searchGroups_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            vk.needRun = false;
            Exceptions.AppendText("VK OFF"+Environment.NewLine);
        }

        GoogleSearcher search = new GoogleSearcher();
        private void button2_Click(object sender, EventArgs e)
        {
           
        }
       
      
        private void GOOGLE_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                search.Search(e.Argument.ToString());
            }
            catch  (Exception exception)
            {
               
            }
        }

        private void GOOGLE_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        public bool stop = true;
      
        private void GOOGLE_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            search.needRun = false;
            Exceptions.AppendText("Google OFF" + Environment.NewLine);
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Альфа тестирование");
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dlg.FileName);
                richTextBox1.Lines = lines;
                label2.Text = richTextBox1.Lines.Count().ToString();
               
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(dlg.FileName, richTextBox1.Lines);
            }
        }

        private void загрузитьПроксиToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }
        

        

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            label2.Text = "Count: " + richTextBox1.Lines.Count().ToString();
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {

        }

        
        private void button2_Click_1(object sender, EventArgs e)
        {
            SendMessageData data = new SendMessageData();
            data.body_mail = richTextBox2.Text;
            data.title_mail = textBox2.Text;
            data.body_icq = richTextBox4.Text;

            if (needRun)
            {
                MessageBox.Show("Рассылка уже происходит!");
                return;
            }
            else
            {
                needRun = true;
            }

            if (s == null)
            {
                var index = entity.statistica.Where(et => et.userid == user.Id && et.request == "").ToList();

                foreach (statistica stat in index)
                {
                    if (stat.data == DateTime.Now.Date)
                    {
                        if (stat.request == textBox1.Text)
                        {
                            s = stat;
                        }
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


            googlevkSENDER.RunWorkerAsync(data);
        }

        string[] attach;
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in dlg.FileNames)
                {
                    listBox2.Items.Add(s);
                    
                }
                attach = dlg.FileNames;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
           
        }
        public bool fromWhere = true;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SendMessageData data = e.Argument as SendMessageData;

            while (true)
            {
                if (needRun)
                {
                    try
                    {
                        if (send.Count != 0)
                        {
                            ContactData se = send.Dequeue() as ContactData;

                            if (se.mails != string.Empty)
                            {
                                string[] mailss = se.mails.Split(',');

                                foreach (string mail in mailss)
                                {
                                    if (mail != string.Empty)
                                    {
                                        if (listBox2.Items.Count > 0)
                                        {

                                            MailSender.SendMail(mail, data.title_mail, data.body_mail, attach);
                                             googlevkSENDER.ReportProgress(1, new ReportOK(mail, "Отправлено"));

                                        }
                                        else
                                        {
                                            MailSender.SendMail(mail, data.title_mail, data.body_mail);
                                             googlevkSENDER.ReportProgress(1, new ReportOK(mail, "Отправлено"));
                                        }
                                    }
                                }
                            }
                            if (se.skypes != string.Empty)
                            {
                                string[] skypes = se.skypes.Split(',');
                                foreach (string sk in skypes)
                                {
                                    if (sk != string.Empty)
                                    {
                                        skyp.Send(data.body_icq, sk);
                                        googlevkSENDER.ReportProgress(1, new ReportOK(sk, "Отправлено"));
                                    }
                                }
                            }
                            if (se.phones != string.Empty)
                            {
                                string[] phones = se.phones.Split(',');
                                foreach (string sk in phones)
                                {
                                    if (sk != string.Empty)
                                    {
                                        Thread.Sleep(1000);

                                       string ssk = RemoveSpaces(sk);
                                       if (ssk.Length < 11)
                                       {
                                           SendWatsAppMessage(ssk, data.body_icq);
                                           googlevkSENDER.ReportProgress(1, new ReportOK(sk, "Отправлено"));
                                       }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                Thread.Sleep(1000);
            }
        }


        public string RemoveSpaces(string sk)
        {

           return sk.Replace("(","").Replace(")","").Replace(" ","").Replace("-","");


        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                SendInfo.Text = "Отправлено сообщений: " + sendMessages++;
                s.countS++;
                ReportOK report = e.UserState as ReportOK;
                dataGridView2.Rows.Add(new object[]{ sendMessages, report.ID , report.Type });

                entity.SaveChanges();
            }
            catch { }

            
        }
        public double TimeStamp()
        {
            return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(dlg.FileName, richTextBox2.Lines);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            richTextBox2.Clear();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                richTextBox2.Lines = File.ReadAllLines(dlg.FileName);
            }
        }

        public object locker = new object();

        private void timer1_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    for (int i = sleep.Count - 1; i >= 0; i--)
            //    {
            //        if (sleep[i].TimeTo < TimeStamp())
            //        {
            //            sleep.Remove(sleep[i]);
            //        }
            //    }
            //}
            //catch(Exception ex)
            //{

            //}
        }

        bool needRunSendOK = true;

        private void button9_Click(object sender, EventArgs e)
        {
            needRunSendOK = false;
            this.button1.Enabled = true;
            okMessenger.needRun = false;
            vk.needRun = false;
            search.needRun = false;
            this.needRun = false;
            this.needRunSolo = false;

        }
        public bool needRun = false;
        public int getMaxIndex(int index)
        {
            int k = 0;
            int max = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                try
                {
                    if (dataGridView1.Rows[i].Cells[index - 1].Value.ToString().Length > max)
                    {
                        max = dataGridView1.Rows[i].Cells[index - 1].Value.ToString().Length;
                        k = i;
                    }
                }
                catch { }
            }
            return k+1;
        }

        private void экспортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В виду обновления функционала данная функция еще не доступна.");
            //if (dataGridView1.Rows.Count != 0)
            //{
            //    SaveFileDialog dlg = new SaveFileDialog();
            //    dlg.Filter = "Excel|*.xlsx";
            //    if (dlg.ShowDialog() == DialogResult.OK)
            //    {
            //        DataGridView view = dataGridView1;
            //        FileInfo info = new FileInfo(dlg.FileName);
                   
            //        using (ExcelPackage package = new ExcelPackage(info))
            //        {
                        
            //            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(DateTime.Now.ToShortDateString());
            //            for (int i = 0; i < view.RowCount; i++)
            //            {
            //                for (int k = 0; k < 7; k++)
            //                {
            //                    try
            //                    {
            //                        worksheet.Cells[i + 1, k + 1].Value = view.Rows[i].Cells[k].Value.ToString();
            //                    }
            //                    catch { }
            //                    worksheet.Cells[i + 1, k + 1].AutoFitColumns();
                               
            //                }
            //            }

            //            worksheet.Cells[getMaxIndex(1), 1].AutoFitColumns();

            //            worksheet.Cells[getMaxIndex(2), 2].AutoFitColumns();

            //            worksheet.Cells[getMaxIndex(3), 3].AutoFitColumns();

            //            worksheet.Cells[getMaxIndex(4), 4].AutoFitColumns();

            //            worksheet.Cells[getMaxIndex(5), 5].AutoFitColumns();

            //            worksheet.Cells[getMaxIndex(6), 6].AutoFitColumns();

            //            worksheet.Cells[getMaxIndex(7), 7].AutoFitColumns();

            //            package.Save();
            //        }
                   
            //    }
            //}
        }

        private void импортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int count = 0;
            //OpenFileDialog dlg = new OpenFileDialog()
            //;
            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            //    FileInfo info = new FileInfo(dlg.FileName);
            //    using (ExcelPackage package = new ExcelPackage(info))
            //    {

            //        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    
            //        for (int i = 0; i < worksheet.Dimension.End.Row; i++)
            //        {
                       
            //                count++;
            //                try
            //                {
            //                    dataGridView1.Rows.Add(new object[] { worksheet.Cells[i + 1, 1].Value, worksheet.Cells[i + 1, 2].Value, worksheet.Cells[i + 1, 3].Value, worksheet.Cells[i + 1, 4].Value, worksheet.Cells[i + 1, 5].Value, worksheet.Cells[i + 1, 6].Value, worksheet.Cells[i + 1, 7].Value });
            //                    ContactData data = new ContactData(worksheet.Cells[i + 1, 1].Value, worksheet.Cells[i + 1, 2].Value, worksheet.Cells[i + 1, 3].Value, worksheet.Cells[i + 1, 4].Value, worksheet.Cells[i + 1, 5].Value, worksheet.Cells[i + 1, 6].Value, worksheet.Cells[i + 1, 7].Value);
            //                    send.Enqueue(data);
            //                }
            //                catch
            //                {
            //                    MessageBox.Show("Как минимум 1 строка не соответствует условию импорта измените файл ошибка в строке " + count);
            //                }
                        
            //        }
            //    }
            //}
            MessageBox.Show("В виду обновления функционала данная функция еще не доступна.");
        }

        private void одиночнаяРассылкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Send send = new Send(acc,user,wa);
            send.Show();
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(dlg.FileName, richTextBox4.Lines);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            richTextBox4.Clear();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                richTextBox4.Lines = File.ReadAllLines(dlg.FileName);
            }
        }

        private void аккаунтыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accounts accs = new Accounts();
            if (accs.ShowDialog() == DialogResult.OK)
            {

                acc = Account.Load();

                //if (acc.icq == null || acc.icqm == null || acc.mails.Count<=0 || acc.vk == null || acc.vkm == null || acc.ok.Count<=0)
                //{
                //    button1.Enabled = false;
                //}
                //else
                //{
                //    button1.Enabled = true;
                //}

            }
            else
            {
                MessageBox.Show("Контакты не сохранены");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        public bool needRunSolo = false;
        private void SearchListBOX_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> list = e.Argument as List<string>;
            foreach (string link in list)
            {
                if (!needRunSolo)
                {
                    return;
                }

                if (link.Contains("vk.com"))
                {

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    string sss = vk.getPage(link);
                    try
                    {
                        doc.LoadHtml(sss);
                    }
                    catch { }

                    string s = doc.DocumentNode.InnerText;

                    SortedSet<string> phones = Searcher.findPhone(s);
                    SortedSet<string> mails = Searcher.findMail(s);
                    SortedSet<string> skype = Searcher.findSkype(sss);

                    ContactData data = new ContactData();
                    data.phones = generateString(phones);
                    data.mails = generateString(mails);
                    data.FIO = Searcher.getFIO(sss);
                    data.city = Searcher.getCity(sss);
                    data.link = link;
                    data.skypes = generateString(skype);
                    this.Invoke(new Rize(inListBox), new object[] { data });

                    vk.searchAllInGroup(link);

                }
                else
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    string sss = search.getHtml(link);
                    if (sss == "Error")
                    {
                        ExceptionCatched(this, new ExceptionArgs("ПРоблема с прокси не возможно обработать сайт", link));
                        continue;
                    }
                    if (sss != "")
                    {
                        try
                        {
                            doc.LoadHtml(sss);
                        }
                        catch { }

                        string s = doc.DocumentNode.InnerText;

                        SortedSet<string> phones = Searcher.findPhone(s);
                        SortedSet<string> mails = Searcher.findMail(s);
                        SortedSet<string> skype = Searcher.findSkype(sss);

                        List<string> moreInfo = search.getMoreLinks(sss, link);

                        ContactData data = new ContactData();
                        data.phones = generateString(phones) + moreInfo[2];
                        data.mails = generateString(mails) + moreInfo[1];
                        data.link = link;
                        data.skypes = generateString(skype) + moreInfo[0]; 
                        this.Invoke(new Rize(inListBox), new object[] { data });
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void очиститьСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();
                send.Clear();
                okPeople.Clear();
            }
            catch
            {

            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                if (richTextBox2.Text != string.Empty && textBox2.Text != string.Empty && richTextBox4.Text != string.Empty)
                {
                    smd.body_mail = richTextBox2.Text;
                    smd.title_mail = textBox2.Text;
                    smd.body_icq = richTextBox4.Text;
                    needRun = true;
                }
                else
                {
                    MessageBox.Show("Нельзя произвести рассылку с пустыми или полупустыми контактами!!!");
                    checkBox3.Checked = false;
                }
            }
            else
            {
                needRun = false;
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            StatShowForm frm = new StatShowForm(user.Id);
            frm.ShowDialog();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
            if (search!=null&&!search.needRun)
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
            }
            if (vk!=null&&!vk.needRun)
            {
                progressBar2.Style = ProgressBarStyle.Blocks;
            }
            if (okMessenger != null && !okMessenger.needRun)
            {
                progressBar4.Style = ProgressBarStyle.Blocks;
            }
            else
            {
                progressBar4.Style = ProgressBarStyle.Marquee;
            }
            if (!needRunSendOK)
            {
                progressBar5.Style = ProgressBarStyle.Blocks;
            }
            if (!needRun)
            {
                progressBar3.Style = ProgressBarStyle.Blocks;
            }
        }

        private void ok_DoWork(object sender, DoWorkEventArgs e)
        {
            
                while (true)
                {
                    Mails pas = acc.getOKAccount();
                    if (pas == null)
                    {
                        MessageBox.Show("Не достаточно аккаунтов ОК для работы...");
                        return;
                    }

                    okMessenger = new OK(pas.mail, pas.mpas);

                    if (okMessenger.Login())
                    {
                        break;
                    }
                    else
                    {
                        acc.Delete(pas, ParserType.OK);
                    }
                }

            okMessenger.onDataGet += vk_onContactRecieve;
            okMessenger.onExceptionCatch += ExceptionCatched;

            okMessenger.Search(e.Argument as string);
        }

        public double isIn(string name)
        {
            for (int i = sleep.Count - 1; i >= 0; i--)
            {
                if (sleep[i].Login == name) { return sleep[i].TimeTo; }
            }

            return 0;
        }

        private void okSender_DoWork(object sender, DoWorkEventArgs e)
        {
            int iterator = 0;
            Thread.Sleep(10000);
            SendMessageData smd = e.Argument as SendMessageData;
          
            while (true)
            {
                changeAccountOK();
                iterator = 0;

              
                while (iterator<7)
                {
                    if (!needRunSendOK)
                    {
                        return;
                    }


                    if (okMessenger.Users.Count > 0)
                    {
                        string user = okMessenger.Users.Dequeue();
                        string s = senders.sendMessage(smd.body_icq, user);

                        if (s == "NotSended")
                        {
                            okSender.ReportProgress(1, new ReportOK(user, "Не отправлено"));
                            continue;
                        }
                        else if (s.Contains("Писать личные сообщения пользователь разрешает только друзьям"))
                        {
                            okSender.ReportProgress(1, new ReportOK(user, "Фе бяка личка закрыта..."));
                            iterator++;
                            Thread.Sleep(10000);
                            continue;
                        }
                        else if (s.Contains("Вы слишком часто отправляете сообщения разным пользователям. Повторите попытку позже"))
                        {
                            break;
                        }

                        okSender.ReportProgress(1, new ReportOK(user, "Отправлено"));
                        iterator++;
                    }

                    Thread.Sleep(10000);
                }

               
                    sleep.Add(new SleepingAcount(pas.mail, TimeStamp() + 900));

            }
        }
      
        public void changeAccountOK()
        {
            while (true)
            {
                pas = acc.getOKAccount();
                Thread.Sleep(1);
                if (pas == null)
                {
                    ExceptionCatched(this, new ExceptionArgs("Рассылка по ОК не запущена. Не достаточно аккаунтов.", "oKSenderWorker"));
                    needRunSendOK = false;
                    return;
                }

                for (int i = sleep.Count - 1; i >= 0; i--)
                {
                    if (sleep[i].Login == pas.mail)
                    {
                        if(sleep[i].TimeTo < TimeStamp())
                        {
                            sleep.Remove(new SleepingAcount(pas.mail, sleep[i].TimeTo));
                            break;
                        }
                        else
                        {
                            pas = null; break; 
                        }
                        
                    }
                }

                if (pas == null)
                {
                    continue;
                }

                senders = new OK(pas.mail, pas.mpas);

                if (senders.Login())
                {
                    break;
                }
                else
                {
                    acc.Delete(pas, ParserType.OK);
                }

                
            }
        }

        private void программаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void тестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchGroups.RunWorkerAsync(HttpUtility.UrlEncode("Водонагреватели"));
        }

    }
    public class ReportOK
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public ReportOK(string id, string type)
        {
            this.ID = id;
            this.Type = type;
        }
    }
    public class SendMessageData{
        public string title_mail { get; set; }
        public string body_mail { get; set; }
        public string body_icq { get; set; }
    }

    

   

  
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ViKing.Engine;

namespace Parser
{
    public partial class Accounts : Form
    {
        StreamWriter write;
        Account acc = new Account();
        public Accounts()
        {
            InitializeComponent();

            XmlSerializer serializer =
        new XmlSerializer(typeof(Account));

            if (!File.Exists("Sys.CFG"))
            {
                using (StreamWriter sw = new StreamWriter("Sys.CFG"))
                {
                    serializer.Serialize(sw, new Account());

                }
            } 
            
            FileStream fs = new FileStream("Sys.CFG", FileMode.Open);
            acc = (Account)serializer.Deserialize(fs);

            fs.Close();

            Proxies.Clear();

            foreach (string s in acc.proxy)
            {
                Proxies.AppendText(s + Environment.NewLine);
            }


            dataGridView1.Rows.Clear();

            foreach (Mails mail in acc.mails)
            {
                dataGridView1.Rows.Add(1);
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = mail.mail;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = mail.mpas;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = mail.smtp;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = mail.port;

            }

            dataGridView2.Rows.Clear();
            foreach (Mails mail in acc.ok)
            {
                dataGridView2.Rows.Add(1);
                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = mail.mail;
                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = mail.mpas;
               

            }

            vkGrid.Rows.Clear();

            foreach (Mails vk in acc.vk)
            {
                vkGrid.Rows.Add(1);
                vkGrid.Rows[vkGrid.RowCount - 1].Cells[0].Value = vk.mail;
                vkGrid.Rows[vkGrid.RowCount - 1].Cells[1].Value = vk.mpas;
            }

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            write = new StreamWriter("Sys.CFG");

            XmlSerializer serializer =
        new XmlSerializer(typeof(Account));


            string[] proxy = Proxies.Text.Split('\n');

            acc.proxy.Clear();

            foreach(string s in proxy){
                if(s!=string.Empty)
                acc.proxy.Add(s);
            }
           
            //acc.icq = WANik.Text;
            //acc.icqm=WALogin.Text;
            //acc.vk=vkLogin.Text;
            //acc.vkm=vkPass.Text;
            //acc.watsappLogin = watsappLogin.Text;
            //acc.watsappPassword = watsappPassword.Text;
            //acc.wazapNik = wazapNik.Text;

            acc.mails.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {

                int num = 0;
                if (row.Cells[3].Value.ToString() != "")
                {
                    num = Convert.ToInt32(row.Cells[3].Value);
                }

                acc.mails.Add(new Mails(row.Cells[0].Value.ToString(),row.Cells[1].Value.ToString(),row.Cells[2].Value.ToString(),num));
            }
            acc.ok.Clear();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            { 
                acc.ok.Add(new Mails(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString()));
            }



            serializer.Serialize(write, acc);

            write.Close();
            this.Close();
        }

        private void Accounts_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(1);
            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = textBox1.Text;
            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = textBox2.Text;
            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = textBox7.Text;
            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = textBox8.Text;
            textBox1.Clear();
            textBox2.Clear();
            textBox7.Clear();
            textBox8.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
        }

      

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dlg.FileName);
                foreach (string acc in lines)
                {
                    string[] mail = acc.Split(':');

                    dataGridView1.Rows.Add(1);
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = mail[0];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = mail[1];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = "";
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = 0;
                }

            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            vkGrid.Rows.Add(1);
            vkGrid.Rows[vkGrid.RowCount - 1].Cells[0].Value = vkLogin.Text;
            vkGrid.Rows[vkGrid.RowCount - 1].Cells[1].Value = vkPass.Text;
            vkLogin.Clear();
            vkPass.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dlg.FileName);
                foreach (string acc in lines)
                {
                    string[] mail = acc.Split(':');

                    vkGrid.Rows.Add(1);
                    vkGrid.Rows[vkGrid.RowCount - 1].Cells[0].Value = mail[0];
                    vkGrid.Rows[vkGrid.RowCount - 1].Cells[1].Value = mail[1];
                }

            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (vkGrid.CurrentRow != null)
            {
                vkGrid.Rows.Remove(vkGrid.CurrentRow);
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            dataGridView2.Rows.Add(1);
            dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = okLogin.Text;
            dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = okPass.Text;

            okPass.Clear();
            okLogin.Clear();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dlg.FileName);
                foreach (string acc in lines)
                {
                    string[] mail = acc.Split(':');
                     
                    dataGridView2.Rows.Add(1);
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = mail[0];
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = mail[1];
                }

            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null)
            {
                dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            WatsAppGrid.Rows.Add(1);
            WatsAppGrid.Rows[WatsAppGrid.RowCount - 1].Cells[0].Value = WANik.Text;
            WatsAppGrid.Rows[WatsAppGrid.RowCount - 1].Cells[1].Value = WALogin.Text;
            WatsAppGrid.Rows[WatsAppGrid.RowCount - 1].Cells[2].Value = WAPass.Text;

            WANik.Clear();
            WALogin.Clear();
            WAPass.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dlg.FileName);
                foreach (string acc in lines)
                {
                    string[] mail = acc.Split(':');

                    WatsAppGrid.Rows.Add(1);
                    WatsAppGrid.Rows[WatsAppGrid.RowCount - 1].Cells[0].Value = mail[0];
                    WatsAppGrid.Rows[WatsAppGrid.RowCount - 1].Cells[1].Value = mail[1];
                    WatsAppGrid.Rows[WatsAppGrid.RowCount - 1].Cells[2].Value = mail[2];
                }

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (WatsAppGrid.CurrentRow != null)
            {
                WatsAppGrid.Rows.Remove(WatsAppGrid.CurrentRow);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;

namespace AllClassesTEST
{
    public partial class Form1 : Form
    {
   
        HashSet<string> list = new HashSet<string>();
        
    
        public Form1()
        {
            InitializeComponent();
            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //if (dlg.ShowDialog() == DialogResult.OK)
            //{ 
                    work.RunWorkerAsync(); 
           // }
        }
        public string[] list1 = new string[200000000];
        private void work_DoWork(object sender, DoWorkEventArgs e)
        {
            //StreamReader read = new StreamReader(e.Argument.ToString());
            //StreamWriter sw = new StreamWriter("result_" + (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString()+ ".txt");
           
            string line = "";
          
            while (!work.CancellationPending)
            {
                try
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        list1[count] = "as;diasldhasldhalsdkhalksdhlasdh";
                        count++;
                    }
                    work.ReportProgress(1);
                    Thread.Sleep(1);
                }
                catch( Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            } 

        }
        public long count = 0;
        private void work_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
             
            this.Text = count.ToString();
        }

        private void work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            work.CancelAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public partial class TestForm : Form
    {

        Account acc = new Account();
        VK vk;

        List<string> list = new List<string>();

        public TestForm()
        {
            InitializeComponent();

           

        }

        void vk_onExceptionCatch(object sender, ExceptionArgs exception)
        {
            throw new NotImplementedException();
        }

        void vk_onContactRecieve(object e)
        {
            
            ContactData data = e as ContactData;
            list.Add(data.ToString());
            this.Invoke(new Action(
                 () =>
                 {
                     richTextBox1.AppendText(data.ToString() + Environment.NewLine);
                 }
            )); 
        }
        


        private void button1_Click(object sender, EventArgs e)
        {
            VK v = new VK("+380933137698", "qwerty418390");
            v.Login();
            v.SearchInGroups("полезная вода");
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
        //    acc = Account.Load();

        //    vk = new VK(acc.vk, acc);

        //    if (!vk.Login())
        //    {
        //        MessageBox.Show("Не вошли в ВК");
        //    }

        //    vk.onExceptionCatch += vk_onExceptionCatch;
        //    vk.onContactRecieve += vk_onContactRecieve;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            vk.SearchInGroups("полезная вода");
        }
    }
}

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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
      
        private void button1_Click(object sender, EventArgs e){

            using (var entry = new statisticEntities1())
            {
                var users = entry.users.Where(b => b.login == textBox1.Text && b.parol == textBox2.Text).FirstOrDefault();

                if (users != null)
                {
                    Form1 form = new Form1(users);
                    form.Show();
                    this.Hide();
                }
            }

           
        }
    }
}

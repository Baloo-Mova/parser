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
    public partial class StatShowForm : Form
    {
        public StatShowForm()
        {
            InitializeComponent();
        }
        public StatShowForm(int id)
        {
            InitializeComponent();

            try{
                statisticEntities1 entity = new statisticEntities1();

                var stat = entity.statistica.Where(e => e.userid == id).ToList();

                foreach (var s in stat)
                {
                    dataGridView1.Rows.Add(new object[] {s.data,s.request,s.countR,s.countS,s.countICQ,s.countSkype,s.countMail });
                }

            }catch{
                MessageBox.Show("Не удалось загрузить статистику.");
            }
        }

        private void StatShowForm_Load(object sender, EventArgs e)
        {

        }
    }
}

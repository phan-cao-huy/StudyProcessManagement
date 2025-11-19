using StudyProcessManagement.Views.SinhVien.assignments;
using StudyProcessManagement.Views.SinhVien.assignments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
namespace StudyProcessManagement.Views.Login
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            var fm = new assignments();
            fm.Show();
            this.Hide();
            //If you want login to reappear when assignments closes, handle FormClosed:
            //fm.FormClosed += (s, args) => this.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentOnCars
{
    public partial class Welcome_page : Form
    {
        public Welcome_page()
        {
            InitializeComponent();
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.Show();
        }
    }
}

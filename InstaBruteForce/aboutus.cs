using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using System.Diagnostics;

namespace InstaBruteForce
{
    public partial class aboutus : MaterialSkin.Controls.MaterialForm
    {
        public aboutus()
        {
            InitializeComponent();
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Process.Start("http://t.me/HelloCyka");
        }
    }
}

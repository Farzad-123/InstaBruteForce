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

namespace InstaBruteForce
{
    public partial class MsgBox : MaterialSkin.Controls.MaterialForm
    {
        public MsgBox()
        {
            InitializeComponent();
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

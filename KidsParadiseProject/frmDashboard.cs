using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KidsParadiseProject
{
    public partial class frmDashboard : Form
    {
        public frmDashboard()
        {
            InitializeComponent();
        }

        private void createGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCreateGame fc = new frmCreateGame();
            fc.ShowDialog();
        }

        private void billingPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInvoice fi = new frmInvoice();
            fi.ShowDialog();
        }
    }
}

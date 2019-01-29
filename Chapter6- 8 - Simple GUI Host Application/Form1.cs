using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Chapter6__8___Simple_GUI_Host_Application
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (PSObject thisDate in new RunspaceInvoke().Invoke("get-date"))
            {
                MessageBox.Show(thisDate.ToString(), "Today's Date");
            }
        }
    }
}
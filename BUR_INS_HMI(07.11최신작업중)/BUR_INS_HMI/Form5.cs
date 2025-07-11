using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BUR_INS_HMI
{
    

    public partial class Form5 : Form
    {

        private System.Windows.Forms.Timer blinkTimer;
        private bool isFlashing = false;
        private Form1 mainForm;

        public Form5(Form1 parent, string message)
        {
            InitializeComponent();
            this.mainForm = parent;

            this.FormClosed += Form5_FormClosed;

            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 1000;
            blinkTimer.Tick += (s, e) =>
            {
                if (message == "RED")
                {
                    this.BackColor = isFlashing ? Color.Black : Color.FromArgb(192, 0, 0);
                    this.label2.Text = "구동 불량 발생";
                    this.label2.ForeColor = isFlashing ? Color.Red : Color.Black;
                }
                else if (message == "YELLOW")
                {
                    this.BackColor = isFlashing ? Color.Black : Color.FromArgb(192, 0, 0);
                    this.label2.Text = "구동 경고 발생";
                    this.label2.ForeColor = isFlashing ? Color.Yellow : Color.Black;
                }
                isFlashing = !isFlashing;
            };
            blinkTimer.Start();
        }



        private void Form5_Load(object sender, EventArgs e)
        {
            //   LoadDataFromDB();
        }

        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            blinkTimer?.Stop();
            blinkTimer?.Dispose();
            mainForm.get_message = false;
        }
    }

}

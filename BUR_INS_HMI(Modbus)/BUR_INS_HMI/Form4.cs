using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUR_INS_HMI
{
    public partial class Form4 : Form
    {
        public delegate void FormSendDataHandler2(string s1, string s2);
        public event FormSendDataHandler2 FormSendEvent2;

        public Form4()
        {
            InitializeComponent();
            editer_show.Text = "0";
        }

        private void editer_btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editer_btn_check_Click(object sender, EventArgs e)
        {
            if (editer_show.Text != "0")
            {
                this.FormSendEvent2(editer_show.Text, unit_lbl.Text);
                editer_show.Text = null;
            }
            this.Close();
        }

        private void editer_btn_1_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_1.Text;
            else
                editer_show.Text += editer_btn_1.Text;
        }

        private void editer_btn_2_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_2.Text;
            else
                editer_show.Text += editer_btn_2.Text;
        }

        private void editer_btn_3_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_3.Text;
            else
                editer_show.Text += editer_btn_3.Text;
        }

        private void editer_btn_4_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_4.Text;
            else
                editer_show.Text += editer_btn_4.Text;
        }

        private void editer_btn_5_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_5.Text;
            else
                editer_show.Text += editer_btn_5.Text;
        }

        private void editer_btn_6_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_6.Text;
            else
                editer_show.Text += editer_btn_6.Text;
        }

        private void editer_btn_7_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_7.Text;
            else
                editer_show.Text += editer_btn_7.Text;
        }

        private void editer_btn_8_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_8.Text;
            else
                editer_show.Text += editer_btn_8.Text;
        }

        private void editer_btn_9_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_9.Text;
            else
                editer_show.Text += editer_btn_9.Text;
        }

        private void editer_btn_0_Click(object sender, EventArgs e)
        {
            if (editer_show.Text == "0")
                editer_show.Text = editer_btn_0.Text;
            else
                editer_show.Text += editer_btn_0.Text;
        }

        private void editer_btn_pt_Click(object sender, EventArgs e)
        {
            bool flag = editer_show.Text.Contains('.');

            if (flag == false)
                editer_show.Text += editer_btn_pt.Text;
        }

        private void editer_btn_del_Click(object sender, EventArgs e)
        {
            string str = editer_show.Text;
            if (editer_show.Text != null && editer_show.Text.Length != 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            editer_show.Text = str;
        }

        private void editer_btn_clr_Click(object sender, EventArgs e)
        {
            editer_show.Text = "0";
        }


    }
}

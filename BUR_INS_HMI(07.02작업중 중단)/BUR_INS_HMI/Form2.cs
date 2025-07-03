using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUR_INS_HMI
{
    public partial class Form2 : Form
    {
        private Form4 f4;
        public delegate void FormSendDataHandler(string s); //delegate 선언
        public event FormSendDataHandler FormSendEvent; //event 생성
        public delegate void FormSendDataHandler2(string s1, string s2);
        private TextBox[] amp_set_txt;

        private decimal[] amp_set;
        private Form1 f1;

        public Form2(Form1 form)
        {
            InitializeComponent();
            admin_name.Visible = true;
            password_panel.Visible = true;
            edit_panel.Visible = false;
            this.f1 = form;
            amp_set = f1.amp;
            amp_set_txt = new TextBox[] { textBox11, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox12 };
            Init_setting();
        }

        String password = "0000";
        String user_input = "";

        private void Init_setting()
        {
            for (int i = 0; i < amp_set.Length; i++)
            {
                amp_set_txt[i].Text = amp_set[i].ToString("F1");
                amp_set_txt[i].Tag = i;

                amp_set_txt[i].KeyDown += AmpTextBox_KeyDown;
                amp_set_txt[i].Leave += AmpTextBox_Leave;

                //  this.Controls.Add(amp_set_txt[i]);

            }

            pos2val.Text = f1.target_rpm.ToString("F1");
            pos2val.TextChanged += pos2val_TextChanged;

        }

        private void UpdateAmpFromTextBox(TextBox tb)
        {
            if (tb != null && decimal.TryParse(tb.Text, out decimal value) && value >= 0)
            {
                int index = (int)tb.Tag;
                amp_set[index] = Math.Round(value, 1);

                // 예시: 라벨 반영
                // amp_label[index].Text = $"{apm[index]:F1} A";
            }
            else
            {
                MessageBox.Show("0 이상 실수를 입력하세요. 예: 12.3", "입력 오류");
                tb.Focus();
            }
        }
        private void AmpTextBox_Leave(object sender, EventArgs e)
        {
            UpdateAmpFromTextBox(sender as TextBox);
        }

        private void AmpTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox tb = sender as TextBox;
                UpdateAmpFromTextBox(tb);

                // 엔터 후 포커스를 다른 곳으로 옮겨줌 (포커스가 Leave를 또 유발)
                this.SelectNextControl(tb, true, true, true, true);

                // 엔터 키 동작 안 남기도록 처리
                e.SuppressKeyPress = true;
            }
        }

       



        private void close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numpad_check_Click(object sender, EventArgs e)
        {
            CheckPassword();
        }

        private void CheckPassword()
        {
            user_input = textBox1.Text;

            if (password.Equals(user_input))
            {
                MessageBox.Show("인증되었습니다.", "관리자 로그인");

                user_input = "";
                textBox1.Text = null;

                //   serialPort.WriteSingleRegister(slaveId, address, value);
                /*  byte[] data = new byte[] { 0x01, 0x06, 0x00, 0x0A, 0x00, 0x01, 0x84, 0x0A }; // 예: Modbus RTU 요청 패킷

                  serialPort.Write(data, 0, data.Length);*/

                this.FormSendEvent("TRUE"); //MainForm으로 데이터 전송

                admin_name.Visible = false;
                password_panel.Visible = false;
                edit_panel.Visible = true;
            }
            else
            {
                MessageBox.Show("비밀번호가 올바르지 않습니다.", "로그인 실패");
                user_input = "";
                textBox1.Text = null;
                this.FormSendEvent("FALSE");    //MainForm으로 데이터 전송
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CheckPassword();
            }
        }

        private void pos2val_TextChanged(object sender, EventArgs e)
        {
            if (pos2val != null && decimal.TryParse(pos2val.Text, out decimal value))
            {
                f1.target_rpm = value;
            }
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < amp_set.Length; i++)
            {
                //   amp_set_txt[i].Text = amp_set[i].ToString("F1");
                f1.amp[i] = Convert.ToDecimal(amp_set_txt[i].Text);
            }

            MessageBox.Show("Complete");
        
        }


    }
}

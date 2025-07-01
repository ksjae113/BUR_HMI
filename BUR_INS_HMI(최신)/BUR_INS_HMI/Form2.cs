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
        public delegate void FormSendDataHandler(string s); //delegate 선언
        public event FormSendDataHandler FormSendEvent; //event 생성

        public Form2()
        {
            InitializeComponent();
        }

        String password = "0000";
        String user_input = "";

        private void close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numpad_check_Click(object sender, EventArgs e)
        {

            if (password.Equals(user_input))
            {
                MessageBox.Show("인증되었습니다.", "관리자 로그인");

                user_input = "";
                password_lbl.Text = null;
                this.FormSendEvent("TRUE"); //MainForm으로 데이터 전송
                                            //   serialPort.WriteSingleRegister(slaveId, address, value);
                /*  byte[] data = new byte[] { 0x01, 0x06, 0x00, 0x0A, 0x00, 0x01, 0x84, 0x0A }; // 예: Modbus RTU 요청 패킷

                  serialPort.Write(data, 0, data.Length);*/


                this.Close();
            }
            else
            {
                MessageBox.Show("비밀번호가 올바르지 않습니다.", "로그인 실패");
                user_input = "";
                password_lbl.Text = null;
                this.FormSendEvent("FALSE");    //MainForm으로 데이터 전송
            }

        }

    


    }
}

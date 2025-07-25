﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
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

        private int change_stat = 0;

        String password = "0000";
        String user_input = "";

        public Form2(string pw)
        {
            InitializeComponent();
            password = pw;
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void numpad_check_Click(object sender, EventArgs e)
        {
            user_input = textBox1.Text;
            if (change_stat != 1 && password.Equals(user_input))
            {
                MessageBox.Show("로그인 되었습니다.", "관리자 로그인");

               
                    user_input = "";
                    textBox1.Text = null;
                    this.FormSendEvent("TRUE"); //MainForm으로 데이터 전송
                           //   serialPort.WriteSingleRegister(slaveId, address, value);
                /*  byte[] data = new byte[] { 0x01, 0x06, 0x00, 0x0A, 0x00, 0x01, 0x84, 0x0A }; // 예: Modbus RTU 요청 패킷

                  serialPort.Write(data, 0, data.Length);*/

                this.Hide();
            }
            else if (change_stat == 1 && password.Equals(user_input))
            {
                MessageBox.Show("로그인 되었습니다.", "관리자 로그인");
                user_input = "";
                textBox1.Text = null;
                now_pass.Visible = false;
                new_pass.Visible = true;
                textBox1.PasswordChar = '\0';
                change_stat = 2;
                
            }
            else if (change_stat == 2)
            {
                password = user_input;
                MessageBox.Show("변경되었습니다.", "패스워드 변경");
                user_input = "";
                textBox1.Text = null;
                new_pass.Visible = false;
                login_name.Visible = true;
                change_stat = 0;
                textBox1.PasswordChar = '*';
                this.FormSendEvent($"{password}");
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
                numpad_check_Click(sender, EventArgs.Empty);
            }
        }

        private void change_btn_Click(object sender, EventArgs e)
        {
            user_input = "";
            textBox1.Text = null;
            change_stat = 1;
            login_name.Visible = false;
            now_pass.Visible = true;
        }
    }
}

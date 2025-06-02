using System;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using ScottPlot;
using System.Drawing.Interop;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.WinForms;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {


        private Form3 f3;


        private System.Windows.Forms.Timer timer;
        private int pulseCount = 0;

        public SerialPort serialPort;

        string str;
        public Form1()
        {
            InitializeComponent();

            Init_port();

            InitChart();

        

        }

      

       

        private void Init_port()
        {
            //COM PORT
            foreach (var key in System.IO.Ports.SerialPort.GetPortNames())
            {
                str = key.ToString();
                MessageBox.Show(str);
            }
        }


        private void InitChart()
        {
            var plot = formsPlot1.Plot;
            var rand = new Random();

            int pointCount = 50;

            for (int seriesIndex = 0; seriesIndex < 27; seriesIndex++)
            {
                double[] xs = ScottPlot.DataGen.Consecutive(pointCount);    //시간축 (50개)
                double[] ys = new double[pointCount];

                for (int i = 0; i < pointCount; i++)
                {
                    //    ys[i] = rand.NextDouble() * 100;    //각 y값은 0 ~100 사이의 난수
                    //   ys[i] = 250 + rand.NextDouble(); 250~251
                    //    if (rand.NextDouble()>0.5)
                    {
                        //   ys[i] = 250 - (rand.NextDouble() * 10);
                        ys[i] = 250;
                        if (seriesIndex == 17 && i == 5)
                        {
                            ys[i] = 245;
                        }
                    }

                }

                plot.AddScatter(xs, ys, label: $"ROLL {seriesIndex + 1}");  // 각 시리즈(라벨) 범례에 추가
            }

            plot.SetAxisLimits(xMin: 0, xMax: 49, yMin: 200, yMax: 300);  //생략시 고정 범위 아니고 자동 범위로 바뀜.
            //x축은 0~49사이만 표시 , y축 0~100 // 만약 사용자가 마우스로 줌/팬을 하게 하고 싶으면 윗 코드 생략하거나 조건부 실행 
            plot.Legend(location: ScottPlot.Alignment.UpperRight);  //범례 위치 제어

            // 축 라벨 추가
            plot.XLabel("시간 (초)");
            plot.YLabel("RPM");

            //반영
            formsPlot1.Refresh();
        }

        private void pic_stop_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("장비를 정지하시겠습니까?", "정지 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
            }
            else
            {
                // AppendLog($"프로그램 비상 정지\t#{+log_cnt}");
                // log_cnt++;
                MessageBox.Show("장비를 정지합니다.", "정지 명령");

            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("종료하시겠습니까?", "종료", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
            }
            else
            {
                Close();
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(serialPort);

            f2.FormSendEvent += new Form2.FormSendDataHandler(DiseaseUpdateEventMethod);
            //form2에 이벤트 추가

            f2.ShowDialog();
            //Show(), ShowDialog() 차이 
            //Show: form2 호출후에도 form1 제어가능 , ShowDialog(): form2 호출후에는 form1 제어불가
        }



        private void logout_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("관리자 모드를 종료하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {

            }
            else
            {
                MessageBox.Show("관리자 모드를 종료합니다.", "관리자 로그아웃");
                sen_btn.Visible = false;
                temp_btn.Visible = false;
                ampare_btn.Visible = false;
                record_btn.Visible = false;
                logout_btn.Visible = false;
                login_btn.Visible = true;
            }
        }

        private void DiseaseUpdateEventMethod(object sender)
        {
            if ("TRUE".Equals(sender.ToString()))
            {
                temp_btn.Visible = true;
                sen_btn.Visible = true;
                ampare_btn.Visible = true;
                record_btn.Visible = true;
                login_btn.Visible = false;
                logout_btn.Visible = true;
            }

        }

        private void temp_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)  //Ver1
            {
                f3 = new Form3();
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethod);
            }

            f3.Show();
            f3.BringToFront();  //이미 열려있다면 앞으로

            if (f3.temp_panel.Visible == true)
                f3.ShowPanel(4);
            else if (f3.temp_panel.Visible == false)
                f3.ShowPanel(1);



        }

        private void sen_btn_Click(object sender, EventArgs e)
        {

            if (f3 == null || f3.IsDisposed)   //Ver1
            {
                f3 = new Form3();
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethod);
            };


            f3.Show();

            f3.BringToFront();  //이미 열려있다면 앞으로

            if (f3.sensor_panel.Visible == true)
                f3.ShowPanel(5);
            else if (f3.sensor_panel.Visible == false)
                f3.ShowPanel(2);

        }

        private void ampare_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)    //Ver1
            {
                f3 = new Form3();
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethod);
            }
            f3.Show();
            f3.BringToFront();  //이미 열려있다면 앞으로

            if (f3.ampare_panel.Visible == true)
                f3.ShowPanel(6);
            else if (f3.ampare_panel.Visible == false)
                f3.ShowPanel(3);


        }

        private void record_btn_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();

            f5.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (str == null)
            {
                MessageBox.Show("COM 설정필요");
                return;
            }

            if (!serialPort.IsOpen)
            {
                serialPort.PortName = "COM103";
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.Parity = System.IO.Ports.Parity.None;
                serialPort.StopBits = System.IO.Ports.StopBits.One;

                serialPort.Open();
                MessageBox.Show("연결 성공");
            }
            else
            {
                MessageBox.Show("이미 연결");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
       
        }
    }
}
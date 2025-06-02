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
                double[] xs = ScottPlot.DataGen.Consecutive(pointCount);    //�ð��� (50��)
                double[] ys = new double[pointCount];

                for (int i = 0; i < pointCount; i++)
                {
                    //    ys[i] = rand.NextDouble() * 100;    //�� y���� 0 ~100 ������ ����
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

                plot.AddScatter(xs, ys, label: $"ROLL {seriesIndex + 1}");  // �� �ø���(��) ���ʿ� �߰�
            }

            plot.SetAxisLimits(xMin: 0, xMax: 49, yMin: 200, yMax: 300);  //������ ���� ���� �ƴϰ� �ڵ� ������ �ٲ�.
            //x���� 0~49���̸� ǥ�� , y�� 0~100 // ���� ����ڰ� ���콺�� ��/���� �ϰ� �ϰ� ������ �� �ڵ� �����ϰų� ���Ǻ� ���� 
            plot.Legend(location: ScottPlot.Alignment.UpperRight);  //���� ��ġ ����

            // �� �� �߰�
            plot.XLabel("�ð� (��)");
            plot.YLabel("RPM");

            //�ݿ�
            formsPlot1.Refresh();
        }

        private void pic_stop_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("��� �����Ͻðڽ��ϱ�?", "���� Ȯ��", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
            }
            else
            {
                // AppendLog($"���α׷� ��� ����\t#{+log_cnt}");
                // log_cnt++;
                MessageBox.Show("��� �����մϴ�.", "���� ���");

            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("�����Ͻðڽ��ϱ�?", "����", MessageBoxButtons.YesNo) != DialogResult.Yes)
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
            //form2�� �̺�Ʈ �߰�

            f2.ShowDialog();
            //Show(), ShowDialog() ���� 
            //Show: form2 ȣ���Ŀ��� form1 ����� , ShowDialog(): form2 ȣ���Ŀ��� form1 ����Ұ�
        }



        private void logout_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("������ ��带 �����Ͻðڽ��ϱ�?", "���� Ȯ��", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {

            }
            else
            {
                MessageBox.Show("������ ��带 �����մϴ�.", "������ �α׾ƿ�");
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
            f3.BringToFront();  //�̹� �����ִٸ� ������

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

            f3.BringToFront();  //�̹� �����ִٸ� ������

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
            f3.BringToFront();  //�̹� �����ִٸ� ������

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
                MessageBox.Show("COM �����ʿ�");
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
                MessageBox.Show("���� ����");
            }
            else
            {
                MessageBox.Show("�̹� ����");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
       
        }
    }
}
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
using System.Text;
using System.Data.SqlClient;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {


        private Form3 f3;


        private byte latestDOByte = 0x00;   //���� �ֱ��� DO raw �� ����

        public SerialPort serialPort;

        string str;

        public Func<byte> GetDOByte;
        private Queue<double> rpm_data = new Queue<double>();


        public Form1()
        {
            InitializeComponent();

            Init_port();

            InitChart();
            RPM_timer.Tick += RPM_Timer;
        }

        private void Init_port()
        {
            serialPort = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);

            byte[] command = new byte[] { 0x04, 0x03, 0x01 };

            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    MessageBox.Show("PORT OPEN");
                    serialPort.Write(command, 0, command.Length);
                    serialPort.DataReceived += serialPort_DataReceived;
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void serialPort_DataReceived(object sender, EventArgs e)
        {
            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);
            string hexString = BitConverter.ToString(buffer).Replace("-", " ");

            try
            {
                this.BeginInvoke((Action)(() =>
                {
                    if (buffer.Length >= 3)
                    {
                        latestDOByte = buffer[2];
                        if (buffer[2] == 0x01)
                        {
                            textBox1.Text += "PORT1 LED ON ";
                            sen_good(sender, e);
                        }
                        else if (buffer[2] == 0x00)
                        {
                            textBox1.Text += "PORT1 LED OFF ";

                            sen_err(sender, e);
                        }
                        else
                            textBox1.Text += "None ";
                    }
                    else
                    {
                        textBox1.AppendText("���� ������ ���� :" + hexString + "\n");
                    }

                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message + "\n");
            }



            //   MessageBox.Show("None");
            //   string hexString = BitConverter.ToString(buffer).Replace("-", " ");
            //   MessageBox.Show($"���ŵ� ������: {hexString}");
        }

        private void sen_good(object sender, EventArgs e)
        {
            roll_num1.BackColor = Color.Blue;
            sen_stat1.Text = "����";
            sen_stat1.ForeColor = Color.White;
            sen_stat1.BackColor = Color.Black;
            if (f3 != null)
            {
                f3.temp_1.BackColor = Color.Black;
                f3.temp_1.ForeColor = Color.White;
                f3.col1_1.BackColor = Color.Black;
                f3.col1_1.ForeColor = Color.White;
            }


        }

        private void sen_err(object sender, EventArgs e)
        {
            roll_num1.BackColor = Color.Red;
            sen_stat1.Text = "ERR";
            sen_stat1.ForeColor = Color.Red;
            sen_stat1.BackColor = Color.Maroon;
            if (f3 != null)
            {
                f3.temp_1.BackColor = Color.Red;
                f3.temp_1.ForeColor = Color.Maroon;
                f3.col1_1.ForeColor = Color.Maroon;
                f3.col1_1.BackColor = Color.Red;
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

        private void RPM_Timer(object sender, EventArgs e)
        {

        }


        private void pic_stop_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("��� �����Ͻðڽ��ϱ�?", "���� Ȯ��", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
            }
            else
            {
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
                if (serialPort.IsOpen)
                {
                    byte[] command = new byte[] { 0x04, 0x03, 0x00 };
                    serialPort.Write(command, 0, command.Length);

                    serialPort.Close();
                }
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                Close();
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();

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
                f3.GetDOByte = GetLatestDOByte;
                f3.Show();

                f3.BringToFront();  //�̹� �����ִٸ� ������
                if (f3.sensor_panel.Visible == true)
                    f3.ShowPanel(5);
                else if (f3.sensor_panel.Visible == false)
                    f3.ShowPanel(2);
            }
            else
                f3.Focus();




           

        }

        private byte GetLatestDOByte()  //f3�� ȣ���� �Լ�
        {
            return latestDOByte;
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
      
    }
}
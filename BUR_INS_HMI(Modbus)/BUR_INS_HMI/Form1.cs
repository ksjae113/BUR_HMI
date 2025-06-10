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
using ScottPlot.Drawing.Colormaps;
using Modbus.Device;
using Microsoft.Win32;
using System.Diagnostics;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {


        private Form3 f3;

        private Label[] roll;
        private Label[] sen_num_arr;
        private Label[] sen_stat_arr;
        private byte latestDOByte = 0x00;   //���� �ֱ��� DO raw �� ����

        public SerialPort serialPort;
        private IModbusSerialMaster _modbusMaster;
        public Func<byte> GetDOByte;

        byte slaveId = 1;
        ushort startAddress = 0x0000;   // 30001
        ushort numInputs = 4;

        private ushort[] prevDI = new ushort[4]; // ���� DI1, DI2 �� ����


        private List<Queue<double>> rpmHistory = new List<Queue<double>>();
        private const int maxPoints = 100;  //�ִ� 100����Ʈ�� ����

        public Form1()
        {
            InitializeComponent();

            InitRPMHistory();

            InitChart();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            roll = new Label[] {roll_num1,roll_num2,roll_num3, roll_num4,roll_num5,roll_num6,roll_num7,roll_num8,
                roll_num9,roll_num10,roll_num11, roll_num12,roll_num13,roll_num14,roll_num15,roll_num16,roll_num17,
                roll_num18,roll_num19,roll_num20,roll_num21,roll_num22, roll_num23,roll_num24,roll_num25,
                roll_num26,roll_num27};
            sen_num_arr = new Label[]{ sen_num1, sen_num2, sen_num3, sen_num4, sen_num5, sen_num6,
                sen_num7, sen_num8, sen_num9, sen_num10 };
            sen_stat_arr = new Label[]{ sen_stat1, sen_stat2, sen_stat3, sen_stat4, sen_stat5, sen_stat6,
                sen_stat7, sen_stat8, sen_stat9, sen_stat10 };
            Init_port();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
                ReadModbusData();
        }

        private void InitRPMHistory()
        {
            for (int i = 0; i < 27; i++)
                rpmHistory.Add(new Queue<double>());
        }

      

        private void Init_port()
        {
            serialPort = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);

            //   byte[] command = new byte[] { 0x04, 0x03, 0x01 };


            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    _modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);
                    _modbusMaster.Transport.ReadTimeout = 200;
                    _modbusMaster.Transport.WriteTimeout = 200;
                    MessageBox.Show("PORT OPEN");
                    //   serialPort.Write(command, 0, command.Length);
                //    serialPort.DataReceived += serialPort_DataReceived;
                    //   MessageBox.Show("Received");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void UpdateRPM(byte doByte)
        {
            int[] rpmValues = new int[27];

            // ������ ���÷� DO ����Ʈ �� �������� RPM ���
            /*   for (int i = 0; i < rpmValues.Length; i++)    
               {
                   // ���� ���� ������ ���� ���� �ٲٱ�
                   bool isOn = ((doByte >> (i % 8)) & 0x01) == 1;
                   rpmValues[i] = isOn ? 245 : 250;
                   //   rpmValues[i] = ((doByte >> (i % 8)) & 0x01) == 1 ? 245 : 250; �� 2�� ���ٷ�
               }*/

            if (doByte == 1)
            {
                // �����ϰ� �ϳ��� �ε����� 245�� ����
                Random rand = new Random();
                int selectedIndex = rand.Next(27);
                for (int i = 0; i < rpmValues.Length; i++)
                {
                    rpmValues[i] = (i == selectedIndex) ? 245 : 250;
                }
            }
            else
            {
                // ���� ��Ĵ�� bit ���� ����
                for (int i = 0; i < rpmValues.Length; i++)
                {
                    rpmValues[i] = ((doByte >> (i % 8)) & 0x01) == 1 ? 245 : 250;
                }
            }

            // �� ä�ο� �� ������ �߰�
            for (int i = 0; i < 27; i++)
            {
                if (rpmHistory[i].Count >= maxPoints)
                    rpmHistory[i].Dequeue();

                rpmHistory[i].Enqueue(rpmValues[i]);
            }

            // �׷��� ������Ʈ
            formsPlot1.Plot.Clear();

            for (int i = 0; i < 27; i++)
            {
                double[] ys = rpmHistory[i].ToArray();
                double[] xs = Enumerable.Range(0, ys.Length).Select(x => (double)x).ToArray();
                formsPlot1.Plot.AddScatter(xs, ys, label: $"ROLL {i + 1}", lineWidth: 1);
            }

            formsPlot1.Plot.Legend();
            formsPlot1.Plot.SetAxisLimits(yMin: 230, yMax: 270);
            formsPlot1.Render();

            // Form3�� ������ ����
            if (f3 != null && !f3.IsDisposed)
            {
                f3.SharedTimerCallback(latestDOByte);
            }
        }

        private volatile bool dataReceivedFlag = false;

        private void serialPort_DataReceived(object sender, EventArgs e)
        { 

            dataReceivedFlag = true;

            /* this.BeginInvoke(new Action(() =>
             {
                 ReadModbusData();
             }));*/




            
        }

        private void ReadModbusData()
        {
            try
            {
                ushort[] data = _modbusMaster.ReadInputRegisters(slaveId, startAddress, numInputs);

                roll_check(data);
                
              
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR:" + ex.Message);
            }
        }

       


        private void roll_check(ushort[] data)
        {
            /* if (data[0] == 1)
             {
                 roll_num1.BackColor = Color.Red;
                 roll_num2.BackColor = Color.Blue;
             }
             else
                 roll_num1.BackColor = Color.Blue;

             if (data[1] == 1)
             {
                 roll_num1.BackColor = Color.Blue;
                 roll_num2.BackColor = Color.Red;
             }
             else
                 roll_num2.BackColor = Color.Blue;
            */

            for (int i = 0; i < data.Length && i < 27 ;  i++)
            {
                if (data[i] == 1)
                {
                    roll[i].BackColor = Color.Red;
                }
                else
                    roll[i].BackColor = Color.Blue;
            }

            Debug.WriteLine("Modbus data received: " + string.Join(", ", data));
        }


        private void sens_check(byte data, int index)
        {

            /*   for (int i = 0; i < 10; i++)
               {
                   if (index == 7)
                   {
                       sen_stat_arr[1].Text = "ERR";
                       sen_stat_arr[1].ForeColor = Color.YellowGreen;
                       sen_stat_arr[1].BackColor = Color.Yellow;
                       if (f3 != null)
                       {
                           f3.temp1_arr[i].BackColor = Color.Red;
                           f3.temp1_arr[i].ForeColor = Color.Maroon;
                           f3.col1_arr[i].BackColor = Color.Red;
                           f3.col1_arr[i].ForeColor = Color.Maroon;
                       }
                   }
                   else if (index == 2)
                   {
                       sen_stat_arr[0].Text = "ERR";
                       sen_stat_arr[0].ForeColor = Color.Red;
                       sen_stat_arr[0].BackColor = Color.Maroon;
                       if (f3 != null)
                       {
                           f3.temp1_arr[i].BackColor = Color.Red;
                           f3.temp1_arr[i].ForeColor = Color.Maroon;
                           f3.col1_arr[i].BackColor = Color.Red;
                           f3.col1_arr[i].ForeColor = Color.Maroon;
                       }
                   }
                   else
                   {
                       sen_stat_arr[i].Text = "����";
                       sen_stat_arr[i].ForeColor = Color.White;
                       sen_stat_arr[i].BackColor = Color.Black;

                       if (f3 != null)
                       {
                           f3.temp1_arr[i].BackColor = Color.Black;
                           f3.temp1_arr[i].ForeColor = Color.White;
                           f3.col1_arr[i].BackColor = Color.Black;
                           f3.col1_arr[i].ForeColor = Color.White;
                       }
                   }*/

            for (int i = 0; i < 10; i++)
            {
                sen_stat_arr[i].Text = "����";
                sen_stat_arr[i].ForeColor = Color.White;
                sen_stat_arr[i].BackColor = Color.Black;

                if (f3 != null)
                {
                    f3.temp1_arr[i].BackColor = Color.Black;
                    f3.temp1_arr[i].ForeColor = Color.White;
                    f3.col1_arr[i].BackColor = Color.Black;
                    f3.col1_arr[i].ForeColor = Color.White;
                }
            }

            if (index == 7)
            {
                sen_stat_arr[1].Text = "ERR";
                sen_stat_arr[1].ForeColor = Color.YellowGreen;
                sen_stat_arr[1].BackColor = Color.Yellow;
                if (f3 != null)
                {
                    f3.temp1_arr[1].BackColor = Color.Red;
                    f3.temp1_arr[1].ForeColor = Color.Maroon;
                    f3.col1_arr[1].BackColor = Color.Red;
                    f3.col1_arr[1].ForeColor = Color.Maroon;
                }
            }
            else if (index == 2)
            {
                sen_stat_arr[0].Text = "ERR";
                sen_stat_arr[0].ForeColor = Color.Red;
                sen_stat_arr[0].BackColor = Color.Maroon;
                if (f3 != null)
                {
                    f3.temp1_arr[0].BackColor = Color.Red;
                    f3.temp1_arr[0].ForeColor = Color.Maroon;
                    f3.col1_arr[0].BackColor = Color.Red;
                    f3.col1_arr[0].ForeColor = Color.Maroon;
                }
            }

        }


        private void InitChart()
        {
            var plt = formsPlot1.Plot;
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.YAxis.ManualTickPositions(new double[] { 225, 250, 275 }, new string[] { "225", "250", "275" });
            double[] xs = ScottPlot.DataGen.Consecutive(50);
            double[] ys = new double[50]; // ��� 0���� �ʱ�ȭ

            plt.AddScatter(xs, ys);
            formsPlot1.Refresh();

            /* var plot = formsPlot1.Plot;
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
             formsPlot1.Refresh();*/
        }



        private void pic_stop_btn_Click(object sender, EventArgs e)
        {

            if (serialPort.IsOpen)
            {
                if (MessageBox.Show("��� �����Ͻðڽ��ϱ�?", "���� Ȯ��", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {

                }
                else
                {
                    MessageBox.Show("��� �����մϴ�.", "���� ���");
                    serialPort.Close();
                    pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.reconnect;
                }
            }
            else //  !serialPort.IsOpen
            {
                pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.estop_on2;
                Init_port();
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

            f2.FormSendEvent += new Form2.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
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

        private void DiseaseUpdateEventMethodF2toF1(object sender)
        {
            if ("TRUE".Equals(sender.ToString()))
            {
                temp_btn.Visible = true;
                sen_btn.Visible = true;
                ampare_btn.Visible = true;
                record_btn.Visible = true;

                login_btn.Visible = false;  //��ɵ� ���� �Ǿ�� �ϹǷ� 2���� Visible�� ����
                logout_btn.Visible = true;
                //    login_btn.Image = BUR_INS_HMI.Properties.Resources.admin_logout_btn; //�̹����� ����Ǵ� ���̹Ƿ� X.
            }

        }



        private void temp_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)  //Ver1
            {
                f3 = new Form3();
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
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
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);

            }

            f3.GetDOByte = GetLatestDOByte;
            f3.Show();

            f3.BringToFront();  //�̹� �����ִٸ� ������
            if (f3.sensor_panel.Visible == true)
                f3.ShowPanel(5);
            else if (f3.sensor_panel.Visible == false)
                f3.ShowPanel(2);
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
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
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

        int count = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (count % 2 == 0)
                {
                    byte[] command = new byte[] { 0x04, 0x03, 0x00 };   //3P off
                    serialPort.Write(command, 0, command.Length);
                    serialPort.DataReceived += serialPort_DataReceived;
                    Thread.Sleep(500);
                    count++;
                }
                else
                {
                    byte[] command = new byte[] { 0x04, 0x03, 0x01 };   //3P on
                    serialPort.Write(command, 0, command.Length);
                    serialPort.DataReceived += serialPort_DataReceived;
                    Thread.Sleep(500);
                    count++;
                }


            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (count % 2 == 0)
                {
                    byte[] command = new byte[] { 0x04, 0x04, 0x00 };   //3P off
                    serialPort.Write(command, 0, command.Length);
                    serialPort.DataReceived += serialPort_DataReceived;
                    Thread.Sleep(500);
                    count++;
                }
                else
                {
                    byte[] command = new byte[] { 0x04, 0x04, 0x01 };   //3P on
                    serialPort.Write(command, 0, command.Length);
                    serialPort.DataReceived += serialPort_DataReceived;
                    Thread.Sleep(500);
                    count++;
                }


            }
        }

        
    }
}
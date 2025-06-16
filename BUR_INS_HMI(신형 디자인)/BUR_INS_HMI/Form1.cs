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
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {


        private Form3 f3;
        internal InfoPanel roll_pan;

        //  private Label[] roll;
        private Panel[] roll;
        private Label[] roll_rpm;
        private PictureBox[] sensor;
        private byte latestDOByte = 0x00;   //가장 최근의 DO raw 값 저장

        public SerialPort serialPort;
        private IModbusSerialMaster _modbusMaster;
        public Func<byte> GetDOByte;

        byte slaveId = 1;
        ushort startAddress = 0x0000;   // 30001
        ushort numInputs = 27;

        int target_rpm;

        public decimal[] amp = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5 }; //index[10]:err_range
        public decimal[] temp = new decimal[] { 30.5M, 30.5M, 30.5M, 30.5M, 30.5M, 30.5M,
            30.5M, 30.5M, 30.5M, 30.5M };

        

        private ushort[] prevDI = new ushort[4]; // 이전 DI1, DI2 값 저장


        private List<Queue<double>> rpmHistory = new List<Queue<double>>();
        private const int maxPoints = 100;  //최대 100포인트만 저장

        public Form1()
        {
            InitializeComponent();

            InitRPMHistory();

            InitChart();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //전체화면 작업표시줄은 존재
         /*   this.FormBorderStyle = FormBorderStyle.None;    //테두리x
            this.WindowState = FormWindowState.Maximized;   //화면 가득 채우기*/

            //전체화면 작업표시줄 없도록
           /* this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.StartPosition = FormStartPosition.Manual;*/

            roll_pan = new InfoPanel();
            roll_pan.Dock = DockStyle.Fill;
            this.info_panel.Controls.Add(roll_pan);

            roll = new Panel[] {roll_pan.roll1, roll_pan.roll2, roll_pan.roll3, roll_pan.roll4,
           roll_pan.roll5,roll_pan.roll6,roll_pan.roll7,roll_pan.roll8,roll_pan.roll9,roll_pan.roll10,
           roll_pan.roll11,roll_pan.roll12,roll_pan.roll13,roll_pan.roll14,roll_pan.roll15,roll_pan.roll16,
           roll_pan.roll17,roll_pan.roll18,roll_pan.roll19,roll_pan.roll20,roll_pan.roll21,roll_pan.roll22,
           roll_pan.roll23,roll_pan.roll24,roll_pan.roll25,roll_pan.roll26,roll_pan.roll27};

            roll_rpm = roll_pan.roll_rpm;

            sensor = new PictureBox[] { pic_sen1, pic_sen2, pic_sen3, pic_sen4, pic_sen5, pic_sen6,
             pic_sen7, pic_sen8, pic_sen9, pic_sen10 };
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                ReadModbusData();
            }
            else
            {
            }
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
                    timer1.Start();
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

            // 간단한 예시로 DO 바이트 값 기준으로 RPM 계산
            /*   for (int i = 0; i < rpmValues.Length; i++)    
               {
                   // 실제 센서 로직에 맞춰 계산식 바꾸기
                   bool isOn = ((doByte >> (i % 8)) & 0x01) == 1;
                   rpmValues[i] = isOn ? 245 : 250;
                   //   rpmValues[i] = ((doByte >> (i % 8)) & 0x01) == 1 ? 245 : 250; 위 2줄 한줄로
               }*/

            if (doByte == 1)
            {
                // 랜덤하게 하나의 인덱스만 245로 설정
                Random rand = new Random();
                int selectedIndex = rand.Next(27);
                for (int i = 0; i < rpmValues.Length; i++)
                {
                    rpmValues[i] = (i == selectedIndex) ? 245 : 250;
                }
            }
            else
            {
                // 기존 방식대로 bit 기준 설정
                for (int i = 0; i < rpmValues.Length; i++)
                {
                    rpmValues[i] = ((doByte >> (i % 8)) & 0x01) == 1 ? 245 : 250;
                }
            }

            // 각 채널에 새 데이터 추가
            for (int i = 0; i < 27; i++)
            {
                if (rpmHistory[i].Count >= maxPoints)
                    rpmHistory[i].Dequeue();

                rpmHistory[i].Enqueue(rpmValues[i]);
            }

            // 그래프 업데이트
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

            // Form3에 데이터 전달
            /*   if (f3 != null && !f3.IsDisposed)
               {
                   f3.SharedTimerCallback(latestDOByte);
               }*/
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

        private bool _hasShownDisconnectMessage = false;
        private void ReadModbusData()
        {
            try
            {
                ushort[] data = _modbusMaster.ReadInputRegisters(slaveId, startAddress, numInputs);

                Random rand = new Random();
                int randnum = rand.Next(4, numInputs);
                if (randnum != 8 && randnum != 11 && randnum != 14)
                {
                    for (int i = 4; i < numInputs; i++)
                    {
                        data[i] = 0x15;
                        data[randnum] = 0x0B;

                    }

                    rpm_check(data);
                    sens_check(data);
                    roll_check(data);
                    copyroll(roll_pan);
                    set_amp(data);
                    getPosition(data);
                }

           /*     rpm_check(data);
                sens_check(data);
                roll_check(data);
                copyroll(roll_pan);
                getPosition(data);*/

                // 정상 통신이 이루어졌으므로 오류 플래그 해제
                _hasShownDisconnectMessage = false;
            }
            catch (TimeoutException)
            {
                if (!_hasShownDisconnectMessage)
                {
                    _hasShownDisconnectMessage = true;
                    MessageBox.Show("통신이 임의로 끊겼습니다.");
                    try
                    {
                        if (serialPort != null && serialPort.IsOpen)
                        {
                            serialPort.Close();
                            MessageBox.Show("통신 종료됨");
                            sens_check(null);

                        }
                        else
                        {
                            MessageBox.Show("통신이 시작되지 않았습니다.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("에러: " + ex.Message);
                    }
                }
            }

            catch (Exception ex)
            {
            }
        }

        private void set_amp(ushort[] data)
        {
            for (int i = 4; i < 14;i++)
            {
                amp[i - 4] = Convert.ToInt32(data[i]);
            }
        }

        private void rpm_check(ushort[] data)
        {
            int avg = 0;

            target_rpm = (int)numericUpDown1.Value;

            avg = (Convert.ToInt32(data[8]) + Convert.ToInt32(data[11]) + Convert.ToInt32(data[14]))/3;

            for (int i = 0; i < numInputs; i++)
            {
                roll_pan.roll_rpm[i].Text = data[i].ToString();
                roll_pan.roll_rpm[i].Visible = (avg >= target_rpm);  // 조건에 따라 true 또는 false
            }
            if (roll_pan.roll_rpm[0].Visible == true)
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
            else
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;




            /*   for (int i = 0; i < data.Length; i++)
               {
                     int rpm = Convert.ToInt32(data[i]);
                     roll_pan.roll_rpm[i].Text = rpm.ToString();
               }

               if (data[0] >= 1)
               {
                   for (int i =0; i < 8; i++)
                   {
                       roll_pan.roll_rpm[i].Text = "20";
                       if (Convert.ToInt32(roll_pan.roll_rpm[i].Text) >=
                           Convert.ToInt32(target_rpm))
                       {
                           roll_pan.roll_rpm[i].Visible = true;
                       }
                       else
                           roll_pan.roll_rpm[i].Visible = false;
                   }
               }

               else if (data[1] >= 1)
               {
                   for (int i =0;i < 16;i++)
                   {
                       roll_pan.roll_rpm[i].Text = "20";
                       if (Convert.ToInt32(roll_pan.roll_rpm[i].Text) >=
                           Convert.ToInt32(target_rpm))
                       {
                           roll_pan.roll_rpm[i].Visible = true;
                       }
                       else
                           roll_pan.roll_rpm[i].Visible = false;
                   }
               }
               else if (data[2] >=1)
               {
                   for (int i = 0; i < 27;i++)
                   {
                       roll_pan.roll_rpm[i].Text = "20";
                       if (Convert.ToInt32(roll_pan.roll_rpm[i].Text) >=
                           Convert.ToInt32(target_rpm))
                       {
                           roll_pan.roll_rpm[i].Visible = true;
                       }
                       else
                           roll_pan.roll_rpm[i].Visible = false;
                   }
               }

               else if (data[3] >= 1)
               {
                   for (int i =0; i< 27; i++)
                   {
                       roll_pan.roll_rpm[i].Text = "0";
                       if (Convert.ToInt32(roll_pan.roll_rpm[i].Text) >=
                           Convert.ToInt32(target_rpm))
                       {
                           roll_pan.roll_rpm[i].Visible = true;
                       }
                       else
                           roll_pan.roll_rpm[i].Visible = false;
                   }
               }*/

        }

        private void getPosition(ushort[] data)
        {

         /*   target_rpm = (int)numericUpDown1.Value;

            if (data[0] == 0 && data[1] == 0 && data[2] == 0 && data[3] == 0)
            {
                pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;

            }
            
            if (data[0] >= 0x01)
            {
                pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                isPosition1Red = true;
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;

                pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
            }
            int rpm8 = int.TryParse(roll_pan.roll_rpm[8].Text, out var r8) ? r8 : 0;
            int rpm11 = int.TryParse(roll_pan.roll_rpm[11].Text, out var r11) ? r11 : 0;
            int rpm14 = int.TryParse(roll_pan.roll_rpm[14].Text, out var r14) ? r14 : 0;
            if (rpm8 >= target_rpm && rpm11 >= target_rpm && rpm14 >= target_rpm && isPosition1Red)
            //data[1]이 0x01
            {
                pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;

            }

            else if (data[2] >= 0x01)
            {
                pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;

                isPosition1Red = false;
            }

            else if (data[3] >= 0x01)
            {
                pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
            }*/
        }

        private void copyroll(InfoPanel p)
        {
            if (f3 != null)
            {
                for (int i = 0; i < 27; i++)
                {
                    f3.infoCopy.roll[i].BackColor = roll[i].BackColor;
                    f3.infoCopy.roll_rpm[i].Visible = roll[i].Visible;
                    f3.infoCopy.roll_rpm[i].Text = roll_rpm[i].Text;
                }
            }
        }


        private void roll_check(ushort[] data)
        {
            int green_min = target_rpm - (int)(target_rpm * 0.1);
            int green_max = target_rpm + (int)(target_rpm * 0.1);
            int yellow_min = target_rpm - (int)(target_rpm * 0.5);
            int yellow_max = target_rpm + (int)(target_rpm * 0.5);
            for (int i = 0; i < data.Length && i < 27 ; i++)
            {


                   if (data[i] >= green_min && data[i] <= green_max)   //RollColor는 어차피 db에서 받아오므로 후에 수정
                   {
                       roll[i].BackColor = Color.ForestGreen;
                   }
                   else if (data[i] >= yellow_min && data[i] <= yellow_max)
                   {
                       roll[i].BackColor = Color.Yellow;
                   }
                   else
                   {
                       roll[i].BackColor = Color.Red;
                   }

            }

            //    Debug.WriteLine("Modbus data received: " + string.Join(", ", data));
        }

        private void sens_check(ushort[] data)
        {
           /* if (!serialPort.IsOpen)   //센서는 전류따라니까 필요 x
            {
                for (int i = 0; i < 10; i++)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                }
            }
            else
            {
                devideIndex(0,2,0);
                devideIndex(3, 4, 1);
                devideIndex(5, 7, 2);
                devideIndex(8, 10, 3);
                devideIndex(11, 12, 4);
                devideIndex(13, 15, 5);
                devideIndex(16, 18, 6);
                devideIndex(19, 20, 7);
                devideIndex(21, 23, 8);
                devideIndex(24, 26, 9);

            }*/
        }

  /*      private void devideIndex(int min, int max, int sen_num)   센서는 전류따라니까 필요x
        {
               int stat = 0;

               for (int i = min; i <= max; i++)
               {
                   if (roll[i].BackColor == Color.Red)
                   {
                       sensor[sen_num].Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                       stat = -1;
                       break;
                   }
                   else if (roll[i].BackColor == Color.Yellow && (stat != -1))
                   {
                       stat = 1;
                   }

                   if (stat == 0)
                       sensor[sen_num].Image = BUR_INS_HMI.Properties.Resources.sen_stat_green;
                   else
                       sensor[sen_num].Image = BUR_INS_HMI.Properties.Resources.sen_stat_yellow;
               }

        }*/


        private void InitChart()
        {
            var plt = formsPlot1.Plot;
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.YAxis.ManualTickPositions(new double[] { 225, 250, 275 }, new string[] { "225", "250", "275" });
            double[] xs = ScottPlot.DataGen.Consecutive(50);
            double[] ys = new double[50]; // 모두 0으로 초기화

            plt.AddScatter(xs, ys);
            formsPlot1.Refresh();

            /* var plot = formsPlot1.Plot;
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
             formsPlot1.Refresh();*/
        }



        private void pic_stop_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    MessageBox.Show("통신 종료됨");
                }
                else
                {
                    MessageBox.Show("통신이 시작되지 않았습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러: " + ex.Message);
            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    MessageBox.Show("통신 종료됨");
                    Close();
                }
                else
                {
                    MessageBox.Show("통신이 시작되지 않았습니다. 종료");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러: " + ex.Message);
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();

            f2.FormSendEvent += new Form2.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
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
                logout_btn.Visible = false;
                login_btn.Visible = true;
                if (f3 != null)
                {
                    f3.err_set_btn.Visible = false;
                    for (int i = 0; i < 10; i++)
                    {
                        f3.set_btn[i].Visible = false;
                    }
                }
            }
        }

        private void DiseaseUpdateEventMethodF2toF1(object sender)
        {
            if ("TRUE".Equals(sender.ToString()))
            {
                /*   temp_btn.Visible = true;
                   ampare_btn.Visible = true;
                   record_btn.Visible = true;*/

                if (f3 != null)
                {
                    f3.err_set_btn.Visible = true;
                    for (int i = 0; i < 10; i++)
                    {
                        f3.set_btn[i].Visible = true;
                    }
                }

                login_btn.Visible = false;  //기능도 변경 되어야 하므로 2개를 Visible로 변경
                logout_btn.Visible = true;
                //    login_btn.Image = BUR_INS_HMI.Properties.Resources.admin_logout_btn; //이미지만 변경되는 것이므로 X.
            }

        }



        private void temp_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)  //Ver1
            {
                f3 = new Form3(amp, temp);
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
            }

            f3.Show();
            f3.BringToFront();  //이미 열려있다면 앞으로

            if (f3.temp_panel.Visible == true)
                f3.ShowPanel(4);
            else if (f3.temp_panel.Visible == false)
                f3.ShowPanel(1);


        }



        private byte GetLatestDOByte()  //f3이 호출할 함수
        {
            return latestDOByte;
        }


        private void ampare_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)    //Ver1
            {
                f3 = new Form3(amp, temp);
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
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

            //  info_panel.Show();
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

        private void runstop_btn_Click(object sender, EventArgs e)
        {
            /*  if (serialPort.IsOpen)
              {
                  if (MessageBox.Show("장비를 정지하시겠습니까?", "정지 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                  {

                  }
                  else
                  {
                      MessageBox.Show("장비를 정지합니다.", "정지 명령");
                      serialPort.Close();
                      sens_check(null);
                  //    pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.reconnect;
                  }
              }
              else //  !serialPort.IsOpen
              {
               //   pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.estop_on2;
                  Init_port();
              }*/

            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    MessageBox.Show("통신 종료됨");
                    sens_check(null);

                }
                else
                {
                    MessageBox.Show("통신이 시작되지 않았습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러: " + ex.Message);
            }
        }

        private void run_btn_Click(object sender, EventArgs e)
        {
            Init_port();
        }

        private void screencap_btn_Click(object sender, EventArgs e)
        {
            Rectangle bounds = System.Windows.Forms.Screen.GetBounds(Point.Empty);
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                //바탕화면 경로 얻기
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                //저장 파일명 지정
                string fileName = $"FulllScreen_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(desktopPath, fileName);


                //이미지 저장
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show("Screen Captured");
            }
        }
    }
}
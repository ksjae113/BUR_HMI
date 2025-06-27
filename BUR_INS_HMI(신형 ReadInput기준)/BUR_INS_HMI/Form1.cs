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
using ScottPlot.WinForms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using ScottPlot.Drawing.Colormaps;
using Modbus.Device;
using Microsoft.Win32;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic.Logging;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {


        private Form3 f3;
        internal InfoPanel roll_pan;
        FormsPlot avg_plot;
        private Panel[] roll;
        private Label[] roll_rpm;
        private PictureBox[] sensor;
        private Label[] info_lbl;
        private byte latestDOByte = 0x00;   //가장 최근의 DO raw 값 저장

        private int login = 0;

        public SerialPort serialPort;
        private ModbusSerialMaster _modbusMaster;
        public Func<byte> GetDOByte;

        byte slaveId = 1;
        ushort startAddress = 0x0000;   // 30001
        ushort numInputs = 36;

        int dog;
        int target_rpm;

        public decimal[] amp = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5 }; //index[10]:err_range
        public decimal[] temp = new decimal[] { 30.5M, 30.5M, 30.5M, 30.5M, 30.5M, 30.5M,
            30.5M, 30.5M, 30.5M, 30.5M };

        public int[] sen = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };


        /*   DateTime measureStartTime;
           List<double> elapsedTimeList = new List<double>();*/

        List<FormsPlot> plots = new();             // 10개의 그래프

        List<List<List<double>>> ys = new();  // [plot][line][values]
        List<List<List<double>>> xs = new();  // [plot][line][indices]


        int sampleCounter = 0;
        bool isMeasuring = false;
        private int[] layout = { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };

        bool prevTrigger = false;  // 직전 DI 상태

        private int tickInterval = 30;

        private ushort[] prevDI = new ushort[4]; // 이전 DI1, DI2 값 저장


        public Form1()
        {
            InitializeComponent();
            //   ShowBarChartWithAverages();
            InitializeGraphs();

            f3 = new Form3(amp, temp);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //전체화면 작업표시줄은 존재
            /*   this.FormBorderStyle = FormBorderStyle.None;    //테두리x
               this.WindowState = FormWindowState.Maximized;   //화면 가득 채우기*/

            //전체화면 작업표시줄 없도록
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.StartPosition = FormStartPosition.Manual;

            avg_plot = new FormsPlot();
            avg_plot.Dock = DockStyle.Fill;

            this.panel2.Controls.Add(avg_plot);
            avg_plot.BringToFront();

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

            info_lbl = new Label[] { info_lbl1,info_lbl2,info_lbl3, info_lbl4,info_lbl5,info_lbl6,
            info_lbl7, info_lbl8, info_lbl9};

            dog = Convert.ToInt32(numericUpDown2.Value);
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
              await  ReadModbusData();
            }
            else
            {
            }
        }




        private void InitializeGraphs()
        {

            ys.Clear();
            xs.Clear();
            plots.Clear();
            flowLayoutPanel1.Controls.Clear();

            int plotWidth = 650;
            int plotHeight = 150;
            int margin = 0;

           //  for (int i = 0; i< 10; i ++)
           for (int i = 0; i < 10; i++)
            {
                int groupIndex = i;

                ys.Add(new List<List<double>>());
                xs.Add(new List<List<double>>());

                var fp = new FormsPlot();
                {
                    fp.Size = new Size(plotWidth, plotHeight);
                    fp.Margin = new Padding(margin);
                }
             //   fp.Size = new Size(plotWidth, plotHeight);
              //  fp.Margin = new Padding(margin);


                fp.Plot.SetAxisLimits(xMin: 0, xMax: layout[groupIndex], yMin: -1, yMax: 30);
                fp.Plot.Title($"Line {(i + 1).ToString("D2")}", size: 12);
                fp.Plot.Layout(left: 20, right: 5, top: 5, bottom: 20);
                fp.Plot.XAxis.TickLabelStyle(fontSize: 10);
                fp.Plot.YAxis.TickLabelStyle(fontSize: 10);

                for (int j = 0; j < layout[groupIndex]; j++)
                {
                    ys[i].Add(new List<double>());
                    xs[i].Add(new List<double>());
                }

                fp.Plot.XLabel("시간(s)");
                fp.Plot.YLabel("RPM");

                plots.Add(fp);

                 this.flowLayoutPanel1.Controls.Add(fp);
              //  fp.BringToFront();
            }
        }

        private void OnNewData(ushort[] data)
        {
            if (data == null || data.Length < 27) return;

            // 현재 트리거 상태
            bool currentTrigger = (data[0] == 1 || data[1] == 1);
            // 측정 시작 (DI 1 또는 2가 1)
            if (currentTrigger)
            {
                if (!isMeasuring)
                {
                    isMeasuring = true;
                    sampleCounter = 0;

                    // 데이터 초기화
                    foreach (var group in ys)
                        foreach (var line in group)
                            line.Clear();

                    foreach (var group in xs)
                        foreach (var line in group)
                            line.Clear();

                    // 평균 Plot이 아직 없으면 초기 생성
                    if (avg_plot == null)
                    {
                        avg_plot = new FormsPlot();
                        this.Controls.Add(avg_plot);
                    }
                }

                // 데이터 누적
                int dataIndex = 0;
                for (int i = 0; i < layout.Length; i++)
                {
                    for (int j = 0; j < layout[i]; j++)
                    {
                        if (dataIndex >= data.Length) break;

                        ys[i][j].Add(data[dataIndex]);
                        xs[i][j].Add(sampleCounter);
                        dataIndex++;
                    }
                }

                sampleCounter++;

                // 시각화 갱신
                UpdateAllPlots();    // 선 그래프
                UpdateBarChart();    // 평균 막대그래프
            }

            // 측정 종료 시
            else if ((data[0] == 0 && data[1] == 0) && isMeasuring)
            {
                isMeasuring = false;
                // 필요 시 avgPlot.Visible = false;
            }
        }

        private void UpdateBarChart()   //평균값 그래프
        {



            if (avg_plot == null) return;

            double[] groupAverages = new double[layout.Length];
            for (int i = 0; i < layout.Length; i++)
            {
                var allValues = ys[i].SelectMany(line => line).ToList();
                groupAverages[i] = allValues.Count > 0 ? allValues.Average() : 0.0;
            }

            string[] groupLabels = Enumerable.Range(1, layout.Length).Select(n => $"G{n}").ToArray();

            var plt = avg_plot.Plot;
            plt.Clear();

            var bar = plt.AddBar(groupAverages);
            bar.FillColor = Color.CornflowerBlue;

            plt.XTicks(Enumerable.Range(0, groupLabels.Length).Select(i => (double)i).ToArray(), groupLabels);
            plt.SetAxisLimits(xMin: -0.5, xMax: layout.Length - 0.5, yMin: -1, yMax: 30);

            // 기준선 (Level)
            double level = Convert.ToDouble(info_lbl9.Text);
            var levelLine = plt.AddHorizontalLine(level);
            levelLine.LineStyle = LineStyle.Dash;
            levelLine.Color = Color.Red;
            levelLine.Label = $"Level {level}";
            plt.Legend();

            plt.Title("평균값 (실시간)");
            avg_plot.Render();
        }


        private void UpdateAllPlots()   //꺾은선 차트
        {
            int roll_cnt = 0;
            for (int i = 0; i < plots.Count; i++)
            {
                var plot = plots[i];
                plot.Plot.Clear();

                for (int j = 0; j < ys[i].Count; j++)
                {
                    var x = xs[i][j].ToArray();
                    var y = ys[i][j].ToArray();
                    var color = GetColor(j);

                    if (x.Length == 0 || y.Length == 0)
                        continue;
                       plot.Plot.AddScatter(x, y, color: color, markerSize: 0, label: $"R{ roll_cnt++ + 1}");

                }

                plot.Plot.Title($"Line {(i + 1).ToString("D2")}");
                plot.Plot.XLabel("시간(s)");
                plot.Plot.YLabel("RPM");

                //  double xMin = Math.Max(0, sampleCounter - 50);
                double xMin = 0;
                double xMax = sampleCounter;

                if (xMax <= xMin)
                    xMax = xMin + 1;

                var ticks = Enumerable.Range(0, (int)((xMax - xMin) / tickInterval) + 1)
                                      .Select(idx => xMin + idx * tickInterval)
                                      .ToArray();

                var labels = ticks.Select(t => $"{t:F0}s").ToArray();

                plot.Plot.XTicks(ticks, labels);

                plot.Plot.Legend(location: Alignment.UpperRight);

                plot.Plot.SetAxisLimits(xMin: xMin, xMax: xMax, yMin: -1, yMax: 30);

                plot.Render();
            }
            roll_cnt = 0;

        }

        private Color GetColor(int index)
        {
            Color[] colors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple };
            return colors[index % colors.Length];
        }


        private void Init_port()
        {
         //   serialPort = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);

            //   byte[] command = new byte[] { 0x04, 0x03, 0x01 };


         //   if (!serialPort.IsOpen)
         //   {
                try
                {
                    serialPort = new SerialPort("COM103")
                    {
                        BaudRate = 115200,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,
                        ReadTimeout = 200
                    };

                    serialPort.Open();
                    _modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);
                 //   _modbusMaster.Transport.ReadTimeout = 200;    //200
                 //  _modbusMaster.Transport.WriteTimeout = 200;
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
          //  }
        }


        private void serialPort_DataReceived(object sender, EventArgs e)
        {

            /* this.BeginInvoke(new Action(() =>
             {
                 ReadModbusData();
             }));*/





        }

        private bool _hasShownDisconnectMessage = false;
    
        //private void ReadModbusData()
        private async Task ReadModbusData()
        {
            try
            {
                //  ushort[] data = _modbusMaster.ReadInputRegisters(slaveId, startAddress, numInputs);
                ushort[] data = await Task.Run(() =>
                {
                    return _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, numInputs);
                });


                Random rand = new Random();
                int randnum = rand.Next(4, numInputs);

                if (randnum != 8 && randnum != 11 && randnum != 14)
                {

                    for (int i = 4; i < numInputs; i++)
                    {
                        data[i] = 0x15;
                        data[randnum] = 0x0B;

                    }
                }

                if (data[3] == 1)
                {
                    for (int i = 4; i < numInputs; i++)
                        data[i] = 0;
                }

           


                rpm_check(data);
                roll_check(data);
                copyroll(roll_pan);
                set_amp_temp(data);
                sens_check(data);
                f3.update_Realamp(amp);
                f3.update_temp(temp);
                getPosition(data);
                OnNewData(data);
                update_info(data);

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

        private void update_info(ushort[] data)
        {
            for (int i =0; i < 8;i++)
            {
                info_lbl[i].Text = data[i + 27].ToString();
            }
        }

        private async Task SaveModbusDataAsync(ushort[] data)
        {
            string connectionString = "Server=localhost;Database=MyDatabase;Trusted_Connection=True;";
            string query = @"
        INSERT INTO InputData (Value0, Value1, Value2, Value3)
        VALUES (@v0, @v1, @v2, @v3);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@v0", data[0]);
                cmd.Parameters.AddWithValue("@v1", data[1]);
                cmd.Parameters.AddWithValue("@v2", data[2]);
                cmd.Parameters.AddWithValue("@v3", data[3]);

                try
                {
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DB 저장 오류: " + ex.Message);
                }
            }
        }

        private void set_amp_temp(ushort[] data)
        {
            for (int i = 0; i < 10; i++)
            {
                amp[i] = Convert.ToInt32(data[i]);
                temp[i] = Convert.ToInt32(data[i]);
            }
        }

        private void rpm_check(ushort[] data)
        {
            int avg = 0;
            bool updown;

            target_rpm = (int)numericUpDown1.Value;

            avg = (Convert.ToInt32(data[8]) + Convert.ToInt32(data[11]) + Convert.ToInt32(data[14])) / 3;


            for (int i = 0; i < 27; i++)    //교정중이랑 RPm visible은 별개로 처리하도록 수정하기
            {
                roll_pan.roll_rpm[i].Text = data[i].ToString();

                roll_pan.roll_rpm[i].Visible = (avg >= target_rpm) ? true : false;  // 조건에 따라 true 또는 false
            }
            if (roll_pan.roll_rpm[8].Visible == true && roll_pan.roll_rpm[11].Visible == true &&
                roll_pan.roll_rpm[14].Visible == true )

                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
            else
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
            /*   for (int i = 0; i < 27; i++)
               {
                   roll_pan.roll_rpm[i].Text = data[i].ToString();

                   roll_pan.roll_rpm[i].Visible = (avg >= target_rpm) ? true : false;  // 조건에 따라 true 또는 false
               }
               if (roll_pan.roll_rpm[0].Visible == true)

                   pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
               else
                   pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
            */



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
                    f3.infoCopy.roll_rpm[i].Visible = roll_rpm[i].Visible;
                    f3.infoCopy.roll_rpm[i].Text = roll_rpm[i].Text;
                }
            }
        }


        private void roll_check(ushort[] data)
        {
            int green_min = target_rpm - (int)(target_rpm * 0.1);
            int green_max = target_rpm + (int)(target_rpm * 0.1);
            int yellow = 15;
            for (int i = 0; i < data.Length && i < 27; i++)
            {


                if (data[i] >= green_min && data[i] <= green_max)   //RollColor는 어차피 db에서 받아오므로 후에 수정
                {
                    roll[i].BackColor = Color.ForestGreen;
                }
                else if (data[i] >= yellow && data[i] <= yellow)
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
            if (!serialPort.IsOpen)   //센서는 전류따라니까 필요 x
            {
                for (int i = 0; i < 10; i++)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (amp[i] >= 20)
                    {
                        sen[i] = 0;
                    }
                    else
                        sen[i] = 2;
                    //    else
                    //        sen[i] = -1;
                }

                sen_img(sen);

            }

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

        private void sen_img(int[] stat)    //Red,Yellow,Green 기준은 추후 수정
        {
            for (int i = 0; i < 10; i++)
            {
                if (stat[i] == 0)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_green;
                    f3.real_amp[i].ForeColor = Color.Blue;
                }
                else if (stat[i] == 2)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
                    f3.real_amp[i].ForeColor = Color.Red;
                }
                else if (stat[i] == -1)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
                    f3.real_amp[i].ForeColor = Color.Blue;

                }
            }
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
                dog_lbl.Visible = false;
                numericUpDown2.Visible = false;
                login = -1;
                if (f3 != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        f3.set_btn[i].Visible = false;
                    }
                    f3.err_set_btn.Visible = false;
                    f3.amp_set_btn.Visible = false;
                }
            }
        }

        private void DiseaseUpdateEventMethodF2toF1(object sender)
        {
            if ("TRUE".Equals(sender.ToString()))
            {
                login_btn.Visible = false;  //기능도 변경 되어야 하므로 2개를 Visible로 변경
                logout_btn.Visible = true;
                dog_lbl.Visible = true;
                numericUpDown2.Visible = true;
                login = 1;
                
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

            if (login == 1)
            {

                for (int i = 0; i < 10; i++)
                {
                    f3.set_btn[i].Visible = true;
                }
                f3.err_set_btn.Visible = true;
                f3.amp_set_btn.Visible = true;
                login_btn.Visible = false;  //기능도 변경 되어야 하므로 2개를 Visible로 변경
                logout_btn.Visible = true;

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


        private void runstop_btn_Click(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();

            f5.Show();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBox1.SelectedItem.ToString(), out int val))
            {
                tickInterval = val;
                
                UpdateAllPlots();  // 변경 후 그래프 갱신
            }
        }
    }
}
using System;
using System.Data;
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
using ScottPlot.Control.EventProcess;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualBasic.ApplicationServices;
using ScottPlot.Renderable;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {
        private Form3 f3;
        public Label[] roll_num;
        FormsPlot avg_plot;
        private Panel[] roll;
        private Label[] roll_rpm;
        private PictureBox[] sensor;
        private Label[] info_lbl;
        private byte latestDOByte = 0x00;   //가장 최근의 DO raw 값 저장

        private bool IsWarningOpen = false;

        private string password = "0000";

        public SerialPort serialPort;
        private ModbusSerialMaster _modbusMaster;
        public Func<byte> GetDOByte;

        private Stopwatch stopwatch = Stopwatch.StartNew();
        private int renderIntervalMs = 150;

        private int roll_good = 0;
        public bool get_message = false;

        byte slaveId = 1;
        ushort startAddress = 0x0000;   // 30001
        ushort numInputs = 36;

        int dog;

        internal decimal[] target_rpm = new decimal[] { 2 };
        public decimal[] amp = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5 }; //index[10]:err_range
        public decimal[] temp = new decimal[] { 30.5M, 30.5M, 30.5M, 30.5M, 30.5M, 30.5M,
            30.5M, 30.5M, 30.5M, 30.5M };
        public decimal[] std_amp = new decimal[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
        public int[] sen = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        bool estop = false;

        /*   DateTime measureStartTime;
           List<double> elapsedTimeList = new List<double>();*/

        List<FormsPlot> plots = new();             // 10개의 그래프

        List<List<List<double>>> ys = new();  // [plot][line][values]
        List<List<List<double>>> xs = new();  // [plot][line][indices]


        int sampleCounter = 0;
        bool isMeasuring = false;
        private int[] layout = { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };

        bool prevTrigger = false;  

        private double sampleInterval = 0.1;
        private double xRangeSeconds = 300;

        private ushort[] prevDI = new ushort[4]; // 이전 DI1, DI2 값 저장

        private CancellationTokenSource _cancellationTokenSource;
        private Task _modbusPollingTask;

        private bool isReading = false;

        private System.Windows.Forms.Timer blinkTimer;
        private bool isFlashing = false;

        private bool ExitFlag = false;

        MssqlLib mssql = new MssqlLib();

        public Form1()
        {
            InitializeComponent();
            //   ShowBarChartWithAverages();
            InitializeGraphs();

            f3 = new Form3(amp, temp, std_amp, target_rpm);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //전체화면 작업표시줄은 존재
            /*   this.FormBorderStyle = FormBorderStyle.None;    //테두리x
               this.WindowState = FormWindowState.Maximized;   //화면 가득 채우기*/

            //전체화면 작업표시줄 X
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.StartPosition = FormStartPosition.Manual;

            avg_plot = new FormsPlot();
            avg_plot.Dock = DockStyle.Fill;

            this.panel2.Controls.Add(avg_plot);
            avg_plot.BringToFront();

            roll = new Panel[] {roll1, roll2, roll3, roll4,
           roll5,roll6,roll7,roll8,roll9,roll10,
           roll11,roll12,roll13,roll14,roll15,roll16,
           roll17,roll18,roll19,roll20,roll21,roll22,
           roll23,roll24,roll25,roll26,roll27};

        

            sensor = new PictureBox[] { pic_sen1, pic_sen2, pic_sen3, pic_sen4, pic_sen5, pic_sen6,
             pic_sen7, pic_sen8, pic_sen9, pic_sen10 };

            info_lbl = new Label[] { info_lbl1,info_lbl2,info_lbl3, info_lbl4,info_lbl5,info_lbl6,
            info_lbl7, info_lbl8, info_lbl9};

            roll_num = new Label[] {
            roll_num1, roll_num2, roll_num3, roll_num4, roll_num5, roll_num6,
            roll_num7, roll_num8, roll_num9, roll_num10, roll_num11, roll_num12,
            roll_num13, roll_num14, roll_num15, roll_num16, roll_num17, roll_num18,
            roll_num19, roll_num20, roll_num21, roll_num22, roll_num23, roll_num24,
            roll_num25, roll_num26, roll_num27
        };

            roll_rpm = new Label[]
            {
                roll_rpm1,roll_rpm2,roll_rpm3,roll_rpm4,roll_rpm5,roll_rpm6,roll_rpm7,roll_rpm8,
                roll_rpm9,roll_rpm10,roll_rpm11,roll_rpm12,roll_rpm13,roll_rpm14,roll_rpm15,roll_rpm16,
                roll_rpm17,roll_rpm18,roll_rpm19,roll_rpm20,roll_rpm21,roll_rpm22,roll_rpm23,roll_rpm24,
                roll_rpm25,roll_rpm26,roll_rpm27
            };

            roll = new Panel[] { roll1,roll2,roll3,roll4,roll5,roll6,roll7,roll8,roll9,
            roll10,roll11,roll12,roll13,roll14,roll15, roll16,roll17,roll18,roll19,
            roll20,roll21,roll22,roll23,roll24,roll25,roll26,roll27};

            dog = Convert.ToInt32(numericUpDown2.Value);
        }

        private async Task ModbusPollingLoop(CancellationToken token)
        {

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (serialPort == null || !serialPort.IsOpen) //지우니까안됨
                    {
                        ExitFlag = true;
                        //   throw new IOException("포트가 연결되어 있지 않습니다."); //밑에 MessageBox랑 한 셋트
                        throw new IOException("PORT not connected");
                    }
                    await ReadModbusData(token);
                    
                }
                catch (Exception ex)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        ExitFlag = true;
                        MessageBox.Show("Modbus error : " + ex.Message); //message : Mod 오류 : 포트 연결 x 로 세트
                        end_init();
                    }));
                    //   await StopCommunicationAsync();
                    await ReadModbusData(token);
                    break;
                }

                try
                {
                    await (Task.Delay(100, token));
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }



            /*   while (!token.IsCancellationRequested)    //기존
               {
                   try
                   {
                       if (serialPort != null && serialPort.IsOpen)
                           //     await ReadModbusData();
                           await ReadModbusData(token);
                   }
                   catch (Exception ex)
                   {
                       Invoke((MethodInvoker)(() =>
                       {
                           MessageBox.Show("Modbus 오류: " + ex.Message);
                       }));
                   }

                   try
                   {
                       await Task.Delay(100, token); // 100ms 주기 / timer.Interval을 대체
                   }
                   catch (TaskCanceledException)
                   {
                       break;
                   }
               }

               Invoke((MethodInvoker)(() =>
               {
                   MessageBox.Show("데이터 수집 중지됨");
               }));*/

            /* while (!token.IsCancellationRequested)
             {
                 if (serialPort.IsOpen)
                 {
                     await ReadModbusData(); //비동기 데이터 읽기
                 }
                 await Task.Delay(100, token);   //주기. timer.Interval을 대체
             }*/
        }

        string csvFilePath = @"C:\Users\tjlee\Desktop\ksj\storage\modbus_log.csv";
        string folderPath = @"C:\\Users\tjlee\Desktop\ksj\storage";

        private void SaveDataToCSV(ushort[] data)
        {

            string folderPath = @"C:\Users\tjlee\Desktop\ksj\storage";
            string dateString = DateTime.Now.ToString("yyyy_MM_dd");    //날짜별 로그 준비
            string fileName = $"modbus_log_{dateString}.csv";
            string csvFilePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!File.Exists(csvFilePath))
            {
                using (StreamWriter sw = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                {
                    sw.WriteLine("Timestamp,Value1,Value2,Value3,value4,value5,value6,value7,value8,value9," +
                        "value10,value11,value12,value13,value14,value15,value16,value17,value18,value19,value20," +
                        "value21,value22,value23,value24,value25,value26,value27,value28,value29,value30"); // 헤더
                }
            }

            using (StreamWriter sw = new StreamWriter(csvFilePath, true, Encoding.UTF8))
            {
                string timestamp = "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";    //로그 찍히는 시간

                // join된 line이 줄바꿈 포함하지 않도록
                string line = timestamp + "," + string.Join(",", data.Select(d => d.ToString()));
                sw.WriteLine(line);
            }

        }

        private void SaveDataToSQL(ushort[] data)   //미완
        {
            try
            {
                for (int i = 0; i < data.Length; i++)
                {
                    /* int id = i + 1;
                     string value = data[i].ToString();

                     mssql.InsertDB(id, value);*/
                    if (data.Length < 36) return;

                    int[] intData = data.Take(36).Select(x => (int)x).ToArray();

                    // DB에 저장
                    mssql.InsertDB(intData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB 저장 실패: " + ex.Message);
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

                /*   fp.Plot.SetAxisLimits(xMin: 0, xMax: layout[groupIndex], yMin: -1, yMax: 30);*/
                fp.Plot.SetAxisLimits(xMin: 0, xMax: xRangeSeconds, yMin: -1, yMax: 30);
                fp.Plot.Title($"{(i + 1).ToString("D2")}", size: 20);
                fp.Plot.Layout(left: 20, right: 5, top: 5, bottom: 20);
                fp.Plot.XAxis.TickLabelStyle(fontSize: 10);
                fp.Plot.YAxis.TickLabelStyle(fontSize: 10);

                for (int j = 0; j < layout[groupIndex]; j++)
                {
                    ys[i].Add(new List<double>());
                    xs[i].Add(new List<double>());
                }

                //   fp.Plot.XLabel("시간(s)");
                //   fp.Plot.YLabel("RPM");

                plots.Add(fp);

                this.flowLayoutPanel1.Controls.Add(fp);

            }
        }

        private void OnNewData(ushort[] data)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnNewData(data)));
                return;
            }

            if (data == null || data.Length < 27)
                return;

            if (!isMeasuring)
            {
                isMeasuring = true;
                stopwatch.Restart();
            }

            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

            if (elapsedSeconds > xRangeSeconds)
            {
                stopwatch.Restart();
                ClearAllData();
                return; // 다음 데이터부터 다시 시작
            }

            int dataIndex = 0;
            for (int i = 0; i < layout.Length; i++)
            {
                for (int j = 0; j < layout[i]; j++)
                {
                    if (dataIndex >= data.Length) break;
                    xs[i][j].Add(elapsedSeconds);
                    ys[i][j].Add(data[dataIndex]);
                    dataIndex++;
                }
            }

            UpdateAllPlots();   //선 그래프
            UpdateBarChart();   //막대 그래프

            /*   if (InvokeRequired)       //범위 딱 안잡혀있는그래프
               {
                   BeginInvoke(new Action(() => OnNewData(data)));
                   return;
               }

               if (data == null || data.Length < 27)
                   return;

               if (!isMeasuring)
               {
                   isMeasuring = true;
                   stopwatch.Restart();
               }

               double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

               // 범위 초과 시: 완전 리셋
               if (elapsedSeconds > xRangeSeconds)
               {
                   stopwatch.Restart();
                   elapsedSeconds = 0;

                   // 모든 데이터 초기화
                   foreach (var group in xs)
                       foreach (var list in group)
                           list.Clear();
                   foreach (var group in ys)
                       foreach (var list in group)
                           list.Clear();

                   // 플롯 리셋
                   for (int i = 0; i < plots.Count; i++)
                   {
                       plots[i].Plot.Clear();
                       plots[i].Plot.SetAxisLimits(xMin: 0, xMax: xRangeSeconds, yMin: -1, yMax: 30);
                       plots[i].Render(); // <- 중요
                   }

                   return; // 다음 프레임부터 다시 그리기 시작
               }

               // ---- 데이터 누적 ----
               int dataIndex = 0;
               for (int i = 0; i < layout.Length; i++)
               {
                   for (int j = 0; j < layout[i]; j++)
                   {
                       if (dataIndex >= data.Length) break;

                       xs[i][j].Add(elapsedSeconds);
                       ys[i][j].Add(data[dataIndex]);
                       dataIndex++;
                   }
               }

               UpdateAllPlots(); // 선 그래프
               UpdateBarChart(); // 막대 그래프 (필요시)*/

            /*
               if (InvokeRequired)
               {
                   BeginInvoke(new Action(() => OnNewData(data)));

                   return;
               }

               if (data == null || data.Length < 27) return;

               // 현재 트리거 상태
               bool currentTrigger = (data[2] == 1 || data[6] == 1);
               // 측정 시작 (DI 1 또는 2가 1)
               if (currentTrigger)
               {
                   if (!isMeasuring)
                   {
                   //    isMeasuring = true;

                       sampleCounter = 0;
                       ClearAllData();
                       // 데이터 초기화

                   }

                   // 데이터 누적


                   double currentTime = sampleCounter * sampleInterval;

                   if (currentTime > xRangeSeconds)
                   {
                       sampleCounter = 0;

                       foreach (var group in ys)
                           foreach (var line in group)
                               line.Clear();

                       foreach (var group in xs)
                           foreach (var line in group)
                               line.Clear();

                       currentTime = 0;
                   }



                   int dataIndex = 0;
                   for (int i = 0; i < layout.Length; i++)
                   {
                       for (int j = 0; j < layout[i]; j++)
                       {
                           if (dataIndex >= data.Length) break;

                           // 슬라이딩 윈도우 유지 (시간 기준)
                           //    while (xs[i][j].Count > 0 && xs[i][j][0] < sampleCounter - xRangeSeconds)

                           //   xs[i][j].Add(sampleCounter);    // 시간값 추가 (혹은 정확한 timestamp)
                           xs[i][j].Add(currentTime);
                           ys[i][j].Add(data[dataIndex]);// 데이터값 추가
                           dataIndex++;
                       }
                   }

                   sampleCounter++;
                   if (stopwatch.ElapsedMilliseconds >= renderIntervalMs)
                   {
                       UpdateAllPlots();    // 선 그래프
                       UpdateBarChart();    // 평균 막대그래프
                       stopwatch.Restart();
                   }
               }

               // 측정 종료 시
               else if ((data[0] == 0 && data[1] == 0) && isMeasuring)
               {
                   isMeasuring = false;
                   // 필요 시 avgPlot.Visible = false;
               }*/
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

            string[] groupLabels = Enumerable.Range(1, layout.Length).Select(n => $"L{n}").ToArray();

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

            plt.Title("Line 평균 선속도");
            avg_plot.Render();
        }


        private void UpdateAllPlots()   //꺾은선 차트
        {

            int roll_cnt = 0;    // 줌 인아웃이안됨
            for (int i = 0; i < plots.Count; i++)
            {
                var plot = plots[i];
                plot.Plot.Clear();

                for (int j = 0; j < ys[i].Count; j++)
                {
                    var x = xs[i][j].ToArray();
                    var y = ys[i][j].ToArray();
                    if (x.Length == 0 || y.Length == 0) continue;

                    var color = GetColor(j);
                    //    plot.Plot.AddScatter(x, y, color: color, markerSize: 0, label: $"R{roll_cnt++ + 1}");
                    plot.Plot.AddScatter(x, y, color: color, markerSize: 0, label: $"{i + 1}-{j + 1}"); //1-1 1-2로 변경
                }

                plot.Plot.Title($"{(i + 1):D2}", size : 20);
                //    plot.Plot.XLabel("시간(s)");
                //    plot.Plot.YLabel("RPM");

                // 눈금 간격은 30초 고정
                int tickStep = 30;
                var ticks = Enumerable.Range(0, (int)xRangeSeconds / tickStep + 1)
                                      .Select(t => (double)(t * tickStep))
                                      .ToArray();
                //    var labels = ticks.Select(t => $"{t}s").ToArray();    
                //    plot.Plot.XTicks(ticks, labels);                     

                //    plot.Plot.SetAxisLimits(xMin: 0, xMax: xRangeSeconds, yMin: -1, yMax: 30);
                plot.Plot.Legend(location: Alignment.UpperRight);
                plot.Render();
            }

            /*  for (int i = 0; i < plots.Count; i++) //범위딱안잡혀있는
              {
                  var plot = plots[i];
                  plot.Plot.Clear();

                  for (int j = 0; j < ys[i].Count; j++)
                  {
                      var x = xs[i][j].ToArray();
                      var y = ys[i][j].ToArray();

                      if (x.Length == 0 || y.Length == 0) continue;

                      var color = GetColor(j);
                      plot.Plot.AddScatter(x, y, color: color, markerSize: 0, label: $"R{j + 1}");
                  }

                  plot.Plot.Title($"Line {(i + 1):D2}");
                  plot.Plot.XLabel("시간 (s)");
                  plot.Plot.YLabel("RPM");

                  double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                  double xMin = Math.Max(0, elapsedSeconds - xRangeSeconds);
                  double xMax = elapsedSeconds;

                  // X축 눈금 설정 (예: 30초 단위)
                  double tickStep = 30;
                  int tickCount = (int)((xMax - xMin) / tickStep) + 1;
                  var ticks = Enumerable.Range(0, tickCount)
                                        .Select(idx => xMin + idx * tickStep)
                                        .ToArray();

                  var labels = ticks.Select(t => $"{(t - xMin):F0}s").ToArray();

                  plot.Plot.XTicks(ticks, labels);
                  plot.Plot.Legend(location: Alignment.UpperRight);
                  plot.Plot.SetAxisLimits(xMin: xMin, xMax: xMax, yMin: -1, yMax: 30);
                  plot.Render();
              }*/



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

            try
            {
                if (serialPort == null || !serialPort.IsOpen)
                {
                    serialPort = new SerialPort("COM103")
                    {
                        BaudRate = 9600,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,
                        ReadTimeout = 100
                    };

                    serialPort.Open();
                    _modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);

                    isReading = true;
                    MessageBox.Show("PORT OPEN");
                    run_btn.Image = BUR_INS_HMI.Properties.Resources.runon_btn_red;
                    runstop_btn.Image = BUR_INS_HMI.Properties.Resources.runstop_btn_long;
                    get_message = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void serialPort_DataReceived(object sender, EventArgs e)
        {

            /* this.BeginInvoke(new Action(() =>
             {
                 ReadModbusData();
             }));*/
        }

        private bool _hasShownDisconnectMessage = false;

        private async Task ReadModbusData(CancellationToken token)
        {
            byte[] deviceAddress = { 1 };   //slave ID 

            for (int i = 0; i < deviceAddress.Length; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                try
                {
                    //  ushort[] data = _modbusMaster.ReadInputRegisters(slaveId, startAddress, numInputs);
                    ushort[] data = await Task.Run(() =>
                    {
                        if (token.IsCancellationRequested)
                            return null;

                        //   return _modbusMaster.ReadInputRegisters(slaveId, startAddress, numInputs);
                        return _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, numInputs);
                        //Holding으로 바꿈
                    });

                    if (token.IsCancellationRequested || data == null)
                        return;



                    //아래 UI 관련 작업은 UI 스레드에서 안전하게 실행


                    Invoke(new Action(() =>
                    {
                        roll_check(data);
                        rpm_check(data);
                        set_amp_temp(data);
                        sens_check(data);
                        f3.update_Realamp(amp);
                        f3.update_temp(temp);
                        //    getPosition(data);
                        OnNewData(data);
                        update_info(data);
                        SaveDataToCSV(data);
                        SaveDataToSQL(data);
                    }));

                    // 정상 통신이 이루어졌으므로 오류 플래그 해제
                    _hasShownDisconnectMessage = false;
                }
                catch (TimeoutException)
                {
                    if (!_hasShownDisconnectMessage)
                    {
                        _hasShownDisconnectMessage = true;
                        
                        if (ExitFlag != true)
                            Invoke(() => MessageBox.Show("통신이 임의로 끊겼습니다."));
                        ExitFlag = true;


                        if (serialPort != null && serialPort.IsOpen)
                        {
                            serialPort.Close();
                            
                            if (ExitFlag != true)
                                Invoke(() => MessageBox.Show("통신이 종료되었습니다."));
                            ExitFlag = true;

                        }
                        //    await StopCommunicationAsync();
                        Invoke(() => end_init());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ReadModbusData Exception: " + ex.Message);
                }
            }
        }

        private void update_info(ushort[] data)
        {
            //   if (this.InvokeRequired)
            if (InvokeRequired)
            {
                //   this.Invoke(new Action(() => update_info(data)));
                BeginInvoke(new Action(() => update_info(data)));
                return;
            }

            if (data == null)
            {
                for (int i = 0; i < 8; i++)
                {
                    info_lbl[i].Text = "-";
                }
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                info_lbl[i].Text = data[i + 27].ToString();
            }
        }

      

        private void set_amp_temp(ushort[] data)
        {
            //  if (this.InvokeRequired)
            if (InvokeRequired)
            {
                //   this.Invoke(new Action(() => set_amp_temp(data)));
                BeginInvoke(new Action(() => set_amp_temp(data)));
                return;
            }

            if (data == null)
            {
                for (int i = 0; i < 10; i++)
                {
                    amp[i] = temp[i] = -1;
                }
                return;
            }

            for (int i = 0; i < 10; i++)
            {
                amp[i] = Convert.ToInt32(data[i]);
                temp[i] = Convert.ToInt32(data[i]);
            }
        }

        private void rpm_check(ushort[] data)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => rpm_check(data)));
                return;
            }

            /*   if (this.InvokeRequired)
               {
                   this.Invoke(new Action(() => rpm_check(data)));
                   return;
               }*/

            if (data == null)
            {
                for (int i = 0; i < 27; i++)
                {
                    roll_rpm[i].Text = "-";
                    roll_rpm[i].Visible = false;
                }
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
                return;
            }

            decimal avg = 0;
            bool updown;



            avg = (Convert.ToDecimal(data[8]) + Convert.ToDecimal(data[11]) + Convert.ToDecimal(data[14])) / 3;


            for (int i = 0; i < 27; i++)    //교정중이랑 RPm visible은 별개로 처리하도록 수정하기
            {
                roll_rpm[i].Text = data[i].ToString();

                roll_rpm[i].Visible = (avg >= target_rpm[0]) ? true : false;  // 조건에 따라 true 또는 false
            }
            if (roll_rpm[8].Visible == true && roll_rpm[11].Visible == true &&
                roll_rpm[14].Visible == true)

                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_realred_darkdark;
            else
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
            /*   for (int i = 0; i < 27; i++)
               {
                   roll_rpm[i].Text = data[i].ToString();

                   roll_rpm[i].Visible = (avg >= target_rpm) ? true : false;  // 조건에 따라 true 또는 false
               }
               if (roll_rpm[0].Visible == true)

                   pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_red;
               else
                   pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none;
            */





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
               int rpm8 = int.TryParse(roll_rpm[8].Text, out var r8) ? r8 : 0;
               int rpm11 = int.TryParse(roll_rpm[11].Text, out var r11) ? r11 : 0;
               int rpm14 = int.TryParse(roll_rpm[14].Text, out var r14) ? r14 : 0;
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


        private void roll_check(ushort[] data)
        {
            
            //   if (this.InvokeRequired)
            if (InvokeRequired)
            {
                //   this.Invoke(new Action(() => roll_check(data)));
                BeginInvoke(new Action(() => roll_check(data)));
                return;
            }

            if (data == null)
            {
                for (int i = 0; i < 27; i++)
                {
                    roll[i].BackColor = Color.ForestGreen;
                }
                return;
            }

            int green_min = (int)(target_rpm[0] - (target_rpm[0] * 0.1M));
            int green_max = (int)(target_rpm[0] + (target_rpm[0] * 0.1M));
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
                    if (roll_good != 1)
                        roll_good = 2;
                }
                else
                {
                    roll[i].BackColor = Color.Red;
                    roll_good = 1;
                }


            }
            //   if (roll_good == 1 && get_message == false) //get_message가 stop_btn되기전까지 true.


          /*  if (roll_good == 1 && get_message == false)
            {
                if (IsWarningOpen == false)
                {
                    show_warning("RED");
                }
               // show_warning("RED");
                //     get_message = true;
            }
            else if (roll_good == 2 && get_message == false)
            {
                if (IsWarningOpen == false)
                {
                    show_warning("YELLOW");
                }

                // show_warning("YELLOW");
                // get_message = true;
            }*/


            /*   if (data[2] == 1 && get_message == false)  
               {
                   show_warning("YELLOW");
                   get_message = true;
               }
               else if (roll_good == 2 && get_message == false)
               {
                   show_warning("YELLOW");
                   get_message = true;
               }*/
        }
        private void show_warning(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 1000;
            blinkTimer.Tick += (s, e) =>
            {
                if (message == "RED")
                {
                    IsWarningOpen = true;
                    panel3.Visible = false;
                    warning_pan.Visible = true;
                    warning_pan.Dock = DockStyle.Right;
                 //   BringToFront();
                    warning_pan.BackColor = isFlashing ? Color.Black : Color.Red;
                    warning_pic.BackColor = isFlashing ? Color.Black : Color.Red;
                    warning_time.Text = $"{timestamp}";
                    warning_lbl.Text = "구동 불량 발생";
                    warning_lbl.ForeColor = isFlashing ? Color.Red : Color.Black;
                }
                else if (message == "YELLOW")
                {
                    IsWarningOpen = true;
                    panel3.Visible = false;
                    warning_pan.Visible = true;
                    warning_pan.Dock = DockStyle.Right;
                 //   BringToFront();
                    warning_pan.BackColor = isFlashing ? Color.Black : Color.Yellow;
                    warning_pic.BackColor = isFlashing ? Color.Black : Color.Yellow;
                    warning_time.Text = $"{timestamp}";
                    warning_lbl.Text = "구동 경고 발생";
                    warning_lbl.ForeColor = isFlashing ? Color.Yellow : Color.Black;
                }
                isFlashing = !isFlashing;
            };
            blinkTimer.Start();
        }

        private void sens_check(ushort[] data)
        {
            //   if (this.InvokeRequired)
            if (InvokeRequired)
            {
                //   this.Invoke(new Action(() => sens_check(data)));
                BeginInvoke(new Action(() => sens_check(data)));
                return;
            }

            if (data == null)   //센서는 전류따라니까 추후 수정 필요할 느낌
            {
                for (int i = 0; i < 10; i++)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
                }
                return;
            }

            if (!serialPort.IsOpen)   //센서는 전류따라니까 필요 x
            {
                for (int i = 0; i < 10; i++)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (amp[i] <= f3.max[i] && amp[i] >= f3.min[i]) //GREEN   //확인 완료 
                        sen[i] = 0;
                    else
                        sen[i] = 2; //RED

                    /*    if (amp[i] == 1)   //GREEN    //f5 조건식 실험용 
                        {
                            sen[i] = 0;
                        }
                        else
                            sen[i] = 2; //RED*/
                }

                sen_img(sen);

            }
        }

        private void sen_img(int[] stat)    //Red,Yellow,Green 기준은 추후 수정
        {
            for (int i = 0; i < 10; i++)
            {
                if (stat[i] == 0)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_green_control2;
                    f3.real_amp[i].ForeColor = Color.Blue;
                }
                else if (stat[i] == 2)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_realred_control;
                    f3.real_amp[i].ForeColor = Color.Red;
                }
                else if (stat[i] == -1)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
                    f3.real_amp[i].ForeColor = Color.Blue;

                }
            }
        }



        private async void pic_stop_btn_Click(object sender, EventArgs e)   //이거 왜있는거지 pic_stop없는데
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                if (_modbusPollingTask != null)
                    await _modbusPollingTask;

                if (serialPort != null && serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.DiscardInBuffer();
                        serialPort.DiscardOutBuffer();
                        serialPort.Close();
                    }
                    catch (Exception closeEx)
                    {
                        MessageBox.Show("Close중 오류" + closeEx.Message);
                    }

                    if (ExitFlag != true)
                        MessageBox.Show("통신 종료됨");
                    Invoke(() => end_init());
                }

                _ = Task.Run(() =>
                {
                    Invoke(() => end_init());
                    /*  BeginInvoke(() =>
                      {
                          rpm_check(null);
                          roll_check(null);
                          set_amp_temp(null);
                          sens_check(null);
                          f3.update_Realamp(amp);
                          f3.update_temp(temp);
                          //    getPosition(data);
                          OnNewData(null);
                          update_info(null);
                      });*/
                });


                /*   if (serialPort != null && serialPort.IsOpen)
                   {
                       serialPort.Close();
                       MessageBox.Show("통신 종료됨");

                       BeginInvoke(() =>
                   //    Invoke(new Action(() =>
                       {
                           rpm_check(null);
                           roll_check(null);
                           copyroll(;
                           set_amp_temp(null);
                           sens_check(null);
                           f3.update_Realamp(amp);
                           f3.update_temp(temp);
                           //    getPosition(data);
                           OnNewData(null);
                           update_info(null);
                       });
                   }
                   else
                   {
                       MessageBox.Show("통신이 시작되지 않았습니다.");
                   }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERR : " + ex.Message);
            }
        }

        private async void exit_btn_Click(object sender, EventArgs e)
        {
            //수정필요 

            try
            {
                _cancellationTokenSource?.Cancel();

                if (_modbusPollingTask != null)
                    await _modbusPollingTask;

                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    if (ExitFlag != true)
                        MessageBox.Show("통신 종료됨");
                    Invoke(() => end_init());
                    //   get_message = false;
                }
                else
                {
                    if (ExitFlag != true)
                    MessageBox.Show("통신이 시작되지 않았습니다.");
                    Invoke(() => end_init());
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러: " + ex.Message);
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(password);

            f2.FormSendEvent += new Form2.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
            //form2에 이벤트 추가

            f2.Show();

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
                dog_lbl.Visible = false;
                numericUpDown2.Visible = false;
                //   login = -1;
                if (f3 != null)
                {
                    f3.setting_panel.Visible = false;
                }
            }
        }

        private void DiseaseUpdateEventMethodF2toF1(object sender)
        {
            if ("TRUE".Equals(sender.ToString()))
            {
                //  dog_lbl.Visible = true;
                //  numericUpDown2.Visible = true;
                f3.login_stat = 1;

                if (f3 == null || f3.IsDisposed)    //Ver1
                {
                    f3 = new Form3(amp, temp, std_amp, target_rpm);
                    f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
                }

                f3.pipe_panel.Visible = false;
                //  f3.ampare_panel.Visible = false;
                f3.setting_panel.Visible = true;
                f3.amp_set10.Visible = true;
                f3.err_set.Visible = true;
                f3.err_set_lbl.Visible = true;
                f3.tar_rpm.Visible = true;
                f3.tar_rpm_lbl.Visible = true;

                f3.Size = new Size(570, 950);

                f3.Show();
                f3.BringToFront();
                //    login_btn.Image = BUR_INS_HMI.Properties.Resources.admin_logout_btn; //이미지만 변경되는 것이므로 X.
            }
            else if (!("FALSE".Equals(sender.ToString())))
            {
                password = sender.ToString();
            }

        }


        private void temp_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)  //Ver1
            {
                f3 = new Form3(amp, temp, std_amp, target_rpm);
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
            }

            f3.Show();
            f3.BringToFront();  //이미 열려있다면 앞으로

            /*  if (f3.pipe_panel.Visible == true)
                  f3.ShowPanel(4);
              else if (f3.pipe_panel.Visible == false)
                  f3.ShowPanel(1);*/


        }



        private byte GetLatestDOByte()  //f3이 호출할 함수
        {
            return latestDOByte;
        }


        private void ampare_btn_Click(object sender, EventArgs e)
        {
            if (f3 == null || f3.IsDisposed)    //Ver1
            {
                f3 = new Form3(amp, temp, std_amp, target_rpm);
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
            }

            f3.pipe_panel.Visible = true;
            f3.setting_panel.Visible = false;
            f3.err_set.Visible = false;
            f3.err_set_lbl.Visible = false;
            f3.tar_rpm.Visible = false;
            f3.tar_rpm_lbl.Visible = false;

            f3.Size = new Size(645, 950);

            f3.ShowDialog();
            f3.BringToFront();  //이미 열려있다면 앞으로

            /*    if (f3.ampare_panel.Visible == true)
                    f3.ShowPanel(6);
                else if (f3.ampare_panel.Visible == false)
                    f3.ShowPanel(3);*/
        }



        private void record_btn_Click(object sender, EventArgs e)
        {

        }


        private async void runstop_btn_Click(object sender, EventArgs e)
        {
            await StopCommunicationAsync();
        }

        private async Task StopCommunicationAsync()
        {
            try
            {
                _cancellationTokenSource?.Cancel();

                if (_modbusPollingTask != null)
                    await _modbusPollingTask;

                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.Close();
                    if (ExitFlag != true)
                     MessageBox.Show("통신 종료됨");
                    Invoke(() => end_init());
                    run_btn.Image = BUR_INS_HMI.Properties.Resources.runon_btn_long;
                    runstop_btn.Image = BUR_INS_HMI.Properties.Resources.runstop_btn_long_red2;
                }

                BeginInvoke(() =>
                {
                    rpm_check(null);

                    roll_check(null);
                    set_amp_temp(null);
                    sens_check(null);
                    update_info(null); // err
                    InitializeGraphs();
                    //    OnNewData(null); // ok 

                    //  end_amp_temp();
                    f3.update_Realamp(amp);
                    f3.update_temp(temp);
                    end_amp_temp();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러 : " + ex.Message);
            }
        }

        private void end_amp_temp()
        {
            for (int i = 0; i < 10; i++)
            {
                amp[i] = temp[i] = 0;
                f3.real_amp[i].ForeColor = Color.Blue;
                f3.temp1_arr[i].ForeColor = Color.Blue;
                f3.temp2_arr[i].ForeColor = Color.Blue;
                f3.real_amp[i].Text = "0.0";
                f3.temp1_arr[i].Text = "0.0";
                f3.temp2_arr[i].Text = "0.0";
            }
        }

        private void run_btn_Click(object sender, EventArgs e)
        {
            if (_modbusPollingTask != null && !_modbusPollingTask.IsCompleted)
            {
                MessageBox.Show("이미 실행 중입니다");
                return;
            }

            Init_port();


            _cancellationTokenSource = new CancellationTokenSource();
            _modbusPollingTask = Task.Run(() => ModbusPollingLoop(_cancellationTokenSource.Token));
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
                //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string desktopPath = @"C:\Users\tjlee\Desktop\ksj\Captured";

                if (!Directory.Exists(desktopPath)) //파일 미존재시 파일 생성
                {
                    Directory.CreateDirectory(desktopPath);
                }

                //저장 파일명 지정
                string fileName = $"FulllScreen_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(desktopPath, fileName);


                //이미지 저장
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show("Screen Captured");
            }
        }







        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _modbusPollingTask?.Wait(); //안전하게 스레드 종료

                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch { }
        }

        private async void pic_stop_btn_Click_1(object sender, EventArgs e)   //추후 데이터 받아서 estop시 반영 필요
        {
            if (serialPort != null && serialPort.IsOpen)    //포트 연결 되어 있고 통신도중일 때.
            {
                if (!estop)
                {
                    if (MessageBox.Show("비상 정지 하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.estop_on;
                        _modbusMaster.WriteSingleRegister(slaveId, 10, 1);
                        MessageBox.Show("장비를 비상 정지합니다.");

                        await StopCommunicationAsync();
                        estop = true;
                    }
                }
                else // estop == true 인 경우
                {
                    if (MessageBox.Show("비상 정지를 해제하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.local_e_stop_off;
                        _modbusMaster.WriteSingleRegister(slaveId, 10, 0);
                        estop = false;
                    }
                }

            }
            else if (estop) //estop == true (on) 인 경우
            {
                if (MessageBox.Show("비상 정지를 해제하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.local_e_stop_off;
                    _modbusMaster.WriteSingleRegister(slaveId, 10, 0);
                    estop = false;
                }
            }
        }

        private void ClearAllData()
        {
            foreach (var group in ys)
                foreach (var line in group)
                    line.Clear();

            foreach (var group in xs)
                foreach (var line in group)
                    line.Clear();

            foreach (var plot in plots)
            {
                plot.Plot.Clear();
                plot.Plot.SetAxisLimits(xMin: 0, xMax: xRangeSeconds, yMin: -1, yMax: 30);
                plot.Render();
            }
        }

        private void StartMeasurement()
        {
            stopwatch.Restart();
            isMeasuring = true;

            foreach (var group in ys)
                foreach (var line in group)
                    line.Clear();

            foreach (var group in xs)
                foreach (var line in group)
                    line.Clear();
        }

        private void ChangeXRangeFromTextBox()
        {
            if (double.TryParse(textBox1.Text, out double minutes) && minutes > 0)
            {
                xRangeSeconds = minutes * 60;
                UpdateAllPlots();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (int.TryParse(textBox1.Text, out int minutes) && minutes > 0)    //int.TryParse()로 실수 자동 거부
                {
                    xRangeSeconds = minutes * 60;
                    // 기존 데이터 유지, 시간도 유지
                    UpdateAllPlots(); // X축 갱신만
                }
                else
                {
                    MessageBox.Show("1 이상의 정수값를 입력하세요 (분 단위)");
                }
            }
            /* if (e.KeyCode == Keys.Enter)
             {
                 e.SuppressKeyPress = true;
                 if (int.TryParse(textBox1.Text, out int minutes) && minutes > 0)
                 {
                     xRangeSeconds = minutes * 60;

                     UpdateAllPlots();  // 새 x축 범위로 다시 그리기만
                 }
                 else
                 {
                     MessageBox.Show("1 이상의 정수를 입력하세요 (단위: 분)", "입력 오류");
                 }
             }*/

        }

        private void end_init()
        {

            run_btn.Image = BUR_INS_HMI.Properties.Resources.run_btn_long;
            runstop_btn.Image = BUR_INS_HMI.Properties.Resources.runstop_btn_long_red2;
            //init 추가
            InitializeGraphs();
            //필요 : /sensor/, /position/, roll, leveling 초기화

            pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
            pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
            pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;

            
            for(int i = 0; i < 10;i++)
            {
                sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
            }

            for (int n = 0; n < 27; n ++)
            {
                roll[n].BackColor = Color.ForestGreen;
            }
            ExitFlag = false;
        }


        private void record_btn_Click_1(object sender, EventArgs e)
        {
            Form6 f6 = new Form6();
            f6.Show();
        }

        private void warning_btn_Click(object sender, EventArgs e)
        {
            warning_pan.Visible = false;
            panel3.Visible = true;
            panel3.Dock = DockStyle.Right;
           // BringToFront();

            get_message = false;

            IsWarningOpen = false;
        }

        

    }
}
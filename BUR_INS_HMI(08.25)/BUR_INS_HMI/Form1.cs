using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using ScottPlot;
using ScottPlot.Plottable;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Modbus.Extensions.Enron;
using System.Runtime.Intrinsics.X86;
using System.Reflection;
using Azure.Core.GeoJson;
using ScottPlot.Plottable;
using ScottPlot.MarkerShapes;
using Microsoft.IdentityModel.Tokens;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace BUR_INS_HMI
{
    public partial class Form1 : Form
    {

        //사무실
        //MssqlLib.cs에서 db 경로는 바꿔줘야함
        private string settingfilePath = @"C:\Users\tjlee\Desktop\ksj\settings.txt";    //설정값 메모장 경로
        public string capturePath = @"C:\Users\tjlee\Desktop\ksj\captured"; //스크린샷 경로
        string csvPath = @"C:\Users\tjlee\Desktop\ksj\storage"; //csv 파일 경로

        /*
        //RIST
        //MssqlLib.cs에서 db 경로는 바꿔줘야함
        
        private string settingfilePath = capturePath = @"C:\Users\User\Desktop\settings.txt";   //설정값 메모장 경로
        public string capturePath = @"C:\Users\User\Desktop\storage";   //스크린샷 경로
        string csvPath = @"C:\Users\User\Desktop\ScreenCaptured";   //csv 파일 경로
        */

        private Label[] test_data;

        private Form3 f3;
        FormsPlot avg_plot;
        private Label[] roll_rpm;
        private PictureBox[] sensor;
        private Label[] info_lbl;
        private byte latestDOByte = 0x00;   //가장 최근의 DO raw 값 저장

        private bool isWarning = false;

        private string password = "0000";

        private SerialPort serialPort;

        public ModbusSerialMaster _modbusMaster;
        public Func<byte> GetDOByte;

        private Stopwatch stopwatch = Stopwatch.StartNew();

        private ushort[] startAddress = {0x0000, 0x0000, 0x0000, 0x0000 };   // 각 시작 주소     30001
        private ushort[] numInputs = { 9 , 40, 40, 16 };    //각 데이터 개수
        private byte[] deviceAddress = { 4, 1, 2, 3 };  //Slave Number

        private ushort[] tempData = { 0,0,0,0,0, 0,0,0,0,0,
                                        0,0,0,0,0, 0,0,0,0,0,
                                        0,0,0,0,0, 0,0,0,0,0,
                                        0,0,0,0,0, 0,0,0,0,0};

     /*   private ushort[] tempData = { 20,20,20,20,20, 20,20,20,20,20,
                                        20,20,20,20,20, 20,20,20,20,20,
                                        20,20,20,20,20, 20,20,20,20,20,
                                        20,20,20,20,20, 20,20,20,20,20};*/

        private bool isBlink = true;

        internal decimal[] target_rpm = new decimal[] { 20, 1000 };  //[ 구동 판정 속도 | 구동 속도 주기 ]
        public decimal[] amp = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,    5 }; //전류값//index[10]:err_range(오차 범위)
        public decimal[] temp1 = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //온도값1
        public decimal[] temp2 = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //온도값2



        public decimal[] min = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public decimal[] max = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public decimal[] difference = new decimal[] { 0.5M };   //UI 반영 기준 수치값 차이

        public decimal[] std_amp = new decimal[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };  //기준 전류

    //    public ushort[] MeasureData = new ushort[80];

        bool estop = false;

        List<FormsPlot> plots = new();             // Plot (10개의 그래프 생성할 곳)

        List<List<List<double>>> ys = new();  // [plot][line][values]
        List<List<List<double>>> xs = new();  // [plot][line][indices]

        bool isMeasuring = false;
        private int[] layout = { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };    //각 라인별 채널 개수

        public bool isRunning = false;

        private double xRangeSeconds = 300; //꺾은선 그래프 기본 측정 시간값 5분(300초)

        private CancellationTokenSource cancellationTokenSourceForModbus;
        private Task _modbusPollingTask;

        private bool isReading = false;

        private System.Windows.Forms.Timer blinkTimer;
        private bool isFlashing = false;

        private bool ExitFlag = false;

        MssqlLib mssql = new MssqlLib();    //sql용 cs파일 생성

        private bool shouldUpdateRpm = false;
        private ushort[] prevData = new ushort[80];

        private ScottPlot.Plottable.BarPlot avgBar; // 평균 막대 1개로 10라인 표현
        private double[] groupAverages = new double[10];    //각 라인별 평균값
        private string[] groupLabels = Enumerable.Range(1, 10).Select(n => $"L{n}").ToArray();  //라인 넘버
        private ScottPlot.Plottable.HLine levelLine;    //지시 레벨링 속도값 표시선


        public Form1()
        {
            InitializeComponent();
            //   ShowBarChartWithAverages();
           
            panel1.BringToFront();
            f3 = new Form3(amp, std_amp, target_rpm, this);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //전체화면 작업표시줄은 존재
            /*   this.FormBorderStyle = FormBorderStyle.None;    //테두리x
               this.WindowState = FormWindowState.Maximized;   //화면 가득 채우기
            */

            //전체화면 작업표시줄 X
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.StartPosition = FormStartPosition.Manual;

            avg_plot = new FormsPlot();
            avg_plot.Dock = DockStyle.Fill;

            this.panel2.Controls.Add(avg_plot);
            avg_plot.BringToFront();

            InitializeGraphs(); //꺾은선, 막대 그래프 초기화



            sensor = new PictureBox[] { pic_sen1, pic_sen2, pic_sen3, pic_sen4, pic_sen5, pic_sen6,
             pic_sen7, pic_sen8, pic_sen9, pic_sen10 };

            info_lbl = new Label[] { info_lbl1,info_lbl2,info_lbl3, info_lbl4,info_lbl5,info_lbl6,
            info_lbl7, info_lbl8, info_lbl9};

            roll_rpm = new Label[]
            {
                roll_rpm1,roll_rpm2,roll_rpm3,roll_rpm4,roll_rpm5,roll_rpm6,roll_rpm7,roll_rpm8,
                roll_rpm9,roll_rpm10,roll_rpm11,roll_rpm12,roll_rpm13,roll_rpm14,roll_rpm15,roll_rpm16,
                roll_rpm17,roll_rpm18,roll_rpm19,roll_rpm20,roll_rpm21,roll_rpm22,roll_rpm23,roll_rpm24,
                roll_rpm25,roll_rpm26,roll_rpm27
            };

            //데이터 확인용
            test_data = new Label[] { data1,label4, label5, label6, label7,label8,label9,label10,
            label11,label12,label13,label14,label15,label16,label17,label18,label19,label20,
            label21,label22,label23,label24,label25,label26,label27,label28,label29,label30,
            label31,label32,label33,label34,label35,label36,label37,label38,label39,label40,
            label41,label42, label43, label44, label45, label46, label47, label48, label49, label50,
            label51, label52, label53, label54, label55, label56, label57, label58, label59, label60,
            label61, label62, label63, label64, label65, label66, label67, label68, label69, label70,
            label71,label72, label73, label74, label75, label76, label77, label78, label79, label80,
            label81, label82,
                label83, label84, label85, label86, label87, label88, label89,
            label90, label91, label92, label93, label94, label95, label96, label97, label98};
            //data 43~82   


            if (!File.Exists(settingfilePath))  //파일 실행시 메모장에서 설정값 읽어오기
            {
              //  MessageBox.Show("설정 파일이 존재하지 않습니다: " + settingfilePath);
                MessageBox.Show("읽어올 설정 파일이 존재하지 않습니다. 임의 초기값으로 설정됩니다.");
                return;
            }
            LoadSettings(settingfilePath);  //파일읽어 설정값 불러오기
        }

        private async Task ModbusPollingLoop(CancellationToken token)   //Modbus 통신 되고 있는지 확인 함수
        {

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (serialPort == null || !serialPort.IsOpen)
                    {
                        ExitFlag = true;
                        //   throw new IOException("포트가 연결되어 있지 않습니다."); //밑에 MessageBox랑 한 셋트
                        throw new IOException("port not connected");
                    }

                    await ReadModbusData(token);

                }
                catch (Exception ex)
                {
                    Invoke((System.Windows.Forms.MethodInvoker)(() =>
                    {
                        ExitFlag = true;
                        MessageBox.Show("Modbus error : " + ex.Message); //message : Mod 오류 : 포트 연결 x 로 세트
                        end_init();
                    }));
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

        //   string csvFilePath = @"C:\Users\tjlee\Desktop\ksj\storage\modbus_log.csv";
//        string folderPath = @"C:\Users\User\Desktop\storage";
        private void SaveDataToCSV(ushort[] data)
        {

            //       string folderPath = @"C:\Users\tjlee\Desktop\ksj\storage";
        //    string folderPath = @"C:\Users\User\Desktop\storage";
            string dateString = DateTime.Now.ToString("yyyy_MM_dd");    //날짜별 로그 준비
            string fileName = $"DataLog_{dateString}.csv";
            string csvFilePath = Path.Combine(csvPath, fileName);

            if (!Directory.Exists(csvPath))
                Directory.CreateDirectory(csvPath);

            if (!File.Exists(csvFilePath))
            {
                using (StreamWriter sw = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                {
                    sw.WriteLine("Timestamp,RPM 1-1,RPM 1-2,RPM 1-3,RPM 1-4,TEMP 1-1,TEMP 1-2,AMP 1,Period1(ms)," +
                        "RPM 2-1,RPM 2-2,RPM 2-3,RPM 2-4,TEMP 2-1,TEMP 2-2,AMP 2,Period2(ms)," +
                        "RPM 3-1,RPM 3-2,RPM 3-3,RPM 3-4,TEMP 3-1,TEMP 3-2,AMP 3,Period3(ms)," +
                        "RPM 4-1,RPM 4-2,RPM 4-3,RPM 4-4,TEMP 4-1,TEMP 4-2,AMP 4,Period4(ms)," +
                        "RPM 5-1,RPM 5-2,RPM 5-3,RPM 5-4,TEMP 5-1,TEMP 5-2,AMP 5,Period5(ms)," +
                        "RPM 6-1,RPM 6-2,RPM 6-3,RPM 6-4,TEMP 6-1,TEMP 6-2,AMP 6,Period6(ms)," +
                        "RPM 7-1,RPM 7-2,RPM 7-3,RPM 7-4,TEMP 7-1,TEMP 7-2,AMP 7,Period7(ms)," +
                        "RPM 8-1,RPM 8-2,RPM 8-3,RPM 8-4,TEMP 8-1,TEMP 8-2,AMP 8,Period8(ms)," +
                        "RPM 9-1,RPM 9-2,RPM 9-3,RPM 9-4,TEMP 9-1,TEMP 9-2,AMP 9,Period9(ms)," +
                        "RPM 10-1,RPM 10-2,RPM 10-3,RPM 10-4,TEMP 10-1,TEMP 10-2,AMP 10,Period10(ms)"); // 헤더
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
        
        private void SaveDataToSQL(ushort[] data)
        {
            try
            {
                if (data == null || data.Length < 80)   //판 정보 포함시 89, 미포함시 80
                    return; // 데이터 부족 시 저장 안 함


                int[] keepIndexes = new int[10];
                for (int i = 0; i < 10; i++)
                {
                         keepIndexes[i] = ((i + 1) * 8) - 1;   //7, 15, 23, 31, 39, 47, 55, 63, 71, 79  판정보 미포함시 RPM 주기 인덱스
                //    keepIndexes[i] = ((i + 1) * 8) + 8;    //16, 24, 32, 40, 48, 56, 64, 72, 80, 88 판정보 포함시 기존 인덱스 +9
                }

                // ushort[] → string[] 변환

                string[] strData = data.Take(80).Select((x, idx) => keepIndexes.Contains(idx) ?   //판정보 미포함시 80
                x.ToString() : (x * 0.1m).ToString("F1")).ToArray();        //RPM 주기 제외 모두 단위 처리 ( x 0.1 )

           

                  // DB 저장
                  mssql.InsertDB(strData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB 저장 실패: " + ex.Message);
            }
        }


  



        private void InitializeGraphs()
        {
            //꺾은선   
            ys.Clear();
            xs.Clear();
            plots.Clear();
            fp_panel.Controls.Clear();
            int margin = 0;

            int plotWidth = 650;

            int plotHeight = 0;


            //막대
            avg_plot.Plot.Clear();
            // 초기 라인별 평균값 0
            groupAverages = new double[10];
            avgBar = avg_plot.Plot.AddBar(groupAverages);
            avgBar.FillColor = Color.CornflowerBlue;

            avg_plot.Plot.XTicks(Enumerable.Range(0, groupLabels.Length).Select(i => (double)i).ToArray(), groupLabels);
            avg_plot.Plot.SetAxisLimits(xMin: -0.5, xMax: groupLabels.Length - 0.5, yMin: 0, yMax: 300);
            //x범위 및 y범위 300 (구동모사장치 최대RPM 300이므로) 설정

            // 레벨선 초기화
            double level = Convert.ToDouble(info_lbl9.Text);
            levelLine = avg_plot.Plot.AddHorizontalLine(level);
            levelLine.Color = Color.Red;
            levelLine.LineStyle = ScottPlot.LineStyle.Dash;
            levelLine.Label = $"Level {level}";

            var level_legend = avg_plot.Plot.Legend();
            level_legend.Location = ScottPlot.Alignment.UpperRight;
            avg_plot.Plot.Title("Line 평균 선속도");
            avg_plot.Render();


            for (int i = 0; i < 10; i++)
            {
                int groupIndex = i;
                ys.Add(new List<List<double>>());
                xs.Add(new List<List<double>>());

                var fp = new FormsPlot();
                {
                    if (i == 0 || i == 5)
                    {
                        plotHeight = 180;
                    }
                    else
                        plotHeight = 160;

                    fp.Size = new Size(plotWidth, plotHeight);
                    fp.Margin = new Padding(margin);


                    if (i == 0 || i == 5)
                        fp.Location = new Point(45 + 685 * (i / 5), -11);
                    else
                        fp.Location = new Point(45 + 685 * (i / 5), 151 * (i % 5));

                    fp.Configuration.ScrollWheelZoom = true;
                    fp.Configuration.MiddleClickAutoAxis = true;
                    fp.Configuration.RightClickDragZoom = true;
                    fp.Configuration.LeftClickDragPan = true;
                    //  fp.Plot.Frame(false);
                }

                var lbl = new Label();    //Line Number 
                lbl.Text = (i + 1).ToString();
                lbl.Font = new Font("맑은 고딕", 18, FontStyle.Bold);
                lbl.ForeColor = Color.Black;
                lbl.BackColor = Color.Transparent;
                lbl.AutoSize = true;

                if (i == 9)
                    lbl.Location = new Point(fp.Left - 20, fp.Top + 10);
                else
                    lbl.Location = new Point(fp.Left - 10, fp.Top + 10);

                var pic_legend = new PictureBox();
                pic_legend.SizeMode = PictureBoxSizeMode.StretchImage;
                //   legend.Location = new Point(fp.Left - 35, fp.Top + 20);
                pic_legend.Size = new Size(60, 55);

                if (i == 0 || i == 5)
                {
                    pic_legend.Location = new Point(fp.Left - 35, fp.Top + 20 + 30);
                    //  lbl.Location = new Point(27 + 690 * (i / 5), 65 + 150 * (i % 5));
                }
                else if (i == 9)
                {
                    pic_legend.Location = new Point(fp.Left - 35, fp.Top + 20 + 30);
                    //  lbl.Location = new Point(22 + 690 * (i / 5), 77 + 150 * (i % 5));
                }
                else
                    pic_legend.Location = new Point(fp.Left - 35, fp.Top + 20 + 30);
                //   lbl.Location = new Point(27 + 690 * (i / 5), 77 + 150 * (i % 5));


                fp.Plot.SetAxisLimits(xMin: 0, xMax: xRangeSeconds, yMin: -1, yMax: 30);
                fp.Plot.XAxis.TickLabelStyle(fontSize: 10);
                fp.Plot.YAxis.TickLabelStyle(fontSize: 10);

                switch (i)
                {
                    case 0:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_1;
                            break;
                        }
                    case 1:
                        {

                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_2;
                            pic_legend.Size = new Size(60, 40);
                            break;
                        }
                    case 2:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_3;
                            break;
                        }
                    case 3:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_4;
                            break;
                        }
                    case 4:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_5;
                            pic_legend.Size = new Size(60, 40);
                            break;
                        }
                    case 5:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_6;
                            break;
                        }
                    case 6:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_7;
                            break;
                        }
                    case 7:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_8;
                            pic_legend.Size = new Size(60, 40);
                            break;
                        }
                    case 8:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_9;
                            break;
                        }
                    case 9:
                        {
                            pic_legend.Image = BUR_INS_HMI.Properties.Resources.legend_10;
                            break;
                        }

                }



                fp_panel.Controls.Add(lbl);
                fp_panel.Controls.Add(pic_legend);

                for (int j = 0; j < layout[groupIndex]; j++)
                {
                    ys[i].Add(new List<double>());
                    xs[i].Add(new List<double>());

                }
                fp_panel.Controls.Add(fp);

                plots.Add(fp);

                //  this.flowLayoutPanel1.Controls.Add(fp);

            }
        }

        private void OnNewData(ushort[] data, int index)    //그래프용 데이터 저장
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnNewData(data, index)));
                return;
            }

            if (data == null || data.Length < 40)
                return;

            if (!isMeasuring)
            {
                isMeasuring = true;
                stopwatch.Restart();
            }

            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

            if (elapsedSeconds > xRangeSeconds) //기준 시간 넘어가면 그래프 다시 처음부터 생성
            {
                stopwatch.Restart();
                ClearAllData();
                return; // 다음 데이터부터 다시 시작
            }

            //SPARE 센서 사용 대비 하드 코딩으로 데이터 저장

            if (index == 0)
            {
                xs[0][0].Add(elapsedSeconds);
                ys[0][0].Add((double)(data[2] / 10.0));   //1-1
                xs[0][1].Add(elapsedSeconds);
                ys[0][1].Add((double)(data[1] / 10.0));   //1-2
                xs[0][2].Add(elapsedSeconds);
                ys[0][2].Add((double)(data[0] / 10.0));   //1-3

                xs[1][0].Add(elapsedSeconds);
                ys[1][0].Add((double)(data[9] / 10.0));   //2-1
                xs[1][1].Add(elapsedSeconds);
                ys[1][1].Add((double)(data[8] / 10.0));   //2-2

                xs[2][0].Add(elapsedSeconds);
                ys[2][0].Add((double)(data[18] / 10.0));   //3-1
                xs[2][1].Add(elapsedSeconds);
                ys[2][1].Add((double)(data[17] / 10.0));   //3-2
                xs[2][2].Add(elapsedSeconds);
                ys[2][2].Add((double)(data[16] / 10.0));   //3-3

                xs[3][0].Add(elapsedSeconds);
                ys[3][0].Add((double)(data[26] / 10.0));   //4-1
                xs[3][1].Add(elapsedSeconds);
                ys[3][1].Add((double)(data[25] / 10.0));   //4-2
                xs[3][2].Add(elapsedSeconds);
                ys[3][2].Add((double)(data[24] / 10.0));   //4-3

                xs[4][0].Add(elapsedSeconds);
                ys[4][0].Add((double)(data[33] / 10.0));   //5-1
                xs[4][1].Add(elapsedSeconds);
                ys[4][1].Add((double)(data[32] / 10.0));   //5-2
            }
            else if (index == 40)
            {


                xs[5][0].Add(elapsedSeconds);
                ys[5][0].Add((double)(data[2] / 10.0));   //6-1
                xs[5][1].Add(elapsedSeconds);
                ys[5][1].Add((double)(data[1] / 10.0));   //6-2
                xs[5][2].Add(elapsedSeconds);
                ys[5][2].Add((double)(data[0] / 10.0));   //6-3

                xs[6][0].Add(elapsedSeconds);
                ys[6][0].Add((double)(data[10] / 10.0));   //7-1
                xs[6][1].Add(elapsedSeconds);
                ys[6][1].Add((double)(data[9] / 10.0));   //7-2
                xs[6][2].Add(elapsedSeconds);
                ys[6][2].Add((double)(data[8] / 10.0));   //7-3

                xs[7][0].Add(elapsedSeconds);
                ys[7][0].Add((double)(data[17] / 10.0));   //8-1
                xs[7][1].Add(elapsedSeconds);
                ys[7][1].Add((double)(data[16] / 10.0));   //8-2

                xs[8][0].Add(elapsedSeconds);
                ys[8][0].Add((double)(data[26] / 10.0));   //9-1
                xs[8][1].Add(elapsedSeconds);
                ys[8][1].Add((double)(data[25] / 10.0));   //9-2
                xs[8][2].Add(elapsedSeconds);
                ys[8][2].Add((double)(data[24] / 10.0));   //9-3

                xs[9][0].Add(elapsedSeconds);
                ys[9][0].Add((double)(data[34] / 10.0));   //10-1
                xs[9][1].Add(elapsedSeconds);
                ys[9][1].Add((double)(data[33] / 10.0));   //10-2
                xs[9][2].Add(elapsedSeconds);
                ys[9][2].Add((double)(data[32] / 10.0));   //10-3
            }
            
            UpdateAllPlots();   //선 그래프
        }


        private void UpdateBarChart(ushort[] data)   //평균값 그래프
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateBarChart(data)));
                return;
            }

            if (avg_plot == null) return;

            double[] groupAverages = new double[layout.Length];  // 전체 10라인 

            //SPARE 센서 사용 대비 하드 코딩

            double[] sum = new double[layout.Length];

                sum[0] = (data[2] + data[1] + data[0]) / 10;
                sum[1] = (data[9] + data[8]) / 10;
                sum[2] = (data[18] + data[17] + data[16]) / 10;
                sum[3] = (data[26] + data[25] + data[24]) / 10;
                sum[4] = (data[33] + data[32]) / 10;
                groupAverages[0] = sum[0] / 3;
                groupAverages[1] = sum[1] / 2;
                groupAverages[2] = sum[2] / 3;
                groupAverages[3] = sum[3] / 3;
                groupAverages[4] = sum[4] / 2;

            sum[5] = (data[42] + data[41] + data[40]) / 10;
            sum[6] = (data[50] + data[49] + data[48]) / 10;
            sum[7] = (data[57] + data[56]) / 10;
            sum[8] = (data[66] + data[65] + data[64]) / 10;
            sum[9] = (data[74] + data[73] + data[72]) / 10;
                 
            groupAverages[5] = sum[5] / 3;
            groupAverages[6] = sum[6] / 3;
            groupAverages[7] = sum[7] / 2;
            groupAverages[8] = sum[8] / 3;
            groupAverages[9] = sum[9] / 3;
           
            avgBar.Values = groupAverages;



              double level = Convert.ToDouble(info_lbl9.Text);
            avg_plot.Plot.Remove(levelLine);  // 이전 라인 제거
            levelLine = avg_plot.Plot.AddHorizontalLine(level);
            levelLine.Color = Color.Red;
            levelLine.LineStyle = ScottPlot.LineStyle.Dash;
            levelLine.Label = $"Level {level.ToString("F1")}";

            avg_plot.Render();

        }




        private void UpdateAllPlots()   //꺾은선 차트
        {

            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds; // 축 고정
            double xMin = Math.Max(0, elapsedSeconds - xRangeSeconds);
            double xMax = elapsedSeconds;

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
                    plot.Plot.AddScatter(x, y, color: color, markerSize: 0);
                }

                int tickStep = 60; // 60초 = 1분
                var ticks = Enumerable.Range(0, (int)xRangeSeconds / tickStep + 1)
                                      .Select(t => (double)(t * tickStep))
                                      .ToArray();
                var labels = ticks.Select(t => $"{t / 60}분").ToArray(); // m(분) 단위 표시

                if (i == 0 || i == 5)   //1Line, 6Line만 밑에 분 표기
                    plot.Plot.XTicks(ticks, labels);
                else
                    plot.Plot.XAxis.Ticks(false);   //X축 눈금 제거

                plot.Plot.SetAxisLimitsX(0, xRangeSeconds); // X축 고정
                plot.Plot.AxisAutoY();

                plot.Plot.Legend(location: Alignment.UpperRight);
                plot.Render();
            }

        }

        private Color GetColor(int index)
        {
            Color[] colors = { Color.Crimson, Color.FromArgb(128, 64, 64), Color.Blue/*, Color.Orange, Color.Purple*/ };

            /*   Color[] colors =
       {
           Color.Blue,        // 파랑
           Color.Red,         // 빨강
           Color.Green,       // 초록
           Color.Orange,      // 주황
           Color.Purple,      // 보라
           Color.Cyan,        // 청록
           Color.Magenta,     // 자홍
           Color.Gold,        // 금색
           Color.DeepSkyBlue, // 하늘 파랑
           Color.LimeGreen,   // 연두
           Color.Brown,       // 갈색
           Color.HotPink,     // 핑크
           Color.DarkSlateGray, // 어두운 청록 회색
           Color.Crimson,     // 진홍
           Color.Teal         // 청록
       };*/
            return colors[index % colors.Length];
        }


        private bool Init_port()    //시리얼 통신 포트 연결 함수
        {
            try
            {
                if (serialPort == null || !serialPort.IsOpen)
                {
                    serialPort = new SerialPort("COM3") //사무실, RIST 모두 COM3으로 동일
                    {
                        BaudRate = 9600,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,
                        ReadTimeout = 100,
                        WriteTimeout = 100
                    };

                    serialPort.Open();
                    _modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);

                    isReading = true;
                    MessageBox.Show("PORT OPEN");
                    run_btn.Image = BUR_INS_HMI.Properties.Resources.runon_0809;
                    runstop_btn.Image = BUR_INS_HMI.Properties.Resources.stopoff_0809;

                    cancellationTokenSourceForModbus = new CancellationTokenSource();

                    return true; //485 포트 연결 성공
                }

                if (!serialPort.IsOpen)
                {
                    MessageBox.Show("포트가 닫혀 있습니다. 다시 연결하세요.");
                    return false;
                }

                //이미 열려있음
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("포트 오픈 실패: " + ex.ToString());
                return false;
            }
        }


        private void serialPort_DataReceived(object sender, EventArgs e)
        {


        }

        private void test_update(ushort[] data, int index)  //데이터 확인용 함수
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => test_update(data, index)));
                return;
            }

            for (int i = 0; i < data.Length; i++)
            {
                test_data[i + index].Text = data[i].ToString();
            }

        }

        private bool _hasShownDisconnectMessage = false;

        private void update_amp_temp(ushort[] data, int index)  //전류값 확인 및 모듈 상태 표기 함수
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => update_amp_temp(data, index)));
                return;
            }

            if (data == null)
            {
                for (int i = 0; i < 10; i++)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
                }
                return;
            }

            if (!serialPort.IsOpen) //추후 위와 합쳐도 O
            {
                for (int i = 0; i < 10; i++)
                {
                    sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
                }
            }
            else
            {

                int offset = (index == 0) ? 0 : 5;

                for (int i = 0; i < 5; i++)
                {
                    decimal ampare = std_amp[i + offset] * layout[i + offset];  //각 라인별 기준 전류 계산
                    min[i + offset] = ampare * ((100.0M - amp[10]) / 100.0M);   //오차범위(amp[10])고려 최소값 계산
                    max[i + offset] = ampare * ((100.0M + amp[10]) / 100.0M);   //오차범위(amp[10])고려 최대값 계산
                    amp[i + offset] = data[(i + 1) * 8 - 2] * 0.1M; //데이터로 전달받은 현재 전류값 저장

                    if (amp[i + offset] >= min[i + offset] && amp[i + offset] <= max[i + offset])   //전류 정상 범위 내 (GREEN)
                        sensor[i + offset].Image = BUR_INS_HMI.Properties.Resources.sen_stat_green_control2;
                    else    //전류 정상 범위 외 (RED)
                        sensor[i + offset].Image = BUR_INS_HMI.Properties.Resources.sen_stat_realred_control;
                }
            }
        }

        private async Task ReadModbusData(CancellationToken token)  //데이터 읽어오기 
        {
            ushort[] slave1Data = null;
            ushort[] slave2Data = null;

            //    ushort[] data = null;

            ushort[] data = new ushort[40]; // 크기 40짜리 배열 만들기   임시용

            while (!token.IsCancellationRequested)
            {
                for (int i = 1; i < 4; i++) //Slave 4, 1, 2, 3 

                {
                      if (token.IsCancellationRequested) //종료 신호를 받으면 루프 탈출
                          return;

                    try
                    {

                        //       data = _modbusMaster.ReadHoldingRegisters(deviceAddress[i], startAddress[i], numInputs[i]);


                        /*  if (data == null)
                          {
                              Console.WriteLine($"Slave {i} data null");
                              continue;
                          }*/

                        Random rnd = new Random();  //데이터 랜덤 생성

                        for (int n = 0; n < data.Length; n++)
                        {
                              data[n] = (ushort)(rnd.Next(0, 301) * 10);  // 0 이상 3000 이하 10단위 난수
                        }

                        int index = (i == 1) ? 0 : 40;  //SLAVE 1(0) or 2(40)
                        if (i == 3) //SLAVE 3
                            index = 80;
                        //아래 UI 관련 작업은 UI 스레드에서 안전하게 실행




                        switch (i)
                        {
                            case 1:

                                slave1Data = data;
                                Invoke(new Action(() =>
                                {

                                    test_update(slave1Data, index); //데이터 확인용 함수
                                    rpm_check(slave1Data, index);   //각 ROLL RPM 확인 함수
                                    roll_check(slave1Data, index);  //ROLL 속도 판정 함수
                                    getPosition();  //교정중 판정 함수

                                    update_amp_temp(slave1Data, index); //전류값 확인 및 모듈 상태 표기 함수

                                    target_rpm[1] = Convert.ToDecimal(slave1Data[7]);   //RPM 주기 저장
                                    f3.tar_period.Text = target_rpm[1].ToString();  //RPM 주기 UI 반영
                                    f3.update_Realamp(slave1Data, index);   //전류값 업데이트(form3)
                                    f3.update_temp(slave1Data, index);  //온도값 업데이트(form3)
                                    OnNewData(slave1Data, index);   //그래프용 데이터 저장


                                }));
                                break;

                            case 2:
                                {
                                    slave2Data = data;
                                    Invoke(new Action(() =>
                                    {   
                                        test_update(slave2Data, index); //데이터 확인용 함수
                                        rpm_check(slave2Data, index);    //각 ROLL RPM 확인 함수
                                        roll_check(slave2Data, index);  //ROLL 속도 판정 함수
                                        getPosition();  //교정중 판정 함수

                                        update_amp_temp(slave2Data, index);//전류값 확인 및 모듈 상태 표기 함수

                                        target_rpm[1] = Convert.ToDecimal(slave2Data[7]);    //RPM 주기 저장
                                        f3.tar_period.Text = target_rpm[1].ToString();  //RPM 주기 UI 반영
                                        f3.update_Realamp(slave2Data, index);   //전류값 업데이트(form3)
                                        f3.update_temp(slave2Data, index); //온도값 업데이트(form3)
                                        OnNewData(slave2Data, index);//그래프용 데이터 저장
                                    }));
                                    break;

                                }

                            case 3:
                                {
                                    Invoke(new Action(() =>
                                    {
                                        test_update(data, index);
                                        local_estop(data);  //LC Panel에서 estop 요청시

                                        //   Console.WriteLine($"Slave3: {string.Join(", ", data)}");
                                        //     warn_check(data);    //ROLL 불량시 경고창 (YELLOW or RED)
                                    }));
                                    break;
                                }
                        }

                        // 정상 통신이 이루어졌으므로 오류 플래그 해제
                        _hasShownDisconnectMessage = false;
                    }
                    catch (TimeoutException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Slave {i} Exception: " + ex.Message);
                        //   MessageBox.Show($"slave {i} ERROR");
                    }
                }


                // 🟡 슬레이브1과 슬레이브2 데이터 모두 수신되었을 경우
                if (slave1Data != null && slave2Data != null)
                {
                    //  Slave 1과 Slave 2의 데이터 병합 [40] + [40] = [80]
                        ushort[] combined = new ushort[slave1Data.Length + slave2Data.Length];
                       Array.Copy(slave1Data, 0, combined, 0, slave1Data.Length);
                       Array.Copy(slave2Data, 0, combined, slave1Data.Length, slave2Data.Length);

                    //Slave 1 + 2 데이터[80]으로 평균값 막대그래프 생성
                    BeginInvoke(new Action(() => UpdateBarChart(combined)));

                   //    SaveDataToCSV(combined);
                       SaveDataToSQL(combined);  // 필요시 같이 저장


                    prevData = combined;
                    Console.WriteLine($"Slave 1  2 : Len {combined.Length}");
                    Console.WriteLine(string.Join(", ", prevData));
                }
                else if (slave1Data != null && slave2Data == null)    //Slave2가 통신x경우
                {
                    //  Slave 1과 Slave 2의 데이터 병합 [40] + [40] = [80]
                    //Slave 2는 통신X인 경우이므로 0으로 채워진 tempData[40]과 병합
                    ushort[] combined = new ushort[slave1Data.Length + 40];
                       Array.Copy(slave1Data, 0, combined, 0, slave1Data.Length);
                       Array.Copy(tempData, 0, combined, slave1Data.Length, 40);


                    //Slave 1 + 2 데이터[80]으로 평균값 막대그래프 생성
                    BeginInvoke(new Action(() => UpdateBarChart(combined)));

                    //    SaveDataToCSV(combined);
                    SaveDataToSQL(combined);  // 필요시 같이 저장


                    prevData = combined;
                    Console.WriteLine($"Slave 1 : Len {combined.Length}");
                    Console.WriteLine(string.Join(", ", prevData));
                }
                else if (slave1Data == null && slave2Data != null)    //Slave1이 통신x경우
                {
                    //  Slave 1과 Slave 2의 데이터 병합 [40] + [40] = [80]
                    //Slave 1는 통신X인 경우이므로 0으로 채워진 tempData[40]과 병합
                    ushort[] combined = new ushort[40 + slave2Data.Length];
                    Array.Copy(tempData, 0, combined, 0, 40);
                    Array.Copy(slave2Data, 0, combined, 40, slave2Data.Length);

                    //Slave 1 + 2 데이터[80]으로 평균값 막대그래프 생성
                    BeginInvoke(new Action(() => UpdateBarChart(combined)));

                    //    SaveDataToCSV(combined);
                    SaveDataToSQL(combined);  // 필요시 같이 저장

                    prevData = combined;
                    Console.WriteLine($"Slave 2 : Len {combined.Length}");
                    Console.WriteLine(string.Join(", ", prevData));
                }
                await Task.Delay(100);
            }

            /*   for (int i = 0; i < 3 ; i++)     //기존

               {
                 // if (token.IsCancellationRequested)//종료 신호를 받으면 루프 탈출
                 //      return;


                   try
                   {
                       ushort[] data = _modbusMaster.ReadHoldingRegisters(deviceAddress[i], startAddress[i], numInputs[i]);
                       if (data == null)
                       {
                        //   MessageBox.Show($"Slave {i} error");
                           continue;
                       }


                     //  _modbusMaster.WriteSingleRegister(deviceAddress[0], 7, (ushort)target_rpm[1]); //10, 1000 갈려서

                       int index = (i == 0) ? 0 : 40;
                       if (i == 2)
                           index = 80;
                       //아래 UI 관련 작업은 UI 스레드에서 안전하게 실행



                       switch (i)
                       {
                           case 0:

                               slave1Data = data;
                               Invoke(new Action(() =>
                               {

                                   test_update(data, index);
                                   //    set_amp_temp(data);   //굳이?
                                   rpm_check(data, index);
                                   roll_check(data, index);
                                   getPosition();
                                   update_amp_temp(data, index);
                                   target_rpm[1] = Convert.ToDecimal(data[7]);
                                   f3.tar_period.Text = target_rpm[1].ToString();
                                   f3.update_Realamp(data, index);
                                   f3.update_temp(data, index);
                                   OnNewData(data, index);
                                      update_info(data, index);


                               }));
                               break;

                           case 1:
                               {
                                   slave2Data = data;
                                   Invoke(new Action(() =>
                                   {
                                       test_update(data, index);
                                       rpm_check(data, index);
                                       roll_check(data, index);
                                       getPosition();
                                       update_amp_temp(data, index);
                                       target_rpm[1] = Convert.ToDecimal(data[7]);
                                       f3.tar_period.Text = target_rpm[1].ToString();
                                       f3.update_Realamp(data, index);
                                       f3.update_temp(data, index);
                                       OnNewData(data, index);
                                       //   update_info(data, index);

                                   }));
                                   break;

                               }

                           case 2:
                               {
                                   BeginInvoke(new Action(() =>
                                   {
                                       test_update(data, index);
                                    //   local_estop(data); //안됨 읽어오는 데이터 변동이 없음

                                       //     warn_check(data);
                                   }));
                                   break;
                               }

                       }


                       // 정상 통신이 이루어졌으므로 오류 플래그 해제
                       _hasShownDisconnectMessage = false;
                   }
                   catch (TimeoutException)
                   {
                   }
                   catch (Exception ex)
                   {
                          Console.WriteLine($"ReadModbusData Slave {i +1 } Exception: " + ex.Message);
                       MessageBox.Show($"slave {i + 1} ERROR");
                   }
               }


               // 🟡 슬레이브1과 슬레이브2 데이터 모두 수신되었을 경우 CSV 저장
               if (slave1Data != null && slave2Data != null)
               {
                   ushort[] combined = new ushort[slave1Data.Length + slave2Data.Length];
                   Array.Copy(slave1Data, 0, combined, 0, slave1Data.Length);
                   Array.Copy(slave2Data, 0, combined, slave1Data.Length, slave2Data.Length);

                //   SaveDataToCSV(combined);
                //   SaveDataToSQL(combined);  // 필요시 같이 저장


                   prevData = combined;
                   Console.WriteLine(string.Join(", ", prevData));
               }*/

        }


        private void local_estop(ushort[] flag)  //LC panel에서 estop 요청시 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => local_estop(flag)));
                return;
            }

            //estop 요청 기준 => Basso DIO 1번이 ON 되면
            if (flag[2] == 1)   //DIO 1 ON
            {
                pic_lc_estop.Image = BUR_INS_HMI.Properties.Resources.local_red;
            }
            else if (flag[2] == 0)  //DIO 1 OFF
            {
                pic_lc_estop.Image = BUR_INS_HMI.Properties.Resources.local_green;
            }
        }

        private void warn_check(ushort[] data) //ROLL 불량시 경고창 (YELLOW or RED)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => warn_check(data)));
                return;
            }

            if (isWarning == false) //[isWarning + @] 조건 설정으로 경고창 띄우는 함수 실행
            {

                show_warning("RED");    //"RED" or "YELLOW" 인수 전달로 경고창 2가지 case
            }
        }

        private void update_info(ushort[] data) //판 정보용 함수
        {
                 if (InvokeRequired)
                 {
                     BeginInvoke(new Action(() => update_info(data)));
                     return;
                 }

            if (data == null)
            {
                for (int i = 0; i < 8; i++)
                {
                    info_lbl[i].Text = null;
                }
                info_lbl[8].Text = "0";
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                info_lbl[i].Text = data[i].ToString();
            }

            info_lbl[8].Text = data[8].ToString("F1");
        }

        private void rpm_check(ushort[] data, int index)     //각 ROLL RPM 확인 함수
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => rpm_check(data, index)));
                return;
            }

            if (data == null)
            {
                for (int i = 0; i < 27; i++)
                {
                    roll_rpm[i].Text = "0";
                }
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
                return;
            }

            int index_offset;
            index_offset = (index == 40) ? 40 : 0;
            int line_offset;
            line_offset = (index == 40) ? 5 : 0;

            //SPARE SENSOR 사용 대비로 인한 하드 코딩
            if (index == 0) //LINE 1 ~5 
            {
                //차이가 difference값 이상 나는 경우만 UI 반영
                //들어오는 데이터 순서랑 ROLL에 표기할 순서랑 반대되므로 [].Text는 index 반대로 대입
                if (Math.Abs((data[0] / 10.0M) - Convert.ToDecimal(roll_rpm[2].Text)) >= difference[0])
                    roll_rpm[2].Text = (data[0] / 10.0M).ToString("F1");  //roll 1-3 
                if (Math.Abs((data[1] / 10.0M) - Convert.ToDecimal(roll_rpm[1].Text)) >= difference[0])
                    roll_rpm[1].Text = (data[1] / 10.0M).ToString("F1");  //roll 1-2
                if (Math.Abs((data[2] / 10.0M) - Convert.ToDecimal(roll_rpm[0].Text)) >= difference[0])
                    roll_rpm[0].Text = (data[2] / 10.0M).ToString("F1");  //roll 1-1 

                if (Math.Abs((data[8] / 10.0M) - Convert.ToDecimal(roll_rpm[4].Text)) >= difference[0])
                    roll_rpm[4].Text = (data[8] / 10.0M).ToString("F1");  //roll 2-2
                if (Math.Abs((data[9] / 10.0M) - Convert.ToDecimal(roll_rpm[3].Text)) >= difference[0])
                    roll_rpm[3].Text = (data[9] / 10.0M).ToString("F1");  //roll 2-1

                if (Math.Abs((data[16] / 10.0M) - Convert.ToDecimal(roll_rpm[7].Text)) >= difference[0])
                    roll_rpm[7].Text = (data[16] / 10.0M).ToString("F1");    //3-3
                if (Math.Abs((data[17] / 10.0M) - Convert.ToDecimal(roll_rpm[6].Text)) >= difference[0])
                    roll_rpm[6].Text = (data[17] / 10.0M).ToString("F1");    //3-2
                if (Math.Abs((data[18] / 10.0M) - Convert.ToDecimal(roll_rpm[5].Text)) >= difference[0])
                    roll_rpm[5].Text = (data[18] / 10.0M).ToString("F1");    //3-1

                if (Math.Abs((data[24] / 10.0M) - Convert.ToDecimal(roll_rpm[10].Text)) >= difference[0])
                    roll_rpm[10].Text = (data[24] / 10.0M).ToString("F1");    //4-3
                if (Math.Abs((data[25] / 10.0M) - Convert.ToDecimal(roll_rpm[9].Text)) >= difference[0])
                    roll_rpm[9].Text = (data[25] / 10.0M).ToString("F1");    //4-2
                if (Math.Abs((data[26] / 10.0M) - Convert.ToDecimal(roll_rpm[8].Text)) >= difference[0])
                    roll_rpm[8].Text = (data[26] / 10.0M).ToString("F1");    //4-1

                if (Math.Abs((data[32] / 10.0M) - Convert.ToDecimal(roll_rpm[12].Text)) >= difference[0])
                    roll_rpm[12].Text = (data[32] / 10.0M).ToString("F1");    //5-2
                if (Math.Abs((data[33] / 10.0M) - Convert.ToDecimal(roll_rpm[11].Text)) >= difference[0])
                    roll_rpm[11].Text = (data[33] / 10.0M).ToString("F1");    //5-1
            }
            else if (index == 40)
            {
                if (Math.Abs((data[0] / 10.0M) - Convert.ToDecimal(roll_rpm[15].Text)) >= difference[0])
                    roll_rpm[15].Text = (data[0] / 10.0M).ToString("F1");   //6-3
                if (Math.Abs((data[1] / 10.0M) - Convert.ToDecimal(roll_rpm[14].Text)) >= difference[0])
                    roll_rpm[14].Text = (data[1] / 10.0M).ToString("F1");   //6-2
                if (Math.Abs((data[2] / 10.0M) - Convert.ToDecimal(roll_rpm[13].Text)) >= difference[0])
                    roll_rpm[13].Text = (data[2] / 10.0M).ToString("F1");   //6-1

                if (Math.Abs((data[8] / 10.0M) - Convert.ToDecimal(roll_rpm[18].Text)) >= difference[0])
                    roll_rpm[18].Text = (data[8] / 10.0M).ToString("F1");   //7-3
                if (Math.Abs((data[9] / 10.0M) - Convert.ToDecimal(roll_rpm[17].Text)) >= difference[0])
                    roll_rpm[17].Text = (data[9] / 10.0M).ToString("F1");   //7-2
                if (Math.Abs((data[10] / 10.0M) - Convert.ToDecimal(roll_rpm[16].Text)) >= difference[0])
                    roll_rpm[16].Text = (data[10] / 10.0M).ToString("F1");   //7-1

                if (Math.Abs((data[16] / 10.0M) - Convert.ToDecimal(roll_rpm[20].Text)) >= difference[0])
                    roll_rpm[20].Text = (data[16] / 10.0M).ToString("F1");   //8-2
                if (Math.Abs((data[17] / 10.0M) - Convert.ToDecimal(roll_rpm[19].Text)) >= difference[0])
                    roll_rpm[19].Text = (data[17] / 10.0M).ToString("F1");   //8-1

                if (Math.Abs((data[24] / 10.0M) - Convert.ToDecimal(roll_rpm[23].Text)) >= difference[0])
                    roll_rpm[23].Text = (data[24] / 10.0M).ToString("F1");   //9-3
                if (Math.Abs((data[25] / 10.0M) - Convert.ToDecimal(roll_rpm[22].Text)) >= difference[0])
                    roll_rpm[22].Text = (data[25] / 10.0M).ToString("F1");   //9-2
                if (Math.Abs((data[26] / 10.0M) - Convert.ToDecimal(roll_rpm[21].Text)) >= difference[0])
                    roll_rpm[21].Text = (data[26] / 10.0M).ToString("F1");   //9-1

                if (Math.Abs((data[32] / 10.0M) - Convert.ToDecimal(roll_rpm[26].Text)) >= difference[0])
                    roll_rpm[26].Text = (data[32] / 10.0M).ToString("F1");   //10-3
                if (Math.Abs((data[33] / 10.0M) - Convert.ToDecimal(roll_rpm[25].Text)) >= difference[0])
                    roll_rpm[25].Text = (data[33] / 10.0M).ToString("F1");   //10-2
                if (Math.Abs((data[34] / 10.0M) - Convert.ToDecimal(roll_rpm[24].Text)) >= difference[0])
                    roll_rpm[24].Text = (data[34] / 10.0M).ToString("F1");   //10-1

            }
        }

        private void getPosition()  //교정중 판정 함수
        {

            //평탄도계 통과, 교정 종료는 추후 진단 프로그램으로부터 데이터 받아서 반영

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => getPosition()));
                return;
            }

            decimal target = target_rpm[0];
            decimal avg = 0;

            //교정 중 판정 기준 : ROLL 4-1, 5-1, 6-1의 속도 평균값이 구동 판정 기준 속도보다 높은 경우 교정 중이라고 판단
            avg = (Convert.ToDecimal(roll_rpm9.Text) + Convert.ToDecimal(roll_rpm12.Text) +
                Convert.ToDecimal(roll_rpm15.Text)) / 3;    

            if (avg >= target)  //교정 중
            {
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_realred_darkdark;   //RED
            }
            else    //교정 X
                pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;  //NONE
        }

        private void roll_check(ushort[] data, int index)   //ROLL 속도 판정 함수
        {

            if (InvokeRequired)
            {

                BeginInvoke(new Action(() => roll_check(data, index)));
                return;
            }

            if (data == null)
            {
                for (int i = 0; i < 27; i++)
                {
                    roll_rpm[i].ForeColor = Color.Black;
                }
                return;
            }


            int offset = (index == 40) ? 14 : 0;     

            for (int i = 0; i < 27; i++)    //속도 기준점은 데이터로 받으니 무관(임시 조건) | 추후 수정 필요
            {
                if ((Convert.ToDecimal(roll_rpm[i].Text) <= 250) && (Convert.ToDecimal(roll_rpm[i].Text) >= 50))
                    roll_rpm[i].ForeColor = Color.Green;    //RPM 정상 범위 내 (GREEN)
                else if ((Convert.ToDecimal(roll_rpm[i].Text) <= 275 && (Convert.ToDecimal(roll_rpm[i].Text) >= 25)))
                    roll_rpm[i].ForeColor = Color.DarkOrange;   //RPM 경고 범위 (YELLOW)
                else
                    roll_rpm[i].ForeColor = Color.Red;  //RPM 불량 범위 (RED)
            }
        }
        private void show_warning(string message)   //점멸하는 경고창 
        {

            if (blinkTimer != null)
                return;     //이미 실행중이면 무시


            isWarning = true;
            isBlink = true;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 1000;
            blinkTimer.Tick += (s, e) =>
            {
                if (message == "RED")
                {
                    panel3.Visible = false;
                    warning_pan.Visible = true;
                    warning_pan.Dock = DockStyle.Right;

                    warning_time.Text = $"{timestamp}";
                    warning_lbl.Text = "구동 불량 발생";
                    if (isBlink == true)
                    {
                        warning_lbl.ForeColor = isFlashing ? Color.Red : Color.Black;
                        warning_pan.BackColor = isFlashing ? Color.Black : Color.Red;
                        warning_pic.BackColor = isFlashing ? Color.Black : Color.Red;
                    }
                    else  //점멸X
                    {
                        blinkTimer?.Stop();
                        blinkTimer?.Dispose();
                        blinkTimer = null;
                        warning_lbl.ForeColor = Color.Red;
                        warning_pan.BackColor = Color.Black;
                        warning_pic.BackColor = Color.Black;

                    }

                }
                else if (message == "YELLOW")
                {
                    panel3.Visible = false;
                    warning_pan.Visible = true;
                    warning_pan.Dock = DockStyle.Right;

                    warning_time.Text = $"{timestamp}";
                    warning_lbl.Text = "구동 경고 발생";
                    if (isBlink == true)
                    {
                        warning_lbl.ForeColor = isFlashing ? Color.Yellow : Color.Black;
                        warning_pan.BackColor = isFlashing ? Color.Black : Color.Yellow;
                        warning_pic.BackColor = isFlashing ? Color.Black : Color.Yellow;
                    }
                    else  //점멸X
                    {
                        blinkTimer?.Stop();
                        blinkTimer?.Dispose();
                        blinkTimer = null;
                        warning_lbl.ForeColor = Color.Yellow;
                        warning_pan.BackColor = Color.Black;
                        warning_pic.BackColor = Color.Black;
                    }

                }
                isFlashing = !isFlashing;
            };
            blinkTimer.Start();
        }
        private void warning_btn_Click(object sender, EventArgs e)
        {
            //  warning_pan.Visible = false;
            //   panel3.Visible = true;
            isBlink = false;
            panel3.Dock = DockStyle.Right;
            // BringToFront();
        }

        private void warning_cls_btn_Click(object sender, EventArgs e)
        {
            warning_pan.Visible = false;
            panel3.Visible = true;
            isBlink = true;
            isWarning = false;
        }
       

     

        private async void exit_btn_Click(object sender, EventArgs e)   // X 버튼으로 종료 
        {
            try
            {
                cancellationTokenSourceForModbus?.Cancel();

                if (_modbusPollingTask != null)
                    await _modbusPollingTask;

                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    if (ExitFlag != true)
                        MessageBox.Show("통신 종료");
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


        private void DiseaseUpdateEventMethodF2toF1(object sender)
        {
            if ("TRUE".Equals(sender.ToString()))   //입력된 string값이 기존 패스워드와 동일한 경우 설정 모드 ON
            {
                f3.login_stat = 1;

                if (f3 == null || f3.IsDisposed)    
                {
                    f3 = new Form3(amp, std_amp, target_rpm, this);
                    f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
                }

                //Form3의 우측 (모듈 관측 패널은 보이지 않고, 모듈 설정 패널만 Visible)

                f3.pipe_panel.Visible = false;     

                f3.setting_panel.Visible = true;
                f3.amp_set10.Visible = true;
                f3.err_set.Visible = true;
                f3.err_set_lbl.Visible = true;
                f3.tar_rpm.Visible = true;
                f3.tar_rpm_lbl.Visible = true;
                f3.tar_period.Visible = true;
                f3.tar_period_lbl.Visible = true;

                f3.Size = new Size(570, 1000);
                f3.edit_pan.Size = new Size(570, 150);

                f3.Show();
                f3.BringToFront();
            }
            else if (!("FALSE".Equals(sender.ToString())))  // 패스워드 변경 모드
            {
                password = sender.ToString();   //입력된 새로운 string으로 패스워드 변경
            }

        }

        private void ampare_btn_Click(object sender, EventArgs e)   //Form3의 좌측 (모듈 설정 패널은 보이지 않고, 모듈 관측 패널만 Visible)
        {
            if (f3 == null || f3.IsDisposed)    //Ver1
            {
                f3 = new Form3(amp, std_amp, target_rpm, this);
                f3.FormSendEvent += new Form3.FormSendDataHandler(DiseaseUpdateEventMethodF2toF1);
            }

            f3.pipe_panel.Visible = true;

            f3.setting_panel.Visible = false;
            f3.err_set.Visible = false;
            f3.err_set_lbl.Visible = false;
            f3.tar_rpm.Visible = false;
            f3.tar_rpm_lbl.Visible = false;
            f3.tar_period.Visible = false;
            f3.tar_period_lbl.Visible = false;
            f3.Size = new Size(650, 950);
            f3.edit_pan.Size = new Size(650, 95);

            f3.Show();
            f3.BringToFront();  //이미 열려있다면 앞으로

            /*    if (f3.ampare_panel.Visible == true)
                    f3.ShowPanel(6);
                else if (f3.ampare_panel.Visible == false)
                    f3.ShowPanel(3);*/
        }



        private void record_btn_Click(object sender, EventArgs e)
        {

        }


        private async void runstop_btn_Click(object sender, EventArgs e)    //stop 버튼 클릭스 통신 종료
        {
            await StopCommunicationAsync();
        }

        private async Task StopCommunicationAsync() //통신 종료 함수
        {
            try
            {
                cancellationTokenSourceForModbus?.Cancel();

                if (_modbusPollingTask != null)
                    await _modbusPollingTask;

                if (serialPort != null && serialPort.IsOpen)
                {

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.Close();
                    isRunning = false;
                    if (ExitFlag != true)
                        MessageBox.Show("통신 종료"); 
                    BeginInvoke(() => end_init());  //통신 종료로 인한 UI 초기화
                    run_btn.Image = BUR_INS_HMI.Properties.Resources.runoff_0809;
                    runstop_btn.Image = BUR_INS_HMI.Properties.Resources.stopon_0809;
                }

                BeginInvoke(() =>   //통신 종료로 인한 UI 초기화
                {

                    roll_check(null, 0);
                    rpm_check(null, 0);
                    update_amp_temp(null, 0);
                    update_info(null); // 
                    InitializeGraphs();
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
                amp[i] = temp1[i]  = temp2[i]  = 0;
                f3.real_amp[i].ForeColor = Color.Black;
                f3.temp1_arr[i].ForeColor = Color.Black;
                f3.temp2_arr[i].ForeColor = Color.Black;
                f3.real_amp[i].Text = "0.0";
                f3.temp1_arr[i].Text = "0.0";
                f3.temp2_arr[i].Text = "0.0";
            }
            
        }

        private void run_btn_Click(object sender, EventArgs e)  //RUN 버튼 클릭시
        {
            if (_modbusPollingTask != null && !_modbusPollingTask.IsCompleted)
            {
                MessageBox.Show("이미 실행 중입니다");
                return;
            }

            bool portOpened = Init_port();

            if (!portOpened)
            {
                isRunning = true;
                MessageBox.Show("포트를 열 수 없습니다. 장치 연결을 확인하세요");
                return;
            }

            cancellationTokenSourceForModbus = new CancellationTokenSource();
            _modbusPollingTask = Task.Run(() => ModbusPollingLoop(cancellationTokenSourceForModbus.Token));
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

                if (!Directory.Exists(capturePath)) //파일 미존재시 파일 생성
                {
                    Directory.CreateDirectory(capturePath);
                }

                //저장 파일명 지정
                string fileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(capturePath, fileName);


                //이미지 저장
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show("Screen Captured");
            }
        }







        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings(settingfilePath);
            
            try
            {
                cancellationTokenSourceForModbus?.Cancel();
                _modbusPollingTask?.Wait(); //안전하게 스레드 종료

                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch { }
        }

        private async void pic_stop_btn_Click_1(object sender, EventArgs e) 
        {
            if (serialPort != null && serialPort.IsOpen)    //포트 연결 되어 있고 통신도중일 때.
            {
                if (!estop)
                {
                    _modbusMaster.WriteSingleRegister(deviceAddress[2], 10, 1); //D3 ON

                    pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.estop_off3;
                    pic_pc_estop.Image = BUR_INS_HMI.Properties.Resources.pc_red;
                    estop = true;
                    MessageBox.Show("장비를 비상 정지합니다.");
                }
                else // estop == true 인 경우
                {
                    if (MessageBox.Show("비상 정지를 해제하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _modbusMaster.WriteSingleRegister(deviceAddress[2], 10, 0); //D3 OFF

                        pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.estop_on;
                        pic_pc_estop.Image = BUR_INS_HMI.Properties.Resources.pc_green;
                        estop = false;
                    }
                }

            }
            else if (estop) //estop == true 인 경우
            {
                if (MessageBox.Show("비상 정지를 해제하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _modbusMaster.WriteSingleRegister(deviceAddress[0], 10, 0); //D3 OFF

                    pic_stop_btn.Image = BUR_INS_HMI.Properties.Resources.estop_on;
                    pic_pc_estop.Image = BUR_INS_HMI.Properties.Resources.pc_green;
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

                //int.TryParse()로 실수형 자동 거부 | 양의 정수형으로만 입력받음
                if (int.TryParse(textBox1.Text, out int minutes) && minutes > 0)    
                {
                    xRangeSeconds = minutes * 60;   //입력된 분 x 60으로 x축 범위 초단위로 생성
                    // 기존 데이터 유지, 시간도 유지
                    UpdateAllPlots(); // X축 갱신
                }
                else
                {
                    MessageBox.Show("1 이상의 정수값를 입력하세요 (분 단위)");
                }
            }

        }

        private void end_init()
        {

            run_btn.Image = BUR_INS_HMI.Properties.Resources.runoff_0809;
            runstop_btn.Image = BUR_INS_HMI.Properties.Resources.stopon_0809;
            //init 추가
            InitializeGraphs();
            //필요 : /sensor/, /position/, /roll/, leveling 초기화

            pic_position1.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
            pic_position2.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;
            pic_position3.Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_darkdark;


            for (int i = 0; i < 10; i++)
            {
                sensor[i].Image = BUR_INS_HMI.Properties.Resources.sen_stat_none_control1;
            }

            for (int n = 0; n < 27; n++)
            {
                roll_rpm[n].ForeColor = Color.Black;
            }


            pic_lc_estop.Image = BUR_INS_HMI.Properties.Resources.local_green;
            pic_pc_estop.Image = BUR_INS_HMI.Properties.Resources.pc_green;

            avg_plot.Plot.Clear();
            ExitFlag = false;
        }


        private void record_btn_Click_1(object sender, EventArgs e)
        {
            Form6 f6 = new Form6();
            f6.Show();
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            //    string filePath = @"C:\Users\User\Desktop\settings.txt"; // 프로그램 실행 폴더에 저장
            SaveSettings(settingfilePath);
           
        }

        private void load_btn_Click(object sender, EventArgs e)
        {
            //   string filePath = @"C:\Users\User\Desktop\settings.txt"; // 프로그램 실행 폴더에 저장

            if (File.Exists(settingfilePath))
            {
                LoadSettings(settingfilePath);
            }
            else
            {
                MessageBox.Show("세팅 파일이 존재하지 않습니다. 임의 초기값으로 설정됩니다.");
            }
        }

        void SaveSettings(string filePath)
        {
            var lines = new List<string>
            {
                "# 프로그램 설정값 저장 파일",
                "//[구동 기준 설정] [RPM 주기 설정] [패스워드] [오차 범위] [모듈별 기준 전류]",
                "//각 설정값 형태 ",
                "//구동 기준 설정 : 소수 한 자리 실수형",
                "//RPM 주기 설정 : 10 ~ 60000 의 10단위 정수형",
                "//패스워드 : 8자리 이하",
                "//오차 범위 : 소수 한 자리 실수형",
                "//모듈별 기준 전류 : 소수 한 자리 실수형",
                "-----------------------------",
               "구동 기준 설정=" + target_rpm[0],
              "RPM 주기 설정=" + target_rpm[1],
              "패스워드=" + password,
              "오차 범위=" + amp[10],

                              "-----------------------------",
              "[모듈별 기준 전류]",
              "모듈 1=" + std_amp[0],
               "모듈 2=" + std_amp[1],
                "모듈 3=" + std_amp[2],
                 "모듈 4=" + std_amp[3],
                  "모듈 5=" + std_amp[4],
                   "모듈 6=" + std_amp[5],
                    "모듈 7=" + std_amp[6],
                     "모듈 8=" + std_amp[7],
                      "모듈 9=" + std_amp[8],
                       "모듈 10=" + std_amp[9],
            };

            File.WriteAllLines(filePath, lines);
            MessageBox.Show("설정값이 저장되었습니다.", "Settings saved");
        }

        void LoadSettings(string filePath)
        {
            if (!File.Exists(filePath)) return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var trimmed = line.Trim();

                // 빈 줄, 주석 무시
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith("//"))
                    continue;

                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "구동 기준 설정":
                        if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]+(\.[0-9]{1})?$"))
                        {
                            target_rpm[0] = decimal.Parse(value);
                            f3.tar_rpm.Text = target_rpm[0].ToString("F1");
                        }
                        else
                            MessageBox.Show("유효하지 않은 구동 기준 설정값입니다.");
                        break;

                    case "RPM 주기 설정":
                        if (int.TryParse(value, out int rpmPeriod))
                        {
                            if (rpmPeriod % 10 == 0)
                            {
                                if (rpmPeriod >= 10 && rpmPeriod <= 60000)
                                {
                                    target_rpm[1] = rpmPeriod;
                                    f3.tar_period.Text = target_rpm[1].ToString();
                                }
                                else
                                    MessageBox.Show("유효하지 않은 RPM 주기 설정값입니다.");
                           //     MessageBox.Show("PASSED");
                            }
                            else
                                MessageBox.Show("유효하지 않은 RPM 주기 설정값입니다.");
                           
                        }
                        else
                        {
                         //   MessageBox.Show("TryParse INT X");
                            MessageBox.Show("유효하지않은 RPM 주기 설정값입니다.");
                        }
                        break;

                    case "패스워드":
                        if (value.Length <= 8)
                            password = value;
                        else
                            MessageBox.Show("유효하지 않은 패스워드 설정값입니다.");
                        break;

                    case "오차 범위":
                        if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]+(\.[0-9]{1})?$")
                            && decimal.Parse(value) <= 100) //소수 한 자리 수의 실수 && 100 이하 이면
                        {
                            amp[10] = decimal.Parse(value);
                            f3.err_set.Text = amp[10].ToString("F1");
                        }
                        else
                            MessageBox.Show("유효하지 않은 오차 범위 설정값입니다.");
                        break;

                    case "모듈 1":
                        check_txt_ampset(value, 0);

                        break;

                    case "모듈 2":
                        check_txt_ampset(value, 1);
                        break;

                    case "모듈 3":
                        check_txt_ampset(value, 2);
                        break;

                    case "모듈 4":
                        check_txt_ampset(value, 3);
                        break;

                    case "모듈 5":
                        check_txt_ampset(value, 4);
                        break;

                    case "모듈 6":
                        check_txt_ampset(value, 5);
                        break;

                    case "모듈 7":
                        check_txt_ampset(value, 6);
                        break;

                    case "모듈 8":
                        check_txt_ampset(value, 7);
                        break;

                    case "모듈 9":
                        check_txt_ampset(value, 8);
                        break;

                    case "모듈 10":
                        check_txt_ampset(value, 9);
                        break;
                }
            }

            MessageBox.Show("설정값을 불러옵니다.","Settings Loaded");
        }

        void check_txt_ampset(string value, int index)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]+(\.[0-9]{1})?$"))
            {
                std_amp[index] = decimal.Parse(value);
                f3.amp_set[index].Text = std_amp[index].ToString("F1");
                f3.tar_amp[index].Text = (f3.standard[index] * f3.channels[index]).ToString("F1");
            }
            else
                MessageBox.Show("유효하지 않은 전류 설정값입니다.");
        }


    }   //end of Form1
}
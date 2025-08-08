using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace scott
{
    public partial class Form1 : Form
    {
        private double[] heights;
        private Random rand = new Random();
        private Timer timer;

        // 'bar'를 지역변수 대신 필드로 쓰려면 'object'로 선언해도 됨
        private object bar;

        public Form1()
        {
            InitializeComponent();

            heights = new double[] { 10, 20, 30, 40, 50 };

            // Bar 추가
            bar = formsPlot1.Plot.Add.Bars(heights);

            // bar가 object 타입이라면 캐스팅해서 속성 설정
            ((ScottPlot.Plottables.Bar)bar).BarWidth = 0.8;
            ((ScottPlot.Plottables.Bar)bar).FillColor = Colors.SteelBlue;

            formsPlot1.Refresh();

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            // 값 업데이트 (랜덤 변동)
            for (int i = 0; i < heights.Length; i++)
                heights[i] += rand.Next(-5, 6);

            // Plottable의 Heights 변경
            bar.Heights = heights;

            // 화면 갱신 (Clear() 안 씀)
            formsPlot1.Refresh();
        }

    }
}
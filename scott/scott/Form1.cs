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

        // 'bar'�� �������� ��� �ʵ�� ������ 'object'�� �����ص� ��
        private object bar;

        public Form1()
        {
            InitializeComponent();

            heights = new double[] { 10, 20, 30, 40, 50 };

            // Bar �߰�
            bar = formsPlot1.Plot.Add.Bars(heights);

            // bar�� object Ÿ���̶�� ĳ�����ؼ� �Ӽ� ����
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
            // �� ������Ʈ (���� ����)
            for (int i = 0; i < heights.Length; i++)
                heights[i] += rand.Next(-5, 6);

            // Plottable�� Heights ����
            bar.Heights = heights;

            // ȭ�� ���� (Clear() �� ��)
            formsPlot1.Refresh();
        }

    }
}
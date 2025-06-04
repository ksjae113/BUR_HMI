using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottPlot;

namespace BUR_INS_HMI
{
    public partial class Form3 : Form
    {
        public delegate void FormSendDataHandler(string s);
        public event FormSendDataHandler FormSendEvent;
        int column = 10;
        decimal amp = 50;   //초기값
        decimal err = 10;   //초기값


        private Queue<double> pulseData = new Queue<double>();
        public Func<byte> GetDOByte;


        public Form3()
        {
            InitializeComponent();
            amp_set.Text = amp.ToString("F1") + " mA";
            err_set.Text = err.ToString("F1") + " %";
            update_amp_err(amp, err);

            Init_pulse();
            pulseTime.Tick += Timer_Tick;
            pulseTime.Start();
        }

        private void Init_pulse()
        {
            var plt = formsPlot.Plot;
            plt.SetAxisLimits(yMin: 0, yMax: 1);
                
            plt.YAxis.ManualTickPositions(new double[] { 0, 1 }, new string[] { "0", "1" });
            double[] xs = ScottPlot.DataGen.Consecutive(50);
            double[] ys = new double[50]; // 모두 0으로 초기화

            plt.AddScatter(xs, ys);
            formsPlot.Refresh();
        }

        

       private void Timer_Tick(object sender, EventArgs e)
        {
            byte raw = GetDOByte?.Invoke() ?? 0;
            double val = (raw & 0x01) > 0 ? 1 : 0;

            if (pulseData.Count >= 100)
                pulseData.Dequeue();

            pulseData.Enqueue(val);

            formsPlot.Plot.Clear();
            formsPlot.Plot.AddSignal(pulseData.ToArray(), sampleRate: 10);
            formsPlot.Render();
        }
        

        public void ShowPanel(int panelIndex)
        {


            switch (panelIndex)       //이거 써도 별차이없는듯
            {
                case 1:
                    temp_panel.Visible = true;
                    break;
                case 2:
                    sensor_panel.Visible = true;
                    break;
                case 3:
                    ampare_panel.Visible = true;
                    break;
                case 4:
                    temp_panel.Visible = false;
                    break;
                case 5:
                    sensor_panel.Visible = false;
                    break;
                case 6:
                    ampare_panel.Visible = false;
                    break;

            }



        }


        private void update_amp_err(decimal amp, decimal err)   //목표전류, 정상전류 범위 계산 및 텍스트 업데이트
        {
            decimal[] channels = new decimal[] { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };
            Label[] trg_amp = new Label[] { target_amp1, target_amp2, target_amp3, target_amp4, target_amp5, target_amp6, target_amp7, target_amp8, target_amp9, target_amp10 };
            Label[] err_ran = new Label[] { err_range1, err_range2, err_range3, err_range4, err_range5, err_range6, err_range7, err_range8, err_range9, err_range10 };

            amp_set.Text = amp.ToString("F1") + " mA";
            err_set.Text = err.ToString("F1") + " %";


            for (int i = 0; i < column; i++)
            {
                decimal ampare = amp * channels[i];
                decimal min = ampare * ((100.0M - err) / 100.0M);
                decimal max = ampare * ((100.0M + err) / 100.0M);
                trg_amp[i].Text = ampare.ToString("F1") + " mA";
                err_ran[i].Text = min.ToString("F1") + " ~ " + max.ToString("F1") + "\nmA";
            }
        }



        private void DiseaseUpdateEventMethod2(object sender, object unit)  //f4 입력에 맞춰 전류 업데이트
        {
            if ("mA".Equals(unit.ToString()))
            {
                amp = Convert.ToDecimal(sender.ToString());
            }
            else if ("%".Equals(unit.ToString()))
            {
                err = Convert.ToDecimal(sender.ToString());
            }

            if (amp != 0 || err != 0)
            {
                update_amp_err(amp, err);
            }
        }


        private void amp_set_btn_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethod2);
            f4.Show();
        }

        private void err_set_btn_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "%";
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethod2);
            f4.Show();
        }

       
    }




}

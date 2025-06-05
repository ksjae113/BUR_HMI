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

        public Label[] temp1_arr;
        public Label[] temp2_arr;
        public Label[] col1_arr;
        public Label[] col2_arr;

    private Queue<double> pulseData = new Queue<double>();
        public Func<byte> GetDOByte;
        

        public Form3()
        {
            InitializeComponent();

            amp_set.Text = amp.ToString("F1") + " mA";
            err_set.Text = err.ToString("F1") + " %";
            update_amp_err(amp, err);

            Init_pulse();
            temp1_arr = new Label[]{ temp_1, temp_2, temp_3, temp_4, temp_5, temp_6, 
                temp_7, temp_8, temp_9, temp_10 };
            temp2_arr = new Label[]{ temp2_1, temp2_2, temp2_3, temp2_4, temp2_5, temp2_6, 
                temp2_7, temp2_8, temp2_9, temp2_10 };
            col1_arr = new Label[] { col1_1, col1_2, col1_3, col1_4, col1_5, col1_6, col1_7, col1_8, col1_9, col1_10 };
            col2_arr = new Label[] { col2_1, col2_2, col2_3, col2_4, col2_5, col2_6, col2_7, col2_8, col2_9, col2_10 };
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

        


        
        public void SharedTimerCallback(byte raw)
        {
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

using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUR_INS_HMI
{
    public partial class Form3 : Form
    {
        public delegate void FormSendDataHandler(string s);
        public event FormSendDataHandler FormSendEvent;
        int column = 10;
        decimal amp = 50;   //초기값
        decimal err = 10;   //초기값

        public Form3()
        {
            InitializeComponent();
            amp_set.Text = amp.ToString("F1") + " mA";
            err_set.Text = err.ToString("F1") + " %";
            update_amp_err(amp, err);
        }

        public void ShowPanel(int panelIndex)
        {


               switch (panelIndex)       //이거 써도 별차이없는듯
               {
                   case 1: temp_panel.Visible = true;
                       break;
                   case 2: sensor_panel.Visible = true; 
                       break;
                   case 3: ampare_panel.Visible = true; 
                       break;
                   case 4: temp_panel.Visible = false;
                       break;
                   case 5 : sensor_panel.Visible = false;
                       break;
                   case 6 : ampare_panel.Visible = false;
                       break;

               }
            
        /*    switch (panelIndex)
            {
                case 1:
                    {
                        temp_panel.Visible = true;
                        temp_lbl.Visible = true;
                    }
                    break;
                case 2:
                    {
                        sensor_panel.Visible = true;
                        sens_lbl.Visible = true;
                    }
                    break;

                case 3:
                    {
                        ampare_panel.Visible = true;
                        ampare_lbl.Visible = true;
                    }
                    break;
                case 4:
                    {
                        temp_panel.Visible = false;
                        temp_lbl.Visible = false;
                    }
                    break;
                case 5:
                    {
                        sensor_panel.Visible = false;
                        sensor_panel.Visible = false;
                    }
                    break;
                case 6:
                    {
                        ampare_panel.Visible = false;
                        ampare_lbl.Visible = false;
                    }
                    break;
            }*/

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
                decimal min = ampare * ((100.0M - err)/100.0M);
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

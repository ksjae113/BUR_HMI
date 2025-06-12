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
using System.Diagnostics;

namespace BUR_INS_HMI
{
    public partial class Form3 : Form
    {
        public delegate void FormSendDataHandler(string s);
        public event FormSendDataHandler FormSendEvent;
        int column = 10;
        public decimal amp = 50;   //초기값
        public decimal err = 10;   //초기값

        public Label[] temp1_arr;
        public Label[] temp2_arr;
        public Label[] col1_arr;
        public Label[] col2_arr;
        public decimal[] channels = new decimal[] { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };
        public decimal[] min = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public decimal[] max = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        private Queue<double> pulseData = new Queue<double>();
        public Func<byte> GetDOByte;

        internal InfoPanel infoCopy;

        public Form3()
        {
            InitializeComponent();

            amp_set.Text = amp.ToString("F1") + " mA";
            err_set.Text = err.ToString("F1") + " %";

            infoCopy = new InfoPanel();
            infoCopy.Dock = DockStyle.Left;

            this.Controls.Add(infoCopy);
            Controls.Add(ampare_panel);
            Controls.Add(temp_panel);
            Controls.Add(line_pan);
            Controls.Add(edit_pan);
            //   copyroll(f1.roll_pan);
            update_amp_err(amp, err);


            temp1_arr = new Label[]{ temp_1, temp_2, temp_3, temp_4, temp_5, temp_6, 
                temp_7, temp_8, temp_9, temp_10 };
            temp2_arr = new Label[]{ temp2_1, temp2_2, temp2_3, temp2_4, temp2_5, temp2_6, 
                temp2_7, temp2_8, temp2_9, temp2_10 };
          
        }

        internal void roll_check(ushort[] data)
        {
            for (int i = 0; i < data.Length && i < 27; i++)
            {
                if (data[i] == 1)
                {
                    infoCopy.roll[i].BackColor = Color.Red;
                }
                else
                    infoCopy.roll[i].BackColor = Color.Blue;
            }

            Debug.WriteLine("Modbus data received: " + string.Join(", ", data));
        }

        /* internal void copyroll(InfoPanel p)
         {
             for (int i =0;i<27;i++)
             {
                 infoCopy.roll[i].BackColor = p.roll[i].BackColor;
             }
         }*/

        internal void updateAmpare(byte d)
        {
            Label[] amp_arr = new Label[] { amp1, amp2, amp3, amp4,amp5,amp6,amp7,
            amp8,amp9,amp10};
            



            for (int i = 0; i < 10; i++)
            {
                decimal new_amp = amp * channels[i];

                if (d == 0x01 && i == 0)   //0,1 체크용 임시
                {
                    new_amp = 0.1M;
                }

                amp_arr[i].Text = new_amp.ToString("F1") + " mA";
                if (new_amp < min[i] || new_amp > max[i])   //range check
                {
                    amp_arr[i].BackColor = Color.Red;
                    amp_arr[i].ForeColor = Color.Maroon;

                }
                else
                {
                    amp_arr[i].BackColor = Color.Black;
                    amp_arr[i].ForeColor = Color.White;

                }
            }

        }


       

        public void ShowPanel(int panelIndex)
        {


            switch (panelIndex)       //이거 써도 별차이없는듯
            {
                case 1:
                    temp_panel.Visible = true;
                    break;
                case 2:
                //    sensor_panel.Visible = true;
                    break;
                case 3:
                    ampare_panel.Visible = true;
                    break;
                case 4:
                    temp_panel.Visible = false;
                    break;
                case 5:
                 //   sensor_panel.Visible = false;
                    break;
                case 6:
                    ampare_panel.Visible = false;
                    break;
            }
        }

        

        private void update_amp_err(decimal amp, decimal err)   //목표전류, 정상전류 범위 계산 및 텍스트 업데이트
        {

            amp_set.Text = amp.ToString("F1") + " mA";
            err_set.Text = err.ToString("F1") + " %";


            for (int i = 0; i < column; i++)
            {
                decimal ampare = amp * channels[i];
                min[i] = ampare * ((100.0M - err) / 100.0M);
                max[i] = ampare * ((100.0M + err) / 100.0M);
            }
        }



        private void DiseaseUpdateEventMethodF4toF3(object sender, object unit)  //f4 입력에 맞춰 전류 업데이트
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
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.Show();
        }

        private void err_set_btn_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "%";
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.Show();
        }

       
    }




}

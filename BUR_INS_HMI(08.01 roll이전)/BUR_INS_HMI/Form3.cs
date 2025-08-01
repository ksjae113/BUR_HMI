﻿using ScottPlot;
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
using System.Runtime.InteropServices.Marshalling;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Modbus.Device;
using Modbus.Extensions.Enron;

namespace BUR_INS_HMI
{
    public partial class Form3 : Form
    {
        public delegate void FormSendDataHandler(string s);
        public event FormSendDataHandler FormSendEvent;

        int column = 10;

        int index = -1;

        public int login_stat = 0;

        

        public Label[] temp1_arr;
        public Label[] temp2_arr;
        public Label[] col1_arr;
        public Label[] col2_arr;
        public Label[] amp_set;
        public Label[] tar_amp;
        public Label[] real_amp;
      
        public decimal[] channels = new decimal[] { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };
        
        public decimal[] min = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public decimal[] max = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
     //   public int[] sensor = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        internal Button[] set_btn;

        private Queue<double> pulseData = new Queue<double>();
        public Func<byte> GetDOByte;

     //   internal InfoPanel infoCopy;

        private decimal[] amp;
        public decimal[] standard;
        public decimal[] target_rpm;
        private ModbusSerialMaster _modbusMaster;
        private Form1 _mainForm;
        public Form3(decimal[] f1amp, decimal[] temp, decimal[] std_amp, decimal[] t_rpm, Form1 mainForm)
        {
            InitializeComponent();

            amp = f1amp;    //amp값 : 라벨인 real_amp와 연결
            standard = std_amp;
            target_rpm = t_rpm;
            _mainForm = mainForm;

            err_set.Text = amp[10].ToString("F1");

         //   infoCopy = new InfoPanel();
         //   infoCopy.Dock = DockStyle.Left;

         //   this.Controls.Add(infoCopy);
            Controls.Add(pipe_panel);
            Controls.Add(line_pan);
            Controls.Add(edit_pan);


            temp1_arr = new Label[]{ temp_1, temp_2, temp_3, temp_4, temp_5, temp_6,
                temp_7, temp_8, temp_9, temp_10 };
            temp2_arr = new Label[]{ temp2_1, temp2_2, temp2_3, temp2_4, temp2_5, temp2_6,
                temp2_7, temp2_8, temp2_9, temp2_10 };
            amp_set = new Label[] { amp_set1,amp_set2,amp_set3,amp_set4,amp_set5,amp_set6,amp_set7,
            amp_set8,amp_set9,amp_set10};
            tar_amp = new Label[]{tar_amp1,tar_amp2,tar_amp3,tar_amp4,tar_amp5,tar_amp6,
            tar_amp7,tar_amp8,tar_amp9,tar_amp10};

          /*  set_btn = new Button[] { amp_set_btn1,amp_set_btn2, amp_set_btn3, amp_set_btn4,
                amp_set_btn5,amp_set_btn6,amp_set_btn7,amp_set_btn8,amp_set_btn9,amp_set_btn10 };*/

            real_amp = new Label[] { amp1, amp2, amp3, amp4, amp5, amp6, amp7, amp8, amp9, amp10 };

            init_amp();
        }

        private void init_amp()
        {
            err_set.Text = amp[10].ToString("F1");
            //    MessageBox.Show("ERR_RANGE : " + err_set.Text);
            for (int i = 0; i < 10; i++)
            {
                //   amp_set[i].Text = amp[i].ToString("F1");    //기준전류 제대로 확립 수정 필요
                amp_set[i].Text = standard[i].ToString("F1");
                tar_amp[i].Text = (standard[i] * channels[i]).ToString("F1");
                real_amp[i].Text = (amp[i] * channels[i]).ToString("F1");
                
                //    MessageBox.Show($"amp[{i}] = {amp_set[i].Text} mA\ntar_amp[{i}] = {tar_amp[i].Text}");
            }
            tar_rpm.Text = target_rpm[0].ToString("F1");
            update_err_all();
        }

     //   internal void update_Realamp(decimal[] a, int index)
        internal void update_Realamp(ushort[] data, int index)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => update_Realamp(data, index)));
                return;
            }

            int offset = (index == 40) ? 5 : 0;

            /*    for (int i = 0; i < 10; i++)
                {
                    real_amp[i].Text = (data[i] * channels[i]).ToString("F1");
                    if (Convert.ToDecimal(real_amp[i].Text) < 1)
                        real_amp[i].ForeColor = Color.Red;
                    else
                        real_amp[i].ForeColor = Color.Blue;
                }*/

            for (int i = 0; i < 5; i++)
            {
                real_amp[i + offset].Text = ((data[i * 8 + 6]) * 0.1).ToString("F1");

                if ((Convert.ToDecimal(real_amp[i + offset].Text) >= min[i + offset] &&
                    (Convert.ToDecimal(real_amp[i + offset].Text) <= max[i + offset])))
                {
                    real_amp[i + offset].ForeColor = Color.Blue;
                }
                else
                    real_amp[i + offset].ForeColor = Color.Red;
            }
        }

        internal void end_amp_temp()
        {
            for (int i = 0; i < 10; i++)
            {
                temp1_arr[i].ForeColor = Color.Blue;
                temp2_arr[i].ForeColor = Color.Blue;
                amp_set[i].ForeColor = Color.Blue;
            }
        }

        internal void update_temp(ushort[] t , int index)  //온도는 2개임.수정 완    //stop시 변경은 init으로 따로만들어야할듯
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => update_temp(t , index)));
                return;
            }


            int offset;
            offset = (index == 40) ? 5 : 0;
            for (int i = 0; i < 5; i++)
            {

                temp1_arr[i + offset].Text = (t[i * 8 + 4] * 0.1).ToString("F1");
                temp2_arr[i + offset].Text = (t[i * 8 + 5] * 0.1).ToString("F1");

                if ((Convert.ToDecimal(temp1_arr[i + offset].Text) >= 20 && (Convert.ToDecimal(temp1_arr[i + offset].Text) <= 35)))
                {
                    temp1_arr[i + offset].ForeColor = Color.Blue;
                }
                else
                    temp1_arr[i + offset].ForeColor = Color.Red;

                if ((Convert.ToDecimal(temp2_arr[i + offset].Text) >= 20 && (Convert.ToDecimal(temp2_arr[i + offset].Text) <= 35)))
                {
                    temp2_arr[i + offset].ForeColor = Color.Blue;
                }
                else
                    temp2_arr[i + offset].ForeColor = Color.Red;
            }

        }


        public void ShowPanel(int panelIndex)
        {


            switch (panelIndex)       //이거 써도 별차이없는듯
            {
                case 1:
                    pipe_panel.Visible = true;
                    break;
                case 2:
                    //    sensor_panel.Visible = true;
                    {

                    }
                    break;
                case 3:
                  //  ampare_panel.Visible = true;
                    break;
                case 4:
                    pipe_panel.Visible = false;
                    break;
                case 5:
                    //   sensor_panel.Visible = false;
                    break;
                case 6:
                //    ampare_panel.Visible = false;
                    break;
            }
        }



        private void update_err_all()   //목표전류, 정상전류 범위 계산 및 텍스트 업데이트
        {
            //오차 5%기준 0.5mA정도 지정하면 GREEN 확인가능.
            err_set.Text = amp[10].ToString("F1");
            tar_rpm.Text = target_rpm[0].ToString("F1");

            for (int i = 0; i < column; i++)
            {
                decimal ampare = standard[i] * channels[i];
                min[i] = ampare * ((100.0M - amp[10]) / 100.0M);
                max[i] = ampare * ((100.0M + amp[10]) / 100.0M);
            }
        }

        private void update_err_index(int index)    //정상 전류 범위 개별 업데이트
        {
            decimal ampare = standard[index] * channels[index];
            tar_amp[index].Text = ampare.ToString("F1");
            min[index] = ampare * ((100.0M - amp[10]) / 100.0M);
            max[index] = ampare * ((100.0M + amp[10]) / 100.0M);
      

        }


        private void DiseaseUpdateEventMethodF4toF3(object sender, object unit)  //f4 입력에 맞춰 업데이트
        {
            decimal new_amp;
            if ("mA".Equals(unit.ToString()))
            {
                new_amp = Convert.ToDecimal(sender.ToString());

                standard[index] = new_amp;
                amp_set[index].Text = standard[index].ToString("F1");
                update_err_index(index);
                index = -1;
            }
            else if ("%".Equals(unit.ToString()))
            {
                amp[10] = Convert.ToDecimal(sender.ToString());
                err_set.Text = amp[10].ToString("F1");
                update_err_all();
            }
            else if ("RPM".Equals(unit.ToString()))
            {
                target_rpm[0] = Convert.ToDecimal(sender.ToString());
                tar_rpm.Text = target_rpm[0].ToString("F1");
                update_err_all();
            }
            else if ("PERIOD".Equals(unit.ToString()))
            {

                target_rpm[1] = Convert.ToDecimal(sender.ToString());
                tar_period.Text = target_rpm[1].ToString();

                _mainForm._modbusMaster.WriteSingleRegister(1, 7, (ushort)target_rpm[1]);
                _mainForm._modbusMaster.WriteSingleRegister(2, 7, (ushort)target_rpm[1]);
            //    MessageBox.Show("CHANGED");
            }
        }

        private void tar_period_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "PERIOD";
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();

        }

        private void DiseaseUpdateEventMethodF4toF3allamp(object sender, object unit)
        {
            decimal new_amp;

            new_amp = Convert.ToDecimal(sender.ToString());

            for (int i = 0; i < 10; i++)
            {
                standard[i] = new_amp;
                amp_set[i].Text = standard[i].ToString("F1");
                update_err_index(i);
            }
            index = -1;
        }

        private void err_set_btn_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "%";
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn1_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 0;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn2_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 1;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn3_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 2;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn4_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 3;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn5_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 4;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn6_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 5;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn7_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 6;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn8_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 7;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn9_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 8;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void amp_set_btn10_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            index = 9;

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
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

        private void amp_set_btn_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "mA";

            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3allamp);
            f4.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.unit_lbl.Text = "RPM";
            f4.FormSendEvent2 += new Form4.FormSendDataHandler2(DiseaseUpdateEventMethodF4toF3);
            f4.ShowDialog();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            login_stat = -1;
        }
    }

}

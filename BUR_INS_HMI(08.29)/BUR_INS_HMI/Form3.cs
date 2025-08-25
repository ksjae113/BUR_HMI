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
        private Form1 _mainForm;
        public Form3(decimal[] f1amp, decimal[] std_amp, decimal[] t_rpm, Form1 mainForm)
        {
            InitializeComponent();

            amp = f1amp;    
            standard = std_amp;
            target_rpm = t_rpm;
            _mainForm = mainForm;

            err_set.Text = amp[10].ToString("F1");

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

            real_amp = new Label[] { amp1, amp2, amp3, amp4, amp5, amp6, amp7, amp8, amp9, amp10 };

            init_amp(); //form3 초기화
        }

        private void init_amp() //form3 초기화
        {
            err_set.Text = amp[10].ToString("F1");  //오차 범위 세팅
            for (int i = 0; i < 10; i++)
            {
                amp_set[i].Text = standard[i].ToString("F1");   //각 기준전류 값으로 세팅
                tar_amp[i].Text = (standard[i] * channels[i]).ToString("F1");   //라인별 기준전류 
                real_amp[i].Text = (amp[i] * channels[i]).ToString("F1");   //실측값 
                temp1_arr[i].Text = _mainForm.temp1[i].ToString("F1");
                temp2_arr[i].Text = _mainForm.temp2[i].ToString("F1");

            }
            tar_rpm.Text = target_rpm[0].ToString("F1");    //RPM 구동 기준값
            tar_period.Text = target_rpm[1].ToString();     //RPM 주기값
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

            for (int i = 0; i < 5; i++)
            {
                // 차이가 difference값 이상 나는 경우만 데이터값 텍스트 UI 반영

                if (Math.Abs(data[i * 8 + 6] * 0.1M - Convert.ToDecimal(real_amp[i+offset].Text)) >= _mainForm.difference[0])
                {
                    real_amp[i + offset].Text = (data[i * 8 + 6] * 0.1).ToString("F1");
                }

             //   real_amp[i + offset].Text = ((data[i * 8 + 6]) * 0.1).ToString("F1");

                if ((Convert.ToDecimal(real_amp[i + offset].Text) >= _mainForm.min[i + offset] &&
                    (Convert.ToDecimal(real_amp[i + offset].Text) <= _mainForm.max[i + offset])))
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
                temp1_arr[i].ForeColor = Color.Black;
                temp2_arr[i].ForeColor = Color.Black;
                amp_set[i].ForeColor = Color.Black;
            }
        }

        internal void update_temp(ushort[] data , int index)  //온도는 2개임.수정 완    //stop시 변경은 init으로 따로만들어야할듯
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => update_temp(data , index)));
                return;
            }


            int offset;
            offset = (index == 40) ? 5 : 0;

            for (int i = 0; i < 5; i++)
            {
                // 새로 읽은 값
                decimal newValue = data[i * 8 + 4] * 0.1M;
                decimal currentValue = Convert.ToDecimal(temp1_arr[i + offset].Text);

                // 값이 일정 차이(difference) 이상 변할 때만 UI 갱신
                if (Math.Abs(newValue - currentValue) >= _mainForm.difference[0])
                {
                    temp1_arr[i + offset].Text = newValue.ToString("F1");
                    currentValue = newValue; // 값 갱신
                }

                if ((Convert.ToDecimal(temp1_arr[i + offset].Text) >= 30 &&
                   (Convert.ToDecimal(temp1_arr[i + offset].Text) <= 40)))
                {
                    temp1_arr[i + offset].ForeColor = Color.Blue;

                }
                else
                    temp1_arr[i + offset].ForeColor = Color.Red;

                newValue = data[i * 8 + 5] * 0.1M;
                currentValue = Convert.ToDecimal(temp2_arr[i + offset].Text);
                // 값이 일정 차이(difference) 이상 변할 때만 UI 갱신
                if (Math.Abs(newValue - currentValue) >= _mainForm.difference[0])
                {
                    temp2_arr[i + offset].Text = newValue.ToString("F1");
                    currentValue = newValue; // 값 갱신
                }
                if ((Convert.ToDecimal(temp2_arr[i + offset].Text) >= 30 &&
                   (Convert.ToDecimal(temp2_arr[i + offset].Text) <= 40)))
                {
                    temp2_arr[i + offset].ForeColor = Color.Blue;

                }
                else
                    temp2_arr[i + offset].ForeColor = Color.Red;


            }

            //컬러 판정 추가 필요
        }


        public void ShowPanel(int panelIndex)
        {


            switch (panelIndex)
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



        private void update_err_all()   //오차 범위 전체 수정으로 목표전류, 정상전류 범위 전체 계산 및 텍스트 업데이트
        {
            err_set.Text = amp[10].ToString("F1");          //오차 범위 업데이트
            tar_rpm.Text = target_rpm[0].ToString("F1");    //RPM 구동 기준 업데이트

            for (int i = 0; i < column; i++)
            {
                decimal ampare = standard[i] * channels[i]; //해당 라인 목표 전류값 계산
                min[i] = ampare * ((100.0M - amp[10]) / 100.0M);    //오차 범위따라 최소값 계산
                max[i] = ampare * ((100.0M + amp[10]) / 100.0M);    //오차 범위따라 최대값 계산
            }
        }

        private void update_err_index(int index)   //오차 범위 개별 수정으로 목표전류, 정상전류 범위 전체 계산 및 텍스트 업데이트
        {
            decimal ampare = standard[index] * channels[index]; //해당 라인 목표 전류값 계산
            tar_amp[index].Text = ampare.ToString("F1");
            min[index] = ampare * ((100.0M - amp[10]) / 100.0M);    //오차 범위따라 최소값 계산   
            max[index] = ampare * ((100.0M + amp[10]) / 100.0M);    //오차 범위따라 최대값 계산
      

        }


        private void DiseaseUpdateEventMethodF4toF3(object sender, object unit)  //f4 입력에 맞춰 업데이트
        {
            decimal new_amp;
            if ("mA".Equals(unit.ToString()))   //개별 전류값 수정시
            {
                new_amp = Convert.ToDecimal(sender.ToString());

                standard[index] = new_amp;  //new_amp 기준 전류 배열[]에 
                amp_set[index].Text = standard[index].ToString("F1");   //해당 배열 값으로 라벨 Text 업데이트
                update_err_index(index);    //index 값 따라서 해당 라인 수정값 업데이트
                index = -1;
            }
            else if ("%".Equals(unit.ToString()))   //오차 범위 수정시
            {
                amp[10] = Convert.ToDecimal(sender.ToString());
                err_set.Text = amp[10].ToString("F1");
                update_err_all();   //오차 범위 수정으로 전체 전류 범위 업데이트
            }
            else if ("RPM".Equals(unit.ToString())) //RPM 구동 기준 수정시
            {
                target_rpm[0] = Convert.ToDecimal(sender.ToString());
                tar_rpm.Text = target_rpm[0].ToString("F1");
                update_err_all();
            }
            else if ("PERIOD".Equals(unit.ToString()))  //RPM 주기 수정시
            {
                try
                {   // RPM 주기 수정값 Slave 1, 2 에 전송
                    _mainForm._modbusMaster.WriteSingleRegister(_mainForm.deviceAddress[1], 7, (ushort)target_rpm[1]);
                    _mainForm._modbusMaster.WriteSingleRegister(_mainForm.deviceAddress[2], 7, (ushort)target_rpm[1]);
                
                    // UI 반영
                    target_rpm[1] = Convert.ToDecimal(sender.ToString());
                    tar_period.Text = target_rpm[1].ToString();
                  
                }
                catch
                {
                    MessageBox.Show("장치가 정상적으로 연결되지 않아 수정할 수 없습니다.");
                }
               
                   
               

                //기존
                
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

        private void amp_set_btn1_Click(object sender, EventArgs e) //추후 tag로 수정 ?
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
                //   string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //   string desktopPath = @"C: \Users\User\Desktop\storage";
                

                //저장 파일명 지정
                string fileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(_mainForm.capturePath, fileName);


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

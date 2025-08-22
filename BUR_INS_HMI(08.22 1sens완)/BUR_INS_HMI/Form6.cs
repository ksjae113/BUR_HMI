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
    public partial class Form6 : Form
    {

        private MssqlLib mssqlLib = new MssqlLib();

        public Form6()
        {
            InitializeComponent();
        }



        private void btnRefresh_Click(object sender, EventArgs e)   //초기화 btn
        {
            LoadModbusData(); // 새로고침
            cbChannel.SelectedIndex = -1;    //콤보박스 출력지정값 초기화. 항목미지정으로.
            cbChannel.Text = "검색항목";
            dtStart.Value = DateTime.Today.AddDays(-1); // 기본 어제로 설정, 날짜 다시 세팅초기화
            dtEnd.Value = DateTime.Today;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            LoadModbusData();

            txtValue.Text = "검색값";
            txtValue.ForeColor = Color.Gray;

            txtValue.Enter += textBox1_Enter;
            txtValue.Leave += textBox1_Leave;

            string[] boardColumns = new string[]
                {
                    "판 식별번호","판 강종","판 폭","판 두께","판 길이",
                    "압연 종료 온도","가속 냉각 종료 온도","교정기 실적 Gap","지시 레벨링 속도"
                };


            // 콤보박스 채우기 (새로운 컬럼명들)
            cbChannel.Items.Clear();
            cbChannel.Items.AddRange(boardColumns);

            for (int i = 1; i <= 10; i++)
            {
                cbChannel.Items.Add($"RPM{i}-1");
                cbChannel.Items.Add($"RPM{i}-2");
                cbChannel.Items.Add($"RPM{i}-3");
                cbChannel.Items.Add($"RPM{i}-4");
                cbChannel.Items.Add($"TEMP{i}-1");
                cbChannel.Items.Add($"TEMP{i}-2");
                cbChannel.Items.Add($"AMP{i}");
                cbChannel.Items.Add($"PERIOD{i}");
            }

            dtStart.Value = DateTime.Today.AddDays(-1); // 기본 어제로 설정
            dtEnd.Value = DateTime.Today;

            // ID, LogTime 컬럼 너비 및 포맷
            if (dataGridView1.Columns.Contains("ID"))
                dataGridView1.Columns["ID"].Width = 60;
            if (dataGridView1.Columns.Contains("LogTime"))
            {
                dataGridView1.Columns["LogTime"].Width = 150;
                dataGridView1.Columns["LogTime"].DefaultCellStyle.Format = "yyyy-MM-dd  HH:mm:ss";
                dataGridView1.Columns["LogTime"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 각 데이터 컬럼 너비 설정


            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                // 가운데 정렬 (ID 제외)
                if (col.Name != "ID")
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 헤더 가운데 정렬
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Width 설정
                switch (col.Name)
                {
                    case "압연 종료 온도":
                    case "가속 냉각 종료 온도":
                        col.Width = 70;
                        break;

                    case "판 강종":
                    case "판 폭":
                    case "판 두께":
                    case "판 길이":
                        col.Width = 75;
                        break;

                    case "판 식별번호":
                        col.Width = 100;
                        break;

                    case "교정기 실적 Gap":
                    case "지시 레벨링 속도":
                        col.Width = 70;
                        break;

                    default:
                        // RPM/TEMP/AMP/PERIOD 등 나머지 컬럼
                        if (!boardColumns.Contains(col.Name) && col.Name != "ID" && col.Name != "LogTime")
                            col.Width = 70;
                        break;
                }
            }

            /* foreach (string col in cbChannel.Items)
             {
                 if (dataGridView1.Columns.Contains(col))
                 {
                     if (boardColumns.Contains(col))
                     {
                         // 판 정보 컬럼별로 Width 지정
                         switch (col)
                         {
                             case "판 식별번호":
                                 dataGridView1.Columns[col].Width = 100;
                                 break;

                             case "압연 종료 온도":
                             case "가속 냉각 종료 온도":
                                 dataGridView1.Columns[col].Width = 85;
                                 break;

                             case "판 강종":
                             case "판 폭":
                             case "판 두께":
                             case "판 길이":
                                 dataGridView1.Columns[col].Width = 80;
                                 break;

                             case "교정기 실적 Gap":
                             case "지시 레벨링 속도":
                                 dataGridView1.Columns[col].Width = 100;
                                 break;

                             default:
                                 dataGridView1.Columns[col].Width = 60; // 나머지 일반 데이터
                                 break;
                         }

                             dataGridView1.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                         //  글씨 센터 위치 하도록

                     }
                 }
             }*/
        }



        private void LoadModbusData()
        {
            try
            {
                DataSet ds = mssqlLib.GetModbusLog();
                dataGridView1.DataSource = ds.Tables[0];
                txtValue.Text = "검색값";
                txtValue.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 불러오기 오류: " + ex.Message);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime? start = null;
            DateTime? end = null;

            if (chkUseDate.Checked)
            {
                start = dtStart.Value.Date;
                end = dtEnd.Value.Date.AddDays(1).AddTicks(-1);
            }

            string column = cbChannel.SelectedItem?.ToString();
            string val = null;  // int? -> string으로 변경

            if (!string.IsNullOrWhiteSpace(txtValue.Text) && txtValue.Text != "검색값")
            {
                val = txtValue.Text.Trim();
            }

            try
            {
                DataSet ds = mssqlLib.GetModbusLogFiltered(start, end, column, val);
                dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("조회 실패: " + ex.Message);
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(txtValue.Text == "검색값")
            {
                txtValue.Text = "";
                txtValue.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtValue.Text))
            {
                txtValue.Text = "검색값";
                txtValue.ForeColor = Color.Gray;
            }
        }
    }
}

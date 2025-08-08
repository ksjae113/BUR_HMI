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

            // 콤보박스 채우기 (새로운 컬럼명들)
            cbChannel.Items.Clear();
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

            dataGridView1.Columns["ID"].Width = 60;
            dataGridView1.Columns["LogTime"].DefaultCellStyle.Format = "yyyy-MM-dd  HH:mm:ss";
            dataGridView1.Columns["LogTime"].Width = 150;  //날짜 150px로 늘려주기

            /*    for (int n = 1; n <= 36; n++)
                {
                    dataGridView1.Columns[$"value{n}"].Width = 55;
                }*/

            // 각 데이터 컬럼 너비 설정
            foreach (string col in cbChannel.Items)
            {
                if (dataGridView1.Columns.Contains(col))
                {
                    dataGridView1.Columns[col].Width = 60;
                }
            }
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

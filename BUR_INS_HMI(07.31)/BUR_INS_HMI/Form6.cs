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

            // 콤보박스 채우기
            for (int i = 1; i <= 36; i++)
                cbChannel.Items.Add($"value{i}");

            dtStart.Value = DateTime.Today.AddDays(-1); // 기본 어제로 설정
            dtEnd.Value = DateTime.Today;

            dataGridView1.Columns["ID"].Width = 60;
            dataGridView1.Columns["log_time"].Width = 150;  //날짜 150px로 늘려주기

            for (int n = 1; n <= 36; n++)
            {
                dataGridView1.Columns[$"value{n}"].Width = 55;
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
            int? val = null;

            if (!string.IsNullOrWhiteSpace(txtValue.Text))
            {
                if (int.TryParse(txtValue.Text.Trim(), out int parsed))
                    val = parsed;
                else
                {
                    MessageBox.Show("유효하지 않은 값입니다.");
                    return;
                }
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

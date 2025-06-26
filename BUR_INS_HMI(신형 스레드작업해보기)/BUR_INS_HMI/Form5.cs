using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BUR_INS_HMI
{


    public partial class Form5 : Form
    {

        public Form5()
        {
            InitializeComponent();

        }

       

        private void Form5_Load(object sender, EventArgs e)
        {
         //   LoadDataFromDB();
        }

        private void LoadDataFromDB()
        {
            string connectionString = "Server=MSSQLSERVER_22;Database=sjkwon;User Id=sa;Password=@aronxia113;";
            string query = "SELECT TOP 100 * FROM InputData ORDER BY Timestamp DESC;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                try
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("데이터 로딩 실패: " + ex.Message);
                }
            }
        }
    }

}

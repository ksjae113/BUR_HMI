using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUR_INS_HMI
{
    

    public partial class Form5 : Form
    {
        /// <summary>
        /// </summary>
        /// 
        public Form5()
        {
            InitializeComponent();
            /////
     

           // infoPanel.BringToFront();
            ///
            
       //     LoadDataFromDatabase();
        }

        

        private void LoadDataFromDatabase()
        {
            string connectionString = "Data Source=서버주소;Initial Catalog=데이터베이스 이름;Integrated Security=True";
            //또는 UserID, Password 포함

            string query = "SELECT * FROM YourTable";   //원하는 SQL 쿼리

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);

            //    dataGridView1.DataSource = table; // dataGridView1은 폼에 배치된 DataGridView
            }
        }

      
    }
}

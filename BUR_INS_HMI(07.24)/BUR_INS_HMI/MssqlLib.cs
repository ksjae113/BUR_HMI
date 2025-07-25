using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace BUR_INS_HMI
{
    internal class MssqlLib
    {

        private static readonly string connectionString = "Server=DESKTOP-4J780GL\\MSSQLSERVER_22;" +
            "Database=sampledb;Uid=sa;Pwd=0000;";
        //INSERT
        //    public void InsertDB(int id, string name)
        // Modbus 36개 값 삽입 함수
        public void InsertDB(int[] values)
        {
            if (values.Length != 36)
                throw new ArgumentException("36개의 값이 필요합니다.");

            // INSERT 쿼리 구성
            string sql = "INSERT INTO ModbusLog (" +
                         string.Join(",", Enumerable.Range(1, 36).Select(i => $"value{i}")) +
                         ") VALUES (" +
                         string.Join(",", Enumerable.Range(0, 36).Select(i => $"@val{i}")) +
                         ")";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    for (int i = 0; i < 36; i++)
                    {
                        cmd.Parameters.AddWithValue($"@val{i}", values[i]);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }


        // SELECT
        public DataSet GetUserInfo()
        {
            string sql = "SELECT * FROM UserInfo ORDER BY id DESC";
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Select 오류: " + ex.Message);
            }

            return ds;
        }

        public DataSet GetModbusLog()
        {
            string sql = "SELECT TOP 1000 * FROM ModbusLog ORDER BY log_time DESC";
            //최근 1000개만 정렬해서 가져오도록 하는 쿼리문
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(ds);
            }
            return ds;
        }

        public DataSet GetModbusLogFiltered(DateTime? start, DateTime? end, string columnName, int? value)
        {
            List<string> conditions = new List<string>();

            if (start.HasValue && end.HasValue)
                conditions.Add("log_time BETWEEN @start AND @end");

            if (!string.IsNullOrEmpty(columnName) && value.HasValue)
                conditions.Add($"{columnName} = @val");

            string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            string sql = $"SELECT TOP 1000 * FROM ModbusLog {whereClause} ORDER BY log_time DESC";

            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                if (start.HasValue && end.HasValue)
                {
                    cmd.Parameters.AddWithValue("@start", start.Value);
                    cmd.Parameters.AddWithValue("@end", end.Value);
                }

                if (!string.IsNullOrEmpty(columnName) && value.HasValue)
                {
                    cmd.Parameters.AddWithValue("@val", value.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }

            return ds;
        }

        // 접속테스트
        /* public bool ConnectionTest()
         {
             string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};",
                 "DESKTOP-4J780GL\\MSSQLSERVER_22",
  "sampledb", "sa", "0000");


             try
             {
                 using (SqlConnection conn = new SqlConnection(connectString))
                 {
                     conn.Open();
                 }
                 MessageBox.Show("SQL server connected");
                 return true;
             }
             catch (Exception)
             {
                 MessageBox.Show("SQL server connect failed");
                 return false;
             }
         }*/
    }
}

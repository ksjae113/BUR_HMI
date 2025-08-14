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

        //   private static readonly string connectionString = "Server=DESKTOP-4J780GL\\MSSQLSERVER_22;" +
        //      "Database=sampledb;Uid=sa;Pwd=0000;";   //작업용 
        private static readonly string connectionString = "Server=DESKTOP-2K8H0IE;" +
              "Database=sampledb;Uid=sa;Pwd=0000;";



        //INSERT
        //    public void InsertDB(int id, string name)
        // Modbus 36개 값 삽입 함수

        public void InsertDB(string[] values)
        {
            if (values.Length < 80)
                throw new ArgumentException("80개의 값이 필요합니다.");

            // 1. DB 컬럼명 리스트 만들기
            var columns = new List<string>();

            for (int i = 1; i <= 10; i++)   //컬럼 생성이 아닌, 이미 존재하는 컬럼 이름을 문자열로 만들어 INSERT문 구성
            {
                columns.Add($"[RPM{i}-1]");
                columns.Add($"[RPM{i}-2]");
                columns.Add($"[RPM{i}-3]");
                columns.Add($"[RPM{i}-4]");
                columns.Add($"[TEMP{i}-1]");
                columns.Add($"[TEMP{i}-2]");
                columns.Add($"[AMP{i}]");
                columns.Add($"[PERIOD{i}]");
            }

            // 2. SQL 파라미터 (@val0 ~ @val79) 만들기
            var parameters = Enumerable.Range(0, columns.Count).Select(i => $"@val{i}").ToList();

            // 3. INSERT 쿼리 구성
            string sql = $"INSERT INTO ModbusLog ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";

            // 4. DB 실행
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@val{i}", values[i] ?? (object)DBNull.Value);
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
            string sql = "SELECT TOP 1000 * FROM ModbusLog ORDER BY LogTime DESC";
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

        public DataSet GetModbusLogFiltered(DateTime? start, DateTime? end, string columnName, string value)
        {
            List<string> conditions = new List<string>();

          /*  List<string> validColumns = new List<string>()    //이게 더 안전 추후 이거로 
    {
        "RPM1-1", "RPM1-2", "RPM1-3", "RPM1-4",
        "TEMP1-1", "TEMP1-2",
        "AMP1", "PERIOD1",
        // ... 나머지 허용 컬럼들 모두 추가
    };*/

            if (start.HasValue && end.HasValue)
                conditions.Add("LogTime BETWEEN @start AND @end");

            if (!string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(value))
                conditions.Add($"[{columnName}] = @val");


            string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            string sql = $"SELECT TOP 1000 * FROM ModbusLog {whereClause} ORDER BY LogTime DESC";

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

                if (!string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(value))
                {
                    cmd.Parameters.AddWithValue("@val", value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }

            return ds;
        }


    

   
    }
}

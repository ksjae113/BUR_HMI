using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;


namespace sql_example
{
    internal class MssqlLib
    {
        // DB 접속 정보 설정  //연결은 성공
      /*  private static string DbSource = "DESKTOP-4J780GL\\MSSQLSERVER_22";// your DB address
        private static string DbName = "sampledb";// your DB database name
    private static string DbUser = "sa";// your DB user name
        private static string DbPassword = "0000";// your DB user password

    // DB 접속 문자열
    private static string connectionString;

        // DB 접속 설정을 위한 생성자
        static MssqlLib()
        {
            connectionString = $"Data Source={DbSource};" +
                                $"Initial Catalog={DbName};" +
                                $"User ID={DbUser};" +
                                $"Password={DbPassword};";
        }

        // 데이터베이스 연동 함수
        public void connect()
        {
            // 데이터베이스 연결
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // DB 서버 접속
                    connection.Open();
                    MessageBox.Show("Connection successful.");
                }
                catch (Exception ex) // DB 서버 접속 실패 시
                {
                    MessageBox.Show("Error connecting to database: " + ex.Message);
                }

                // DB 서버 접속 종료
                connection.Close();
            }
        }*/

        
        // 접속테스트
        public bool ConnectionTest()
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //데이터조회
        public void SelectDB()
        {
            string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};", 
                "DESKTOP-4J780GL\\MSSQLSERVER_22",
 "sampledb", "sa", "0000");
            string sql = "select * from UserInfo ORDER BY DESC";

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Close();
            }
        }

        //INSERT처리
        public void InsertDB(int id, string name)
        {
            string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};", 
                "DESKTOP-4J780GL\\MSSQLSERVER_22",
"sampledb", "sa", "0000");
            string sql = $"Insert Into UserInfo  (id,name) values ({id},'{name}')";

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        //데이터조회
        public DataSet GetUserInfo()
        {
            string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};",
                "DESKTOP-4J780GL\\MSSQLSERVER_22",
 "sampledb", "sa", "0000");
            string sql = "select * from [UserInfo]";
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(ds);
            }
            return ds;
        }
    }
   }


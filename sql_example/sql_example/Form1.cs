using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;

namespace sql_example
{
    public partial class Form1 : Form
    {
        MssqlLib mMssqlLib = new MssqlLib();

        public Form1()
        {
            InitializeComponent();
        //    mMssqlLib.connect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void Search()
        {
            DataSet ds = mMssqlLib.GetUserInfo();
            dataGridView1.DataSource = ds.Tables[0];
        }


        private void button1_Click_1(object sender, EventArgs e)    //조회
        {
            Search();   
        }

        private void button2_Click_1(object sender, EventArgs e)    //추가
        {
            int id = Int32.Parse(txtID.Text);
            string name = txtName.Text.Trim();
            mMssqlLib.InsertDB(id, name);
            Search();
        }
    }
}

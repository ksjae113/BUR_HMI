using ScottPlot;
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
        List<List<List<double>>> ys = new();  // [plot][line][values]
        List<List<List<double>>> xs = new();  // [plot][line][indices]
        List<FormsPlot> plots = new();
        int sampleCounter = 0;
        bool isMeasuring = false;
        private int[] layout = { 3, 2, 3, 3, 2, 3, 3, 2, 3, 3 };


        public Form5()
        {
            InitializeComponent();
            InitializeGraphs();


            //     LoadDataFromDatabase();
        }

        private void InitializeGraphs()
        {

            ys.Clear();
            xs.Clear();
            plots.Clear();



            for (int i = 0; i < 10; i++)
            {
                ys.Add(new List<List<double>>());
                xs.Add(new List<List<double>>());

                var fp = new FormsPlot();
                fp.Height = 100;
                fp.Dock = DockStyle.Top;
                fp.Margin = new Padding(2);
                fp.Plot.SetAxisLimits(yMin: 0, yMax: 25);
                fp.Plot.Title($"Line {i + 1}", size: 12);
                fp.Plot.Layout(left: 20, right: 5, top: 5, bottom: 20);
                fp.Plot.XAxis.TickLabelStyle(fontSize: 10);
                fp.Plot.YAxis.TickLabelStyle(fontSize: 10);

                for (int j = 0; j < layout[i]; j++)
                {
                    ys[i].Add(new List<double>());
                    xs[i].Add(new List<double>());
                }

                plots.Add(fp);
            }
        }

        private void OnNewData(ushort[] data)
        {
            if (data == null || data.Length < 27) return;

            if (data[0] == 1 || data[1] == 1)  // 측정 중일 때
            {
                if (!isMeasuring)
                {
                    isMeasuring = true;
                    sampleCounter = 0;

                    foreach (var group in ys)
                        foreach (var line in group)
                            line.Clear();

                    foreach (var group in xs)
                        foreach (var line in group)
                            line.Clear();
                }

                int dataIndex = 0;
                for (int i = 0; i < layout.Length; i++)
                {
                    for (int j = 0; j < layout[i]; j++)
                    {
                        if (dataIndex >= data.Length) break;

                        ys[i][j].Add(data[dataIndex]);
                        xs[i][j].Add(sampleCounter);
                        dataIndex++;
                    }
                }
                sampleCounter++;

                UpdateAllPlots();
            }
            else if (data[0] == 0 && isMeasuring)
            {
                isMeasuring = false;
                // 측정 종료 시 추가 처리 가능
            }
            /*  private void LoadDataFromDatabase()
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
              }*/


        }

        private void UpdateAllPlots()
        {
            for (int i = 0; i < plots.Count; i++)
            {
                var plot = plots[i];
                plot.Plot.Clear();

                for (int j = 0; j < ys[i].Count; j++)
                {
                    var x = xs[i][j].ToArray();
                    var y = ys[i][j].ToArray();
                    var color = GetColor(j);

                    plot.Plot.AddScatter(x, y, color: color, markerSize: 0, label: $"L{j + 1}");
                }

                plot.Plot.Title($"Line {i + 1}");
                //   plot.Plot.XLabel("Time (s)");
                //   plot.Plot.YLabel("RPM");
                plot.Plot.SetAxisLimits(xMin: 0, yMin: 0, yMax: 25);
                plot.Plot.XAxis.ManualTickSpacing(1);  // X축 눈금 간격 1초

                //   plot.Plot.Legend.IsVisible = true;
                //   plot.Plot.Legend.Location = ScottPlot.Alignment.UpperRight;

                plot.Render();
            }
        }

        private Color GetColor(int index)
        {
            Color[] colors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple };
            return colors[index % colors.Length];
        }
    }
}

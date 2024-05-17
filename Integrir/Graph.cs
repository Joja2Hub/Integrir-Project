using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Integrir
{
    public partial class Graph : Form
    {

        public NpgsqlConnection con;
        public System.Data.DataTable dtContract = new System.Data.DataTable();
        public Graph(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
            LoadComponent();
        }


        void LoadComponent()
        {
            List<String> productsNames = new List<String>();
            string sql = @"
            SELECT name 
            FROM Товары";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string productsName = reader.GetString(0);
                productsNames.Add(productsName);
            }
            reader.Close();
            string[] productsNamesArr = productsNames.ToArray();
            checkedListBox1.Items.AddRange(productsNamesArr);
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            DateTime reportFrom = dateTimePicker1.Value;
            DateTime reportTo = dateTimePicker1.Value;
            List<string> clientsNamesChecked = new List<string>();
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                clientsNamesChecked.Add(checkedListBox1.CheckedItems[i].ToString());
            }

            System.Data.DataTable dtReport = new System.Data.DataTable();
            DataSet dsReport = new DataSet();
            dtContract.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                string selectedProduct = checkedListBox1.CheckedItems[i].ToString();

                string sql = @"
        SELECT shipment_status, sum(quantity)
        FROM Договор_товары d
            JOIN Товары t ON d.product_id = t.id
            JOIN Договор c ON d.contract_id = c.id
        WHERE t.name = :productName
        GROUP BY shipment_status
        ORDER BY shipment_status";

                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                command.Parameters.AddWithValue("productName", selectedProduct);

                NpgsqlDataReader reader = command.ExecuteReader();

                Series series = new Series
                {
                    Name = selectedProduct,
                    IsVisibleInLegend = true,
                    ChartType = SeriesChartType.Column
                };

                while (reader.Read())
                {
                    string shipmentStatus = reader.GetString(0);
                    int quantity = reader.GetInt32(1);
                    series.Points.AddXY(shipmentStatus, quantity);
                }

                chart1.Series.Add(series);
                reader.Close();
            }

            // Настраиваем параметры диаграммы
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
        }
    }
}

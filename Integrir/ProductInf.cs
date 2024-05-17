using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integrir
{
    public partial class ProductInf : Form
    {
        public NpgsqlConnection con;
        public DataTable dt = new DataTable();
        public DataSet ds = new DataSet();
        public ProductInf(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
        }

        private void ProductInf_Load(object sender, EventArgs e)
        {
            Update();
        }

        void Update()
        {
            dt.Clear();
            string sql = @"SELECT * FROM Товары";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "ID Товара";
            dataGridView1.Columns[1].HeaderText = "Наименование";
            dataGridView1.Columns[2].HeaderText = "Цена";
            dataGridView1.Sort(dataGridView1.Columns["id"], ListSortDirection.Ascending);
            dataGridView1.Refresh();
            //делаем ширину колонок подходящей под содержимое
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            string name = (string)dataGridView1.CurrentRow.Cells["name"].Value.ToString();
            decimal productPrice = (decimal)dataGridView1.CurrentRow.Cells["price"].Value;
            AddProduct productForm = new AddProduct(con, id, name, productPrice);
            productForm.ShowDialog();
            Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            string name = (string)dataGridView1.CurrentRow.Cells["name"].Value.ToString();
            DialogResult result = MessageBox.Show("Точно удалить продкут " + name + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string sql = @" DELETE FROM  Товары WHERE id = :id";
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
                Update();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddProduct addProduct = new AddProduct(con);
            addProduct.ShowDialog();
            Update();
        }
    }
}

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
    public partial class Clients : Form
    {
        public NpgsqlConnection con;
        public DataTable dt = new DataTable();
        public DataSet ds = new DataSet();
        public Clients(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
        }

        private void Update()
        {
            dt.Clear();
            string call = @"SELECT * FROM Клиенты";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(call, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "Идентификатор клиенты";
            dataGridView1.Columns[1].HeaderText = "ФИО";
            dataGridView1.Columns[2].HeaderText = "Номер телефона";
            dataGridView1.Sort(dataGridView1.Columns["client_id"], ListSortDirection.Ascending);
            dataGridView1.Refresh();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void Clients_Load(object sender, EventArgs e)
        {
            Update();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddClient addClient = new AddClient(con);
            addClient.ShowDialog();
            Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int client_id = (int)dataGridView1.CurrentRow.Cells["client_id"].Value;
            string clientName = (string)dataGridView1.CurrentRow.Cells["client_name"].Value.ToString();
            string clientPhone = (string)dataGridView1.CurrentRow.Cells["phone"].Value.ToString();
            AddClient addClient = new AddClient(con, client_id, clientName, clientPhone);
            addClient.ShowDialog();
            Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int client_id = (int)dataGridView1.CurrentRow.Cells["client_id"].Value;
            string clientName = (string)dataGridView1.CurrentRow.Cells["client_name"].Value.ToString();
            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись " + clientName + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string call = @"DELETE FROM Клиенты WHERE client_id = :client_id";
                NpgsqlCommand command = new NpgsqlCommand(call, con);
                command.Parameters.AddWithValue("client_id", client_id);
                command.ExecuteNonQuery();
                Update();
            }
        }
    }
}

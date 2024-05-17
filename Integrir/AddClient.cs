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
    public partial class AddClient : Form
    {
        public NpgsqlConnection con;
        public int client_id = -1;
        public string client_name;
        public string phone;
        public AddClient(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
        }

        public AddClient(NpgsqlConnection con, int client_id, string client_name, string phone)
        {
            InitializeComponent();
            this.con = con;
            this.client_id = client_id;
            this.client_name = client_name;
            this.phone = phone;
            textBox1.Text = client_name;
            textBox2.Text = phone;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.client_id == -1)
            {
                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    DialogResult result = MessageBox.Show("Добавить новые данные?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string call = @" INSERT INTO Клиенты (client_name, phone) VALUES (:client_name,  :phone)";
                        NpgsqlCommand command = new NpgsqlCommand(call, con);
                        command.Parameters.AddWithValue("client_name", textBox1.Text);
                        command.Parameters.AddWithValue("phone", textBox2.Text);
                        command.ExecuteNonQuery();
                        Close();
                    }
                }
                else
                    MessageBox.Show("Ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //для изменения
            else
            {
                DialogResult result = MessageBox.Show("Вы действительно хотите изменить данные " + this.client_name + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string call = @" UPDATE  Клиенты SET client_name = :client_name, phone = :phone WHERE client_id = :client_id";
                    NpgsqlCommand command = new NpgsqlCommand(call, con);
                    command.Parameters.AddWithValue("client_id", this.client_id);
                    command.Parameters.AddWithValue("client_name", textBox1.Text);
                    command.Parameters.AddWithValue("phone", textBox2.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
            }
        }
    }
}

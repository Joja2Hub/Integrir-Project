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
    public partial class AddProduct : Form
    {

        public NpgsqlConnection conn;
        public int id = -1;
        public string name;
        public decimal price;
        public AddProduct(NpgsqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
        }

        public AddProduct(NpgsqlConnection conn, int id, string name, decimal price)
        {
            InitializeComponent();
            this.conn = conn;
            this.id = id;
            this.name = name;
            this.price = price;
            maskedTextBox1.Text = name;
            textBox1.Text = price.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.id == -1)
            {
                decimal res;
                if (maskedTextBox1.Text != "" && decimal.TryParse(textBox1.Text, out res))
                {
                    DialogResult result = MessageBox.Show("Добавить новые данные?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string sql = @" INSERT INTO Товары (name, price) VALUES (:name,  :price)";
                        NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                        command.Parameters.AddWithValue("name", maskedTextBox1.Text);
                        command.Parameters.AddWithValue("price", decimal.Parse(textBox1.Text));
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
                DialogResult result = MessageBox.Show("Изменить? " + this.id + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string sql = @" UPDATE Товары SET name = :name, price = :price WHERE id = :id";
                    NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                    command.Parameters.AddWithValue("id", this.id);
                    command.Parameters.AddWithValue("name", maskedTextBox1.Text);
                    command.Parameters.AddWithValue("price", decimal.Parse(textBox1.Text));
                    command.ExecuteNonQuery();
                    Close();
                }
            }
        }
    }
}

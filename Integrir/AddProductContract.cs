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
    public partial class AddProductContract : Form
    {

        public NpgsqlConnection con;
        public int contractProductId = -1;
        public string contrID;
        public List<String> contractsIDs;
        public string productName;
        public List<String> productsNames;
        public int productQuantity;
        public AddProductContract(NpgsqlConnection con, List<String> contractsIDs, List<String> productsNames)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.con = con;
            this.contractsIDs = contractsIDs;
            this.comboBox1.DataSource = contractsIDs;
            this.productsNames = productsNames;
            this.comboBox2.DataSource = productsNames;
        }

        public AddProductContract(NpgsqlConnection con, int contractProductId, string contrID, List<String> contractsIDs, string productName, List<String> productsNames, int productQuantity)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.con = con;
            this.contractProductId = contractProductId;
            this.contrID = contrID;
            this.productName = productName;
            this.productQuantity = productQuantity;
            this.contractsIDs = contractsIDs;
            this.productsNames = productsNames;
            this.comboBox1.DataSource = contractsIDs;
            this.comboBox2.DataSource = productsNames;
            this.textBox1.Text = productQuantity.ToString();
        }



        // Кнопки
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.contractProductId == -1)
            {
                int res;
                if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null && int.TryParse(textBox1.Text, out res))
                {
                    DialogResult result = MessageBox.Show("Добавить новые данные?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {

                        int contractId = Convert.ToInt32(comboBox1.SelectedItem.ToString());


                        string sql = @"SELECT id FROM Товары WHERE name = :name";
                        NpgsqlCommand command = new NpgsqlCommand(sql, con);
                        command.Parameters.AddWithValue("name", comboBox2.SelectedItem.ToString());
                        object productIdObj = command.ExecuteScalar();
                        int productId = Convert.ToInt32(productIdObj);

                        sql = @"
                        INSERT 
                        INTO Договор_товары (contract_id, product_id, quantity, local_total_sum) 
                        VALUES (:contract_id, :product_id, :quantity, :local_total_sum)";
                        command = new NpgsqlCommand(sql, con);
                        command.Parameters.AddWithValue("contract_id", contractId);
                        command.Parameters.AddWithValue("product_id", productId);
                        command.Parameters.AddWithValue("local_total_sum", 0);
                        command.Parameters.AddWithValue("quantity", int.Parse(this.textBox1.Text));
                        command.ExecuteNonQuery();
                        Close();
                    }
                }
                else
                    MessageBox.Show("Не удалось добавить запись!\nУбедитесь, что вы ввели все данные корректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //для изменения
            else
            {
                DialogResult result = MessageBox.Show("Изменить записи с номером " + this.contractProductId + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {

                    int contractId = Convert.ToInt32(comboBox1.SelectedItem.ToString());

                    string sql = @" SELECT id FROM Товары WHERE name = :name";
                    NpgsqlCommand command = new NpgsqlCommand(sql, con);
                    command.Parameters.AddWithValue("name", comboBox2.SelectedItem.ToString());
                    object productIdObj = command.ExecuteScalar();
                    int productId = Convert.ToInt32(productIdObj);

                    sql = @"
                    UPDATE Договор_товары 
                    SET contract_id = :contract_id, product_id = :product_id, quantity = :quantity 
                    WHERE id = :id";
                    command = new NpgsqlCommand(sql, con);
                    command.Parameters.AddWithValue("id", this.contractProductId);
                    command.Parameters.AddWithValue("contract_id", contractId);
                    command.Parameters.AddWithValue("product_id", productId);
                    command.Parameters.AddWithValue("quantity", int.Parse(this.textBox1.Text));
                    command.ExecuteNonQuery();
                    Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddProductContract_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedItem = contrID;
            this.comboBox2.SelectedItem = productName;
        }
    }
}

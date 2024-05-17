using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integrir
{
    public partial class AddContract : Form
    {
        public NpgsqlConnection con;
        public int contractId = -1;
        public DateTime signedDate;
        public string clientName;
        public List<string> clientsNames;
        public List<string> payment_statuses = new List<string> { "оплачен", "не оплачен", "внесена предоплата" };
        public List<string> paymentTypes = new List<string> { "наличка", "картой" };
        public string payment_status;
        public string payment_type;

        public AddContract(NpgsqlConnection con, List<string> clientsNames)
        {
            InitializeComponent();
            this.con = con;
            this.clientsNames = clientsNames;
            this.comboBox1.DataSource = clientsNames;
            this.comboBox3.DataSource = paymentTypes;
            this.comboBox2.DataSource = payment_statuses;
        }

        public AddContract(NpgsqlConnection con, int contractId, DateTime signedDate, string clientName, List<string> clientsNames, string payment_status, string payment_type)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.con = con;
            this.contractId = contractId;
            this.signedDate = signedDate;
            this.dateTimePicker1.Value = signedDate;
            this.clientName = clientName;
            this.comboBox1.DataSource = clientsNames;
            this.comboBox3.DataSource = paymentTypes;
            this.comboBox2.DataSource = payment_statuses;
            this.payment_status = payment_status;
            this.payment_type = payment_type;
        }

        private void AddContract_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedItem = clientName;
            this.comboBox2.SelectedItem = payment_status;
            this.comboBox3.SelectedItem = payment_type;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.contractId == -1)
            {
                if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null && comboBox3.SelectedItem != null)
                {
                    DialogResult result = MessageBox.Show("Добавить новые данные?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string sql = @" SELECT client_id FROM Клиенты WHERE client_name = :client_name";
                        NpgsqlCommand command = new NpgsqlCommand(sql, con);
                        command.Parameters.AddWithValue("client_name", comboBox1.SelectedItem.ToString());
                        object clientIdObj = command.ExecuteScalar();
                        int clientId = Convert.ToInt32(clientIdObj);

                        sql = @" INSERT INTO Договор (client_id, signed_date, payment_type, payment_status, total_sum) VALUES (:client_id, :signed_date, :payment_type, :payment_status, :total_sum)";
                        command = new NpgsqlCommand(sql, con);
                        command.Parameters.AddWithValue("client_id", clientId);
                        command.Parameters.AddWithValue("signed_date", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("total_sum", 0.00);
                        command.Parameters.AddWithValue("payment_type", comboBox3.SelectedItem.ToString());
                        command.Parameters.AddWithValue("payment_status", comboBox2.SelectedItem.ToString());
                        command.ExecuteNonQuery();
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось добавить запись!\nУбедитесь, что вы ввели все данные корректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            //для изменения
            else
            {
                DialogResult result = MessageBox.Show("Вы действительно хотите изменить данные в записи с номером " + this.contractId + "?", "Подтверждение добавления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string sql = @"SELECT client_id FROM Клиенты WHERE client_name = :client_name";
                    NpgsqlCommand command = new NpgsqlCommand(sql, con);
                    command.Parameters.AddWithValue("client_name", comboBox1.SelectedItem.ToString());
                    object clientIdObj = command.ExecuteScalar();
                    int clientId = Convert.ToInt32(clientIdObj);

                    sql = @"UPDATE Договор SET client_id = :client_id, signed_date = :signed_date, payment_type = :payment_type, payment_status = :payment_status WHERE id = :contract_id";
                    command = new NpgsqlCommand(sql, con);
                    command.Parameters.AddWithValue("contract_id", this.contractId);
                    command.Parameters.AddWithValue("client_id", clientId);
                    command.Parameters.AddWithValue("signed_date", dateTimePicker1.Value);
                    command.Parameters.AddWithValue("payment_type", comboBox3.SelectedItem.ToString());
                    command.Parameters.AddWithValue("payment_status", comboBox2.SelectedItem.ToString());
                    command.ExecuteNonQuery();
                    Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
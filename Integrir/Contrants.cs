using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.Office.Interop.Excel;

namespace Integrir
{
    
    public partial class Contrants : Form
    {

        public System.Data.DataTable dtContract = new System.Data.DataTable();
        public DataSet dsContract = new DataSet();
        public System.Data.DataTable dtContractProduct = new System.Data.DataTable();
        public DataSet dsContractProduct = new DataSet();
        public NpgsqlConnection con;
        public Contrants(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
            LoadCheckBox();
            
        }


        void LoadCheckBox()
        {
            List<String> clientsNames = new List<String>();
            string sql = @"
            SELECT client_name 
            FROM Клиенты";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string clientName = reader.GetString(0);
                clientsNames.Add(clientName);
            }
            reader.Close();
            string[] clientsNamesArr = clientsNames.ToArray();
            checkedListBox1.Items.AddRange(clientsNamesArr);
        }

        private void Contrants_Load(object sender, EventArgs e)
        {
            UpdateContr();
            UpdateCProduct();
        }

        void UpdateContr()
        {
            dtContract.Clear();
            string call = @" SELECT * FROM Договор";
            NpgsqlDataAdapter daProductsInvoices = new NpgsqlDataAdapter(call, con);
            dsContract.Reset();
            daProductsInvoices.Fill(dsContract);
            dtContract = dsContract.Tables[0];
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dtContract;
            dataGridView1.Columns[0].HeaderText = "ID договора";
            dataGridView1.Columns[1].HeaderText = "ID клиента";
            dataGridView1.Columns[2].HeaderText = "Дата подписания";
            dataGridView1.Columns[3].HeaderText = "Общая сумма";
            dataGridView1.Columns[4].HeaderText = "Тип оплаты";
            dataGridView1.Columns[5].HeaderText = "Статус оплаты";
            dataGridView1.Columns[6].HeaderText = "Статус отгрузки";
            dataGridView1.Sort(dataGridView1.Columns["id"], ListSortDirection.Ascending);
            dataGridView1.Refresh();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        void UpdateCProduct()
        {
            dtContractProduct.Clear();
            string call = @"
            SELECT cp.id, cp.contract_id, cp.product_id, cp.quantity, cp.local_total_sum
            FROM Договор_товары cp
            JOIN Товары p ON cp.product_id = p.id";
            NpgsqlDataAdapter daBillOfLanding = new NpgsqlDataAdapter(call, con);
            dsContractProduct.Reset();
            daBillOfLanding.Fill(dsContractProduct);
            dtContractProduct = dsContractProduct.Tables[0];
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = dtContractProduct;
            dataGridView2.Columns[0].HeaderText = "ID записи";
            dataGridView2.Columns[1].HeaderText = "ID договора";
            dataGridView2.Columns[2].HeaderText = "ID продукта";
            dataGridView2.Columns[3].HeaderText = "Количество";
            dataGridView2.Columns[4].HeaderText = "Общая сумма";
            dataGridView2.Sort(dataGridView2.Columns["id"], ListSortDirection.Ascending);
            dataGridView2.Refresh();

            //делаем ширину колонок подходящей под содержимое
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        //Кнопки контрактов
        private void button1_Click(object sender, EventArgs e)
        {
            List<String> clientsNames = new List<String>();
            string sql = @"
            SELECT client_name 
            FROM Клиенты";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string clientName = reader.GetString(0);
                clientsNames.Add(clientName);
            }
            reader.Close();

            AddContract contractForm = new AddContract(con, clientsNames);
            contractForm.ShowDialog();
            UpdateContr();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            int clientId = (int)dataGridView1.CurrentRow.Cells["client_id"].Value;
            DateTime signedDate;
            try
            {
                signedDate = (DateTime)dataGridView1.CurrentRow.Cells["signed_data"].Value;
            }
            catch (Exception ex)
            {
                signedDate = DateTime.Today;
            }
            string payment_status = (string)dataGridView1.CurrentRow.Cells["payment_status"].Value;
            string payment_type = (string)dataGridView1.CurrentRow.Cells["payment_type"].Value;


            string sql = @"
            SELECT client_name 
            FROM Клиенты 
            WHERE client_id = :client_id";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("client_id", clientId);
            object clientNameObj = command.ExecuteScalar();
            string clientName = Convert.ToString(clientNameObj);


            List<String> clientsNames = new List<String>();
            sql = @"
            SELECT client_name 
            FROM Клиенты";
            command = new NpgsqlCommand(sql, con);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string tmpClientName = reader.GetString(0);
                clientsNames.Add(tmpClientName);
            }
            reader.Close();


            AddContract contractForm = new AddContract(con, id, signedDate, clientName, clientsNames, payment_status, payment_type);
            contractForm.ShowDialog();
            UpdateContr();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись с номером " + id + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string sql = @" DELETE FROM Договор WHERE id = :id";
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
                UpdateContr();
            }
        }

        //Кнопки товаров в договорах
        private void button6_Click(object sender, EventArgs e)
        {
            List<String> contractsCodes = new List<String>();
            string sql = @"
            SELECT id 
            FROM Договор";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int contractCode = reader.GetInt32(0);
                contractsCodes.Add(Convert.ToString(contractCode));
            }
            reader.Close();

            List<String> productsNames = new List<String>();
            sql = @"
            SELECT name 
            FROM Товары ";
            command = new NpgsqlCommand(sql, con);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                string productName = reader.GetString(0);
                productsNames.Add(productName);
            }
            reader.Close();

            AddProductContract contractProductsForm = new AddProductContract(con, contractsCodes, productsNames);
            contractProductsForm.ShowDialog();
            UpdateContr();
            UpdateCProduct();
        }


        //Функции обноваления данны таблиц
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            clickUpdate();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            clickUpdate();
        }


        void clickUpdate()
        {
            int contractId = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            dtContractProduct.Clear();
            string sql = @"
            SELECT * 
            FROM  Договор_товары
            WHERE contract_id = " + contractId;
            NpgsqlDataAdapter dtAlterContractProduct = new NpgsqlDataAdapter(sql, con);
            dsContractProduct.Reset();
            dtAlterContractProduct.Fill(dsContractProduct);
            dtContractProduct = dsContractProduct.Tables[0];
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = dtContractProduct;
            dataGridView2.Columns[0].HeaderText = "ID записи";
            dataGridView2.Columns[1].HeaderText = "ID договора";
            dataGridView2.Columns[2].HeaderText = "ID продукта";
            dataGridView2.Columns[3].HeaderText = "Количество";
            dataGridView2.Columns[4].HeaderText = "Стоимость";
            dataGridView2.Sort(dataGridView2.Columns["id"], ListSortDirection.Ascending);
            dataGridView2.Refresh();
        }

        //Удаление и изменение товара в контракте.
        private void button5_Click(object sender, EventArgs e)
        {
            int contractProductId = (int)dataGridView2.CurrentRow.Cells["id"].Value;
            int contractId = (int)dataGridView2.CurrentRow.Cells["contract_id"].Value;
            int productId = (int)dataGridView2.CurrentRow.Cells["product_id"].Value;
            int productQuantity = (int)dataGridView2.CurrentRow.Cells["quantity"].Value;

            string contractCode = Convert.ToString(contractId);

            string sql = @"
            SELECT name 
            FROM Товары 
            WHERE id = :id";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("id", productId);
            object productIdObj = command.ExecuteScalar();
            string productName = Convert.ToString(productIdObj);

            List<String> contractsCodes = new List<String>();
            sql = @"
            SELECT id 
            FROM Договор";
            command = new NpgsqlCommand(sql, con);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int tmpContractCode = reader.GetInt32(0);
                contractsCodes.Add(Convert.ToString(tmpContractCode));
            }
            reader.Close();

            List<String> productsNames = new List<String>();
            sql = @"
            SELECT name 
            FROM Товары";
            command = new NpgsqlCommand(sql, con);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                string tmpProductName = reader.GetString(0);
                productsNames.Add(tmpProductName);
            }
            reader.Close();

            AddProductContract contractProductsForm = new AddProductContract(con, contractProductId, contractCode, contractsCodes, productName, productsNames, productQuantity);
            contractProductsForm.ShowDialog();
            UpdateCProduct();
            UpdateContr();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            DialogResult result = MessageBox.Show("Удалить запись с номером " + id + "?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string sql = @"
                DELETE FROM Договор_товары 
                WHERE id = :id";
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
                UpdateContr();
                UpdateCProduct();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            clickUpdate();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            clickUpdate();
        }

        //Отчет
        private void button7_Click(object sender, EventArgs e)
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

            // SQL запрос для получения данных о договорах и продуктах
            string sql = @"
    SELECT 
        cl.client_name,
        t.name AS product_name,
        dt.quantity,
        dt.local_total_sum
    FROM 
        public.""Клиенты"" cl
    JOIN 
        public.""Договор"" c ON cl.client_id = c.client_id
    JOIN 
        public.""Договор_товары"" dt ON c.id = dt.contract_id
    JOIN 
        public.""Товары"" t ON dt.product_id = t.id
    WHERE 
        c.shipment_status = 'готовится к отправке'
    ORDER BY 
        cl.client_name";

            NpgsqlCommand command = new NpgsqlCommand(sql, con);

            // Адаптер для заполнения DataSet
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(dsReport);

            dtReport = dsReport.Tables[0];

            // Создание объекта Excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;
            Workbook wb = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            // Заголовки столбцов
            List<string> headers = new List<string> { "Имя клиента", "Наименование товара", "Количество", "Сумма" };
            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cells[1, i + 1] = headers[i];
            }

            // Заполнение данных
            try
            {
                for (int row = 0; row < dtReport.Rows.Count; row++)
                {
                    DataRow dr = dtReport.Rows[row];
                    for (int col = 0; col < dtReport.Columns.Count; col++)
                    {
                        ws.Cells[row + 2, col + 1] = dr[col].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при экспорте данных: " + ex.Message);
            }


            // Сохранение файла Excel
            wb.SaveAs(@"C:\Users\79182\Desktop\Инстиутут\3ий Курс\2\Интегральные среды\Integrir\Report.xlsx", XlFileFormat.xlWorkbookDefault);
            wb.Close();
            excelApp.Quit();
            UpdateContr();
            UpdateCProduct();
        }
        





        //График
        private void button8_Click(object sender, EventArgs e)
        {
            Graph contractForm = new Graph(con);
            contractForm.ShowDialog();
        }
    }
}

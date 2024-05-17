using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Integrir
{
    public partial class Form1 : Form
    {

        public NpgsqlConnection con;

        public Clients client;
        public ProductInf products;
        public Contrants contrants;

        public static Form1 SelfRef {  get; set; }

        public Form1()
        {
            SelfRef = this;
            InitializeComponent();
            Connection();
            UserConrolInit();
        }

        void UserConrolInit()
        {
            client = new Clients(con);
            products = new ProductInf(con);
            contrants = new Contrants(con);

            client.Visible = false;
            products.Visible = false;
            contrants.Visible = false;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                products.Visible = false;
                contrants.Visible = false;
                client.Visible = true;
            }
            catch (Exception ex)
            {
                UserConrolInit();
                products.Visible = false;
                contrants.Visible = false;
                client.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                products.Visible = true;
                contrants.Visible = false;
                client.Visible = false;
            }
            catch (Exception ex)
            {

                UserConrolInit();
                products.Visible = true;
                contrants.Visible = false;
                client.Visible = false;
            }
        }

        public void Connection()
        {
            string connection = @"Server = localhost; Port = 5432; UserId = postgres; password = 112; database = Erushev_C#DataBase";
            con = new NpgsqlConnection(connection);
            con.Open();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                products.Visible = false;
                contrants.Visible = true;
                client.Visible = false;
            }
            catch (Exception ex)
            {
                UserConrolInit();
                products.Visible = false;
                contrants.Visible = true;
                client.Visible = false;
            }
        }
    }
}

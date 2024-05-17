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
    public partial class Products : Form
    {
        int id;
        string name;
        int price;
        public NpgsqlConnection con;
        public Products(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
        }



        public Products(NpgsqlConnection con, int id, string name, int price)
        {
            InitializeComponent();
            this.con = con;
            this.id = id;
            this.name = name;
            this.price = price;
            textBoxName.Text = name;
            textBoxPrice.Text = price.ToString();
        }























        private void Products_Load(object sender, EventArgs e)
        {

        }
    }
}

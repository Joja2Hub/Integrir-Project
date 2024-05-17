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
    public partial class ProductsInf : Form
    {

        public NpgsqlConnection con;
        public DataTable dt = new DataTable();
        public DataSet ds = new DataSet();
        public ProductsInf(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
        }

        private void ProductsInf_Load(object sender, EventArgs e)
        {
            Update();
        }

        void Update()
        {

        }
    }
}

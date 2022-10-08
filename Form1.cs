using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace OrderNotify
{
    public partial class Form1 : Form
    {
        private IList<Order> _orders;
        private readonly string _connectionString = "data source=DESKTOP-M4TEFJN\\SQLEXPRESS;Database=ProzDB; integrated security=true";
        private readonly SqlTableDependency<Order> sqlordertracker;
        private int userstoreid;
        public Form1()
        {
            InitializeComponent();
            var ordermapper = new ModelToTableMapper<Order>();
            ordermapper.AddMapping(model => model.StoreID, "StoreID");
            sqlordertracker = new SqlTableDependency<Order>(_connectionString, "Orders",mapper: ordermapper);
            sqlordertracker.OnChanged += sqlordertracker_OnChanged;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GetOrderData();
            sqlordertracker.Start(); //main function
            button1.Text = "Connected";
            button1.Enabled = false;
            userstoreid = Int32.Parse(textBox1.Text);
            textBox1.Enabled = false;
        }

        private void sqlordertracker_OnChanged(object sender, RecordChangedEventArgs<Order> e)
        {
           // if (_orders != null)
            //{
                if (e.ChangeType != ChangeType.None)
                {
                    switch (e.ChangeType)
                    {
                        case ChangeType.Delete:
                            //_orders.Remove(_orders.FirstOrDefault(c => c.StoreID == e.Entity.StoreID));
                            break;
                        case ChangeType.Insert:
                        {
                            if(e.Entity.StoreID == userstoreid) 
                            MessageBox.Show("There is a new Order request"); //_orders.Add(e.Entity);
                        }                            
                        break;                                                    
                    }                    
                }
           // }
        }

        //optional method
        private IEnumerable<Order> GetOrderData()
        {
            _orders = new List<Order>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM [Orders]";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var storeid = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("StoreID"));
                            var ordername = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Ordername"));
                            var orderaddress = sqlDataReader.GetString(sqlDataReader.GetOrdinal("OrderAddress"));

                            _orders.Add(new Order { StoreID = storeid, OrderName = ordername, OrderAddress = orderaddress });
                        }
                    }
                }
            }

            return _orders;
        }
    }
}

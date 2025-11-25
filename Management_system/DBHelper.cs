using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Management_system
{
    class DBHelper
    {
        private static string connString =
       "Server=127.0.0.1;Port=3307;Database=doanqlsv;Uid=hieu;Pwd=123456;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsParadiseProject
{
    public static class ConnectionString
    {
        private static string connectionString = @"Data Source=DESKTOP-L18NOD1\SQLEXPRESS02;Initial Catalog=KidsParadise;Integrated Security=True";

        public static string ConnectionStrings
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
    }
}

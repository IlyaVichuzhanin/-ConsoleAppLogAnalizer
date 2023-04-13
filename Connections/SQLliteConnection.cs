using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace ConsoleAppLogAnalizer.Connections
{
    public class SQLliteConnection
    {
        public SQLliteConnection()
        {
            ConnectionStringBuilder=new SqliteConnectionStringBuilder();
            ConnectionStringBuilder.DataSource = "Y:\\Program Files\\RFD\\tNavigator-license\\var\\www\\html\\tNavigator\\statistic\\db.sqlite";
            Connection = new SqliteConnection(ConnectionStringBuilder.ConnectionString);
            
        }
        private SqliteConnectionStringBuilder ConnectionStringBuilder { get; set; }
        public SqliteConnection Connection { get; set; }
    }
}
